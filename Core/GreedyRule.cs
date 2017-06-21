using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class GreedyRule. It represents the rule to follow at each step of a greedy algorithm to decide which action to select.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public abstract class GreedyRule<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        /// <summary>
        /// Returns the possible actions, from the best to the worst
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>IEnumerable&lt;O&gt;.</returns>
        public IEnumerable<O> Best2WorstActions(S solution)
        {
            // first, compute the value of all actions
            List<KeyValuePair<O, double>> actions = GetActionValues(solution);

            // sort the by decreasing values
            actions.Sort((pair1, pair2) => -pair1.Value.CompareTo(pair2.Value));

            // return from best to worst
            foreach (KeyValuePair<O, double> kv in actions)
                yield return kv.Key;
        }

        /// <summary>
        /// Evaluates the fit of the greedy rule of choosing all actions from a certain current solution. The higher the score, 
        /// the more inclined to select that action.
        /// </summary>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>System.Double.</returns>
        protected abstract List<KeyValuePair<O, double>> GetActionValues(S currentSolution);
    }
}
