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
    public class CplexConstructiveAlgorithmLearner<S, I, O> : IGreedyAlgorithmLearner<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        Cplex _model;
        bool _debug = false;
        bool _penalizeNumberOfAttributes;

        string _descriptionFile = "..\\..\\..\\variables.txt";
        public double ObjectiveValue { get; set; }
        public double OptimalityGap { get; private set; }
        public double LowerBound { get; private set; }
        // for each instance i, for each sequence j, s[i,j]
        SortedList<I, SortedList<Sequence<S, I, O>, IIntVar>> _s;
        // for each attribute name, g[v]
        SortedList<string, INumVar> _g;
        // for each attribute name, a[v[
        SortedList<string, INumVar> _a;
        // for each instance i, for each sequence j, for each step t, gamma[i,j,t]
        SortedList<I, SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>> _gamma;
        INumVar _sep; // minimum separation
        public double Sep;

        SortedList<I, S> _optimalSolutionsTraining;

        int _nattr = 0;
        double _lambda = 0;
        double _eps = 0.0000001; // 0.001
        int _maxSeconds;
        AbstractTableModifier _modifier;
        int _maxAttributes;
        public int NumberOfGeneratedAttributes { get; set; }

        public CplexConstructiveAlgorithmLearner(double lambda, int timeoutSeconds,
            bool PenalizeNumberOfAttributes, AbstractTableModifier modifier = null, int maxAttributes = Int32.MaxValue)
        {
            _lambda = lambda;
            _maxSeconds = timeoutSeconds;
            _modifier = modifier;
            _maxAttributes = maxAttributes;
            _penalizeNumberOfAttributes = PenalizeNumberOfAttributes;
            NumberOfGeneratedAttributes = -1;
        }

        public GreedyRule<S, I, O> Learn(List<S> solutions)
        {
            _optimalSolutionsTraining = new SortedList<I, S>();
            foreach (S sol in solutions)
                _optimalSolutionsTraining.Add(sol.Instance, sol);

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
            //_model.Use(new MyCallBack(_maxAttributes,_lambda));
            _model.Solve();
            _model.WriteSolution("..\\..\\..\\solution.lp");
            ObjectiveValue = _model.ObjValue;
            OptimalityGap = Math.Round(_model.GetMIPRelativeGap(), 6);
            LowerBound = Math.Round(_model.GetBestObjValue(), 6);
            Sep = _model.GetValue(_sep);
            Console.WriteLine("ObjVal = " + Math.Round(ObjectiveValue, 4) + ". \n");
            Console.WriteLine("Sep = " + Sep + ". \n");
            Console.WriteLine("OptimalityGap = " + Math.Round(OptimalityGap, 4) + ". \n");

            if (!_model.IsPrimalFeasible())
            {
                Console.WriteLine("*****************************");
                Console.WriteLine("NOT FEASIBLE");
            }

            // iterate all solutions
            // values[i,j] is the value on instance j found by solution i
            double[] gaps= new double[_model.GetSolnPoolNsolns()];
            Console.WriteLine("There are " + _model.GetSolnPoolNsolns() + " solutions and " +
                _s.Keys.Count + " instances to solve");
            for (int nsol = 0; nsol < _model.GetSolnPoolNsolns(); nsol++)
            {
                double obj = _model.GetObjValue(nsol);
 
                Console.Write(Math.Round(obj,3) + "\t");
                SortedList<string, double> beta = new SortedList<string, double>();
                foreach (string att in _g.Keys)
                {
                    // add to beta only the non-zero attributes
                    double val = _model.GetValue(_g[att],nsol);
                    beta.Add(att, val);
                }
                GreedySolver<S, I, O> solver = new GreedySolver<S, I, O>(new FunctionGreedyRule<S, I, O>(beta, _modifier), _maxSeconds, _modifier);
                // evaluate this solution on training set
                for (int i=0;i<_s.Keys.Count;i++)
                {
                    
                    I inst = _s.Keys[i];
                    S sol = solver.Solve(inst);
                    double val = sol.IsFeasible() ? sol.Value : 0;
                    double opt = _optimalSolutionsTraining[inst].Value;
                    // define gap depending on max or min problem
                    double gap = opt > val? (opt - val) / opt : (val - opt) / opt;
                    gaps[nsol] += gap / (_s.Keys.Count + 0.0);
                }
                Console.WriteLine("Avg gap = " + Math.Round(gaps[nsol],7));
            }

            // find the best solution (the one with the lowest avg gap)



            // get values of g
            SortedList<string, double> bestBeta = new SortedList<string, double>();
            foreach (string att in _g.Keys)
            {
                // add to beta only the non-zero attributes
                double val = _model.GetValue(_g[att]);
                Console.WriteLine(att + ": " + val);
                if (Math.Abs(val) > 0)
                    bestBeta.Add(att, val);
                else if (_modifier != null)
                    _modifier.SilenceAttribute(att);
            }
            FunctionGreedyRule<S, I, O> func = new FunctionGreedyRule<S, I, O>(bestBeta, _modifier);

            // get violations to the rule
            foreach (I i in _gamma.Keys)
                foreach (Sequence<S, I, O> j in _gamma[i].Keys)
                    foreach (int t in _gamma[i][j].Keys)
                        if (_model.GetValue(_gamma[i][j][t]) > 0.01)
                        {
                            // I found a violation at step t of sequence j of instance i
                            string viol = "******** VIOLATION ********\n";

                            S sol = j.Solutions[t];
                            viol += "Violation in instance " + i + " sequence " + j + "\n";
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
            StreamWriter sw = new StreamWriter(_descriptionFile, true);
            sw.WriteLine("\n======== CONSTRAINTS ========");

            Random rand = new Random(0);

            // constraint (1)
            int i_index = 0;
            foreach (I i in _s.Keys)
            {
                ILinearNumExpr constr = _model.LinearNumExpr();
                foreach (Sequence<S, I, O> j in _s[i].Keys)
                    constr.AddTerm(1, _s[i][j]);
                _model.AddEq(1.0, constr, "C1_" + (i_index++));
            }

            // constraint (2) and g and a
            _g = new SortedList<string, INumVar>();
            _a = new SortedList<string, INumVar>();
            i_index = -1;
            foreach (I i in _gamma.Keys)
            {
                Console.WriteLine("Setting up constraints for instance " + (i_index+1) + " out of " + _gamma.Count);
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
                        if (_modifier != null)
                        {
                            // expand the attributes
                            dt = _modifier.Modify(dt);
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
                                NumberOfGeneratedAttributes = v_iojh.AttributeValues.Count;
                                foreach (DataSupport.Column col in v_iojh.AttributeValues.Keys)
                                {
                                    string gvarname = "g_" + (attIndex);
                                    string avarname = "a_" + (attIndex++);
                                    _g.Add(col.Name, _model.NumVar(-1.0, 1.0, gvarname));
                                    if (_penalizeNumberOfAttributes)
                                        _a.Add(col.Name, _model.IntVar(0, 1, avarname));
                                    else
                                        _a.Add(col.Name, _model.NumVar(0, 1, avarname));
                                    sw.WriteLine(gvarname + ": g(att=" + col.Name + ")");
                                    sw.WriteLine(avarname + ": g(att=" + col.Name + ")");
                                }
                            }

                            // add constraint 2
                            double bigM = 0;
                            double eps = 100;
                            foreach (DataSupport.Column c in v_iojh.AttributeValues.Keys)
                            {
                                double diff = Math.Abs(v_iojh[c] - v_ioj_star[c]);
                                bigM += diff;
                                if (diff > 0 && diff < eps)
                                    eps = diff;
                            }
                            if (bigM == 0)
                            {
                                eps = 0.0;
                            }
                            bigM += eps;
                            ILinearNumExpr constr = _model.LinearNumExpr();
                            constr.AddTerm(-1, _sep);
                            foreach (DataSupport.Column c in v_ioj_star.AttributeValues.Keys)
                                constr.AddTerm(v_ioj_star[c], _g[c.Name]);
                            foreach (DataSupport.Column c in v_iojh.AttributeValues.Keys)
                                constr.AddTerm(-v_iojh[c], _g[c.Name]);
                            constr.AddTerm(bigM, _gamma[i][j][t]);
                            constr.AddTerm(-bigM, _s[i][j]);
                            string constName = "2_" + i_index + "_" + j_index + "_" + t +
                            "_" + (h_index);
                            sw.WriteLine(constName + ": i=" + i + "; j=" + j + "; t=" + t + "; chosen action=" + chosen + GetAttributesString(v_ioj_star) + " vs " + feasibleActions[h_index] + GetAttributesString(v_iojh));
                            _model.AddGe(constr, eps - bigM, constName);
                        }

                        // max attributes constraint
                        if (_penalizeNumberOfAttributes)
                        {
                            ILinearNumExpr sumAttr = _model.LinearNumExpr();
                            foreach (IIntVar iv in _a.Values)
                                sumAttr.AddTerm(1, iv);
                            _model.AddLe(sumAttr, _maxAttributes, "max_attributes");
                        }
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
            //obj.AddTerm(-1, _sep);
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
                if (att.Equals("[p]/[w]"))
                {
                    //Console.WriteLine("SETTING UP p/w = 1");
                    //Console.ReadLine();
                    //_model.AddEq(_a[att],  1);
                    //_model.AddEq(_g[att], 1);
                }
                //_model.SetPriority(_a[att], 1);
            }

            sw.Close();

        }

        private string GetAttributesString(Row attributes)
        {
            string s = "[";
            int i = 0;
            foreach (DataSupport.Column col in attributes.AttributeValues.Keys)
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
            _sep = _model.NumVar(0, Double.MaxValue, "minsep");
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
                foreach (Sequence<S, I, O> seq in inst.SequencesThatMayBuild(sol))
                {
                    string svarname = "s_" + instanceIndex + "_" + seqIndex;
                    IIntVar s_ij = _model.IntVar(0, 1, svarname);
                    sw.WriteLine(svarname + ": s(i=" + inst + ", j=" + seq + ")");

                    _s[inst].Add(seq, s_ij);
                    _gamma[inst].Add(seq, new SortedList<int, IIntVar>());
                    // make all components of gamma[i,j]
                    for (int t = 0; t < seq.Count; t++)
                    {
                        string gammavarname = "gamma_" + instanceIndex + "_" + seqIndex + "_" + t;
                        _gamma[inst][seq].Add(t, _model.IntVar(0, 1, gammavarname));
                        sw.WriteLine(gammavarname + ": gamma(i=" + inst + ", j=" + seq + ",t=" + t + ")");
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

        ///// <summary>
        ///// Class MyCallBack. Prunes the current node if its lower bound is impossible
        ///// </summary>
        ///// <seealso cref="ILOG.CPLEX.Cplex.BranchCallback" />
        //protected class MyCallBack : Cplex.BranchCallback
        //{
        //    int _nattr;
        //    double _lambda;
        //    public MyCallBack(int nattributes, double lambda)
        //    {
        //        _nattr = nattributes;
        //        _lambda = lambda;
        //    }
        //    public override void Main()
        //    {
        //        if (_nattr >= 9)
        //            return;
        //        double d = this.GetBestObjValue();
        //        double fract = d - Math.Floor(d);
        //        //Console.WriteLine("I am inside the callback. LB = " + d +". Fractional part = "+fract);
        //        if (fract > _lambda * _nattr + 0.0000000001)
        //        {
        //            //Console.WriteLine("Prune!");
        //            //Console.ReadLine();
        //            this.Prune();
        //        }
        //    }
        //}
    }
}