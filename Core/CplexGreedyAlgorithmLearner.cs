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
        // for each instance i, for each sequence j, s[i,j]
        SortedList<I, SortedList<Sequence<S, I, O>, IIntVar>> _s;
        // for each attribute name, g[v]
        SortedList<string, INumVar> _g;
        // for each attribute name, a[v[
        SortedList<string, INumVar> _a;
        // for each instance i, for each sequence j, for each step t, gamma[i,j,t]
        SortedList<I, SortedList<Sequence<S, I, O>, SortedList<int, IIntVar>>> _gamma;
        int _nattr = 0;

        public GreedyRule<S, I, O> Learn(List<S> solutions)
        {
            _model = new Cplex();

            SetupVariables(solutions);
            // CONTINUE HERE
            return null;
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
                            _g.Add(attName, _model.NumVar(Double.MinValue, Double.MaxValue, "g_" + (attIndex++)));
                            _a.Add(attName, _model.NumVar(0, Double.MaxValue, "a_" + (attIndex++)));
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
