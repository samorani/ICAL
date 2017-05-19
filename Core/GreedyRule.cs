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
        /// Chooses the best action among those available for the current solution.
        /// </summary>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>O.</returns>
        public O ChooseAction(S currentSolution)
        {
            double bestVal = Double.MinValue;
            O bestAction = null;
            foreach (O action in currentSolution.GetFeasibleActions())
            {
                double val = EvaluateQuality(action, currentSolution);
                if (val > bestVal)
                {
                    bestVal = val;
                    bestAction = action;
                }
            }
            return bestAction;
        }

        /// <summary>
        /// Evaluates the fit of the greedy rule of choosing an action from a certain current solution. The higher the score, 
        /// the more inclined to select that action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>System.Double.</returns>
        protected abstract double EvaluateQuality(O action, S currentSolution);
    }
}
