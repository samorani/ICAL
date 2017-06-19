using ILOG.Concert;
using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CplexGreedyAlgorithmLearner<S, I, O> : IGreedyAlgorithmLearner<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        Cplex _model;
        string _descriptionFile = "..\\..\\..\\variables.txt";
        public double ObjectiveValue { get; set; }
        // for each instance i, for each sequence j, s[i,j]
        SortedList<I, SortedList<Sequence<S, I, O>, IIntVar>> _s;
        // for each attribute name, g[v]
        SortedList<string, INumVar> _g;
        // for each attribute name, a[v[
        SortedList<string, INumVar> _a;
        // for each instance i, for each sequence j, for each step t, gamma[i,j,t]
        SortedList<I, SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>> _gamma;
        int _nattr = 0;
        double _lambda = 0;
        double _eps = 0.001;
        int _maxSeconds;
        int _maxAttributes;

        public CplexGreedyAlgorithmLearner(double lambda, int timeoutSeconds, int maxAttributes)
        {
            _lambda = lambda;
            _maxSeconds = timeoutSeconds;
            _maxAttributes = maxAttributes;
        }

        public GreedyRule<S, I, O> Learn(List<S> solutions)
        {
            _model = new Cplex();
            _model.SetParam(Cplex.IntParam.TimeLimit, _maxSeconds);

            SetupVariables(solutions);
            SetupModel(solutions);
            return Solve();
        }

        private GreedyRule<S, I, O> Solve()
        {
            _model.ExportModel("..\\..\\..\\model.lp");
            Console.Write("Solving... ");
            //_model.SetOut(null);
            _model.Solve();
            _model.WriteSolution("..\\..\\..\\solution.lp");
            ObjectiveValue = _model.ObjValue;
            Console.WriteLine("ObjVal = " + Math.Round(ObjectiveValue, 4) + ". \n");

            if (!_model.IsPrimalFeasible())
            {
                Console.WriteLine("*****************************");
                Console.WriteLine("NOT FEASIBLE");
            }

            // get values of g
            SortedList<string, double> beta = new SortedList<string, double>();
            foreach (string att in _g.Keys)
                beta.Add(att, _model.GetValue(_g[att]));
            FunctionGreedyRule<S, I, O> func = new FunctionGreedyRule<S, I, O>(beta);

            // get violations to the rule
            foreach (I i in _gamma.Keys)
                foreach (Sequence<S,I,O> j in _gamma[i].Keys)
                    foreach (int t in _gamma[i][j].Keys)
                        if (_model.GetValue(_gamma[i][j][t]) > 0.01)
                        {
                            // I found a violation at step t of sequence j of instance i
                            string viol = "******** VIOLATION ********\n";

                            S sol = j.Solutions[t];
                            viol += "Violation in instance " + i + " sequence "+j+ "\n";
                            viol += "At step " + t + ", here is the solution:\n";
                            viol += sol + "\n";
                            viol += "We should have selected " + j.Actions[t] + ", with score "+ Math.Round(SumproductWithG(sol.GetAttributesOfAction(j.Actions[t])),6) + ". The other actions are:\n";
                            foreach (O other in sol.GetFeasibleActions())
                                if (!other.IsSameAs(j.Actions[t]))
                                    viol += other + ", whose score is " + Math.Round(SumproductWithG(sol.GetAttributesOfAction(other)), 6) + "\n";
                            Console.Write(viol);
                        }
            _model.End();
            return func;
        }

        private void SetupModel(List<S> solutions)
        {
            StreamWriter sw = new StreamWriter(_descriptionFile,true);
            sw.WriteLine("\n======== CONSTRAINTS ========");

            // objective
            ILinearNumExpr obj = _model.LinearNumExpr();
            foreach (I instance in _gamma.Keys)
                foreach (Sequence<S, I, O> seq in _gamma[instance].Keys)
                    foreach (int t in _gamma[instance][seq].Keys)
                        obj.AddTerm(1, _gamma[instance][seq][t]);
            foreach (string att in _a.Keys)
                obj.AddTerm(_lambda, _a[att]);
            _model.AddMinimize(obj);

            // constraint (1)
            int i_index = 0;
            foreach (I i in _s.Keys)
            {
                ILinearNumExpr constr = _model.LinearNumExpr();
                foreach (Sequence<S,I,O> j in _s[i].Keys)
                    constr.AddTerm(1, _s[i][j]);
                _model.AddEq(1.0, constr, "C1_" + (i_index++));
            }

            // constraint (2)
            i_index = -1;
            foreach (I i in _gamma.Keys)
            {
                i_index++;
                int j_index = -1;

                foreach (Sequence<S, I, O> j in _gamma[i].Keys)
                {
                    j_index++;
                    foreach (int t in _gamma[i][j].Keys)
                    {
                        // get the object at step t of sequence j
                        O chosen = j.Actions[t];
                        SortedList<string, double> v_ioj_star = j.Solutions[t].GetAttributesOfAction(chosen);
                        int h_index = -1;
                        foreach (O h in j.Solutions[t].GetFeasibleActions())
                        {
                            h_index++;
                            if (!h.IsSameAs(chosen))
                            {
                                // add constraint 2
                                SortedList<string, double> v_iojh = j.Solutions[t].GetAttributesOfAction(h);
                                double bigM = _eps;
                                for (int v = 0; v < v_iojh.Count; v++)
                                    bigM += Math.Abs(v_iojh.Values[v] - v_ioj_star.Values[v]);

                                ILinearNumExpr constr = _model.LinearNumExpr();
                                foreach (string att in v_ioj_star.Keys)
                                    constr.AddTerm(v_ioj_star[att], _g[att]);
                                foreach (string att in v_iojh.Keys)
                                    constr.AddTerm(-v_iojh[att], _g[att]);
                                constr.AddTerm(bigM, _gamma[i][j][t]);
                                constr.AddTerm(-bigM, _s[i][j]);
                                string constName = "2_" + i_index + "_" + j_index + "_" + t +
                                "_" + (h_index++);
                                sw.WriteLine(constName + ": i=" + i + "; j=" + j + "; t=" + t + "; chosen action=" + chosen + GetAttributesString(v_ioj_star) + " vs " + h + GetAttributesString(v_iojh));
                                _model.AddGe(constr, _eps - bigM, constName);
                            }
                        }
                    }
                }
            }

            // constraints (4) and (5)
            foreach (string att in _a.Keys)
            {
                ILinearNumExpr expr = _model.LinearNumExpr();
                expr.AddTerm(1, _a[att]);
                expr.AddTerm(-1, _g[att]);
                _model.AddGe(expr, 0);
                expr = _model.LinearNumExpr();
                expr.AddTerm(1, _a[att]);
                expr.AddTerm(1, _g[att]);
                _model.AddGe(expr, 0);
            }

            // max attributes constraint
            ILinearNumExpr expr6 = _model.LinearNumExpr();
            foreach (string att in _a.Keys)
                expr6.AddTerm(1, _a[att]);
            _model.AddLe(expr6, _maxAttributes + 0.0001, "max_attributes");

            sw.Close();

        }

        private string GetAttributesString(SortedList<string, double> attributes)
        {
            string s = "[";
            int i = 0;
            foreach(string att in attributes.Keys)
            {
                string end = ",";
                if (++i == attributes.Count)
                    end = "]";
                s += attributes[att] + end;
            }
            return s;
        }

        private void SetupVariables(List<S> solutions)
        {
            _s = new SortedList<I, SortedList<Sequence<S, I, O>, IIntVar>>();
            _g = new SortedList<string, INumVar>();
            _a = new SortedList<string, INumVar>();
            _gamma = new SortedList<I, SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>>();

            int instanceIndex = 0;

            StreamWriter sw = new StreamWriter(_descriptionFile);
            sw.WriteLine("======== VARIABLES ========");
            // set up the variables
            foreach (S sol in solutions)
            {
                Console.WriteLine("Setting up variables for " + sol.Instance);
                int seqIndex = 0;
                I inst = sol.Instance;
                _s.Add(inst, new SortedList<Sequence<S, I, O>, IIntVar>());
                _gamma.Add(inst, new SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>());
                foreach (Sequence<S,I,O> seq in inst.SequencesThatMayBuild(sol))
                {
                    if (_nattr == 0)
                    {
                        SortedList<string, double> attributes = sol.GetAttributesOfAction(seq.Actions[0]);
                        _nattr = attributes.Count;
                        int attIndex = 0;
                        foreach (string attName in attributes.Keys)
                        {
                            string gvarname = "g_" + (attIndex);
                            string avarname = "a_" + (attIndex++);
                            _g.Add(attName, _model.NumVar(-1.0, 1.0, gvarname));
                            _a.Add(attName, _model.IntVar(0, 1, avarname));
                            sw.WriteLine(gvarname + ": g(att=" + attName + ")");
                            sw.WriteLine(avarname + ": g(att=" + attName + ")");
                        }
                    }
                    string svarname = "s_" + instanceIndex + "_" + seqIndex;
                    IIntVar s_ij = _model.IntVar(0, 1, svarname);
                    sw.WriteLine(svarname + ": s(i=" + inst + ", j="+seq+")");

                    _s[inst].Add(seq, s_ij);
                    _gamma[inst].Add(seq, new SortedList<int, IIntVar>());
                    // make all components of gamma[i,j]
                    for (int t = 0; t < seq.Count; t++)
                    {
                        string gammavarname = "gamma_" + instanceIndex + "_" + seqIndex + "_" + t;
                        _gamma[inst][seq].Add(t, _model.IntVar(0, 1, gammavarname ));
                        sw.WriteLine(gammavarname + ": gamma(i=" + inst + ", j=" + seq + ",t="+t + ")");
                    }
                    seqIndex++;
                    if (seqIndex % 100 == 0)
                        Console.WriteLine("Explored " + seqIndex + " sequences");
                }
                instanceIndex++;
            }
            sw.Close();
        }

        /// <summary>
        /// Computes the sumproducts between g and the attributes of an action, assuming that the model has been solved.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns>System.Double.</returns>
        private double SumproductWithG(SortedList<string,double> attributes)
        {
            double score = 0;
            foreach (string att in attributes.Keys)
                score += attributes[att] * _model.GetValue(_g[att]);
            return score;
        }
    }
}
