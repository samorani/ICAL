using ILOG.Concert;
using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CplexGreedyAlgorithmLearner<S, I, O> : IGreedyAlgorithmLearner<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        Cplex _model;
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

        public CplexGreedyAlgorithmLearner(double lambda)
        {
            _lambda = lambda;
        }

        public GreedyRule<S, I, O> Learn(List<S> solutions)
        {
            _model = new Cplex();

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
            Console.Write("ObjVal = " + Math.Round(ObjectiveValue, 4) + ". ");

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
            _model.End();
            return func;
        }

        private void SetupModel(List<S> solutions)
        {
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
            i_index = 0;
            foreach (I i in _gamma.Keys)
            {
                int j_index = 0;

                foreach (Sequence<S, I, O> j in _gamma[i].Keys)
                    foreach (int t in _gamma[i][j].Keys)
                    {
                        // get the object at step t of sequence j
                        O chosen = j.Options[t];
                        SortedList<string, double> v_ioj_star = j.Solutions[t].GetAttributesOfOption(chosen);
                        int h_index = -1;
                        foreach (O h in j.Solutions[t].GetFeasibleOptions())
                        {
                            h_index++;
                            if (!h.IsSameAs(chosen))
                            {
                                // add constraint 2
                                SortedList<string, double> v_iojh = j.Solutions[t].GetAttributesOfOption(h);
                                double bigM = _eps;
                                foreach (double val in v_iojh.Values)
                                    bigM += Math.Abs(val);

                                ILinearNumExpr constr = _model.LinearNumExpr();
                                foreach (string att in v_ioj_star.Keys)
                                    constr.AddTerm(v_ioj_star[att], _g[att]);
                                foreach (string att in v_iojh.Keys)
                                    constr.AddTerm(-v_iojh[att], _g[att]);
                                constr.AddTerm(bigM, _gamma[i][j][t]);
                                constr.AddTerm(-bigM, _s[i][j]);

                                _model.AddGe(constr, _eps - bigM, "2_" + (i_index++) + "_" + (j_index++) + "_" + t +
                                "_" + (h_index++));
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

        }

        private void SetupVariables(List<S> solutions)
        {
            _s = new SortedList<I, SortedList<Sequence<S, I, O>, IIntVar>>();
            _g = new SortedList<string, INumVar>();
            _a = new SortedList<string, INumVar>();
            _gamma = new SortedList<I, SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>>();

            int instanceIndex = 0;

            // set up the variables
            foreach (S sol in solutions)
            {
                int seqIndex = 0;
                I inst = sol.Instance;
                _s.Add(inst, new SortedList<Sequence<S, I, O>, IIntVar>());
                _gamma.Add(inst, new SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>());
                foreach (Sequence<S,I,O> seq in inst.SequencesThatMayBuild(sol))
                {
                    if (_nattr == 0)
                    {
                        SortedList<string, double> attributes = sol.GetAttributesOfOption(seq.Options[0]);
                        _nattr = attributes.Count;
                        int attIndex = 0;
                        foreach (string attName in attributes.Keys)
                        {
                            _g.Add(attName, _model.NumVar(-1.0, 1.0, "g_" + (attIndex)));
                            _a.Add(attName, _model.IntVar(0, 1, "a_" + (attIndex++)));
                        }
                    }
                    IIntVar s_ij = _model.IntVar(0, 1, "s_" + instanceIndex + "_" + seqIndex);
                    _s[inst].Add(seq, s_ij);
                    _gamma[inst].Add(seq, new SortedList<int, IIntVar>());
                    // make all components of gamma[i,j]
                    for (int t = 0; t < seq.Count; t++)
                        _gamma[inst][seq].Add(t, _model.IntVar(0, 1, "gamma_" + instanceIndex + "_" + seqIndex + "_" + t));

                    seqIndex++;
                }
                instanceIndex++;
            }
        }
    }
}
