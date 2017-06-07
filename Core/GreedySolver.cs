using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class GreedySolver. Choose the best available action.  When no actions are available, 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public class GreedySolver<S, I, O> : ISolver<S,I,O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        private int _curStep;
        private GreedyRule<S, I, O> _rule;
        private int _maxTimeSeconds;
        private DateTime _begin;
        public GreedySolver(GreedyRule<S, I, O> rule, int maxTimeSeconds)
        {
            _rule = rule;
            _maxTimeSeconds = maxTimeSeconds;
        }
        /// <summary>
        /// Solves the specified instance. At each step, it picks the best action according to the rule. The algorithm 
        /// stops when no action is available any more. In that case, if the final solution is infeasible, the algorithm will 
        /// backtrack its choices and select the second best, third best, etc. action.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>S.</returns>
        public S Solve(I instance)
        {
            _begin = DateTime.Now;
            S sol = instance.BuildEmptySolution();
            _curStep = 0;
            foreach (S finalSol in PerformNextStep(sol,_rule))
            {
                return finalSol;
            }
            return null;
        }

        protected IEnumerable<S> PerformNextStep(S curSol, GreedyRule<S, I, O> rule)
        {
            bool atLeastOnePossibleAction = false;
            //Console.WriteLine("At " + curSol);
            foreach (O opt in rule.Best2WorstActions(curSol))
            {
                //Console.WriteLine("Best action is " + opt);
                atLeastOnePossibleAction = true;
                foreach (S s2 in PerformNextStep(curSol.ChooseAction(opt), rule))
                {
                    //Console.WriteLine(++_curStep);
                    yield return s2;
                }
            }
            if (!atLeastOnePossibleAction & curSol.IsFeasible())
            {
                // we are done. If the solution is feasible, return it, if not do nothing (will backtrack)
                yield return curSol;
            }
            if ((DateTime.Now - _begin).TotalSeconds > _maxTimeSeconds)
                yield return null;
            //if (!atLeastOnePossibleAction & !curSol.IsFeasible())
            //    Console.WriteLine("Infeasible: " + curSol);

        }
    }
}
