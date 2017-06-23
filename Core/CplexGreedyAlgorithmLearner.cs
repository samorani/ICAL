using DataSupport;
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
        bool _debug = false;
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
        bool Expand = false;
        int _maxAttributes;

        public CplexGreedyAlgorithmLearner(double lambda, int timeoutSeconds, bool expandAttributes, int maxAttributes = Int32.MaxValue)
        {
            _lambda = lambda;
            _maxSeconds = timeoutSeconds;
            Expand = expandAttributes;
            _maxAttributes = maxAttributes;
        }

        public GreedyRule<S, I, O> Learn(List<S> solutions)
        {
            _model = new Cplex();
            _model.SetParam(Cplex.IntParam.TimeLimit, _maxSeconds);

            SetupVariablesExceptGA(solutions);
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
            FunctionGreedyRule<S, I, O> func = new FunctionGreedyRule<S, I, O>(beta, Expand);

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
                            viol += "We should have selected " + j.Actions[t] + ". The other actions are:\n";
                            foreach (O other in sol.GetFeasibleActions())
                                if (!other.IsSameAs(j.Actions[t]))
                                    viol += other + "\n";
                            Console.Write(viol);
                        }
            _model.End();
            return func;
        }

        private void SetupModel(List<S> solutions)
        {
            StreamWriter sw = new StreamWriter(_descriptionFile,true);
            sw.WriteLine("\n======== CONSTRAINTS ========");


            // constraint (1)
            int i_index = 0;
            foreach (I i in _s.Keys)
            {
                ILinearNumExpr constr = _model.LinearNumExpr();
                foreach (Sequence<S,I,O> j in _s[i].Keys)
                    constr.AddTerm(1, _s[i][j]);
                _model.AddEq(1.0, constr, "C1_" + (i_index++));
            }

            // constraint (2) and g and a
            _g = new SortedList<string, INumVar>();
            _a = new SortedList<string, INumVar>();
            i_index = -1;
            AttributeExpander exp = new AttributeExpander();
            foreach (I i in _gamma.Keys)
            {
                i_index++;
                int j_index = -1;
                if (_debug)
                    Console.WriteLine("Instance: " + i.ToString());

                foreach (Sequence<S, I, O> j in _gamma[i].Keys)
                {
                    j_index++;
                    if (_debug)
                        Console.WriteLine("Sequence: " + j.ToString());
                    foreach (int t in _gamma[i][j].Keys)
                    {
                        // get the object at step t of sequence j
                        O chosen = j.Actions[t];
                        if (_debug)
                            Console.WriteLine("Get object: " + chosen);
                        Row v_ioj_star = j.Solutions[t].GetAttributesOfAction(chosen);
                        List<DataSupport.Column> columns = new List<DataSupport.Column>(v_ioj_star.AttributeValues.Keys);

                        // put everything in a table. At row 0 we have the chosen action.
                        Table dt = new Table(columns);
                        dt.AddRow(v_ioj_star);
                        List<O> feasibleActions = new List<O>(j.Solutions[t].GetFeasibleActions());
                        foreach (O h in feasibleActions)
                            if (!h.IsSameAs(chosen))
                            {
                                Row v_iojh = j.Solutions[t].GetAttributesOfAction(h);
                                dt.AddRow(v_iojh);
                            }
                        if (_debug)
                             Console.WriteLine("ORIGINAL TABLE:\n" + dt);

                        // Expand
                        if (Expand)
                        {
                            if (i_index == 0)
                                exp.BuildAttributeExpressions(columns);

                            // expand the attributes
                            dt = exp.ExpandAttributes(dt);
                        }
                        if (_debug)
                        {
                            Console.WriteLine("MODIFIED TABLE:\n" + dt + "\n... ");
                            Console.ReadLine();
                        }
                        for (int h_index = 1; h_index < dt.Rows.Count; h_index++) // skip v_ioj_star
                        {
                            Row v_iojh = dt.Rows[h_index];
                            v_ioj_star = dt.Rows[0];

                            // add _g and _a variables in the first iteration
                            if (_nattr == 0)
                            {
                                _nattr = v_iojh.Count;
                                int attIndex = 0;
                                foreach (DataSupport.Column col in v_iojh.AttributeValues.Keys)
                                {
                                    string gvarname = "g_" + (attIndex);
                                    string avarname = "a_" + (attIndex++);
                                    _g.Add(col.Name, _model.NumVar(-1.0, 1.0, gvarname));
                                    _a.Add(col.Name, _model.IntVar(0, 1, avarname));
                                    sw.WriteLine(gvarname + ": g(att=" + col.Name + ")");
                                    sw.WriteLine(avarname + ": g(att=" + col.Name + ")");
                                }
                            }

                            // add constraint 2
                            double bigM = _eps;
                            foreach (DataSupport.Column c in v_iojh.AttributeValues.Keys)
                                bigM += Math.Abs(v_iojh[c] - v_ioj_star[c]);

                            ILinearNumExpr constr = _model.LinearNumExpr();
                            foreach (DataSupport.Column c in v_ioj_star.AttributeValues.Keys)
                                constr.AddTerm(v_ioj_star[c], _g[c.Name]);
                            foreach (DataSupport.Column c in v_iojh.AttributeValues.Keys)
                                constr.AddTerm(-v_iojh[c], _g[c.Name]);
                            constr.AddTerm(bigM, _gamma[i][j][t]);
                            constr.AddTerm(-bigM, _s[i][j]);
                            string constName = "2_" + i_index + "_" + j_index + "_" + t +
                            "_" + (h_index);
                            sw.WriteLine(constName + ": i=" + i + "; j=" + j + "; t=" + t + "; chosen action=" + chosen + GetAttributesString(v_ioj_star) + " vs " + feasibleActions[h_index] + GetAttributesString(v_iojh));
                            _model.AddGe(constr, _eps - bigM, constName);
                        }

                        // max attributes constraint
                        ILinearNumExpr sumAttr = _model.LinearNumExpr();
                        foreach (IIntVar iv in _a.Values)
                            sumAttr.AddTerm(1, iv);
                        _model.AddLe(sumAttr, _maxAttributes,"max_attributes");
                    }
                }
            }

            // objective
            ILinearNumExpr obj = _model.LinearNumExpr();
            foreach (I instance in _gamma.Keys)
                foreach (Sequence<S, I, O> seq in _gamma[instance].Keys)
                    foreach (int t in _gamma[instance][seq].Keys)
                        obj.AddTerm(1, _gamma[instance][seq][t]);
            foreach (string att in _a.Keys)
                obj.AddTerm(_lambda, _a[att]);
            _model.AddMinimize(obj);

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

            sw.Close();

        }

        private string GetAttributesString(Row attributes)
        {
            string s = "[";
            int i = 0;
            foreach(DataSupport.Column col in attributes.AttributeValues.Keys)
            {
                string end = ",";
                if (++i == attributes.Count)
                    end = "]";
                s += attributes[col] + end;
            }
            return s;
        }

        private void SetupVariablesExceptGA(List<S> solutions)
        {
            _s = new SortedList<I, SortedList<Sequence<S, I, O>, IIntVar>>();
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
        private double SumproductWithG(Row attributes)
        {
            double score = 0;
            foreach (DataSupport.Column col in attributes.AttributeValues.Keys)
                score += attributes[col.Name] * _model.GetValue(_g[col.Name]);
            return score;
        }
    }
}
