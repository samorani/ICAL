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
    public class GreedySolver<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        /// <summary>
        /// Solves the specified instance. At each step, it picks the best action according to the rule. The algorithm 
        /// stops when no action is available any more. In that case, if the final solution is infeasible, the algorithm will 
        /// backtrack its choices and select the second best, third best, etc. action.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>S.</returns>
        public S Solve(I instance, GreedyRule<S,I,O> rule)
        {
            S sol = instance.BuildEmptySolution();

            foreach (S finalSol in PerformNextStep(sol,rule))
            {
                return finalSol;
            }
            return null;
        }

        public IEnumerable<S> PerformNextStep(S curSol, GreedyRule<S, I, O> rule)
        {
            bool atLeastOnePossibleAction = false;
            foreach (O opt in rule.Best2WorstActions(curSol))
            {
                atLeastOnePossibleAction = true;
                foreach (S s2 in PerformNextStep(curSol.ChooseAction(opt), rule))
                    yield return s2;
            }
            if (!atLeastOnePossibleAction & curSol.IsFeasible())
            {
                // we are done. If the solution is feasible, return it, if not do nothing (will backtrack)
                yield return curSol;
            }
        }
    }
}
