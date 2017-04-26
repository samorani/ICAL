using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class GreedyRule. It represents the rule to follow at each step of a greedy algorithm to decide which option to select.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public abstract class GreedyRule<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        /// <summary>
        /// Chooses the best option among those available for the current solution.
        /// </summary>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>O.</returns>
        public O ChooseOption(S currentSolution)
        {
            double bestVal = Double.MinValue;
            O bestOption = null;
            foreach (O option in currentSolution.GetFeasibleOptions())
            {
                double val = EvaluateQuality(option, currentSolution);
                if (val > bestVal)
                {
                    bestVal = val;
                    bestOption = option;
                }
            }
            return bestOption;
        }

        /// <summary>
        /// Evaluates the fit of the greedy rule of choosing an option from a certain current solution. The higher the score, 
        /// the more inclined to select that option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>System.Double.</returns>
        protected abstract double EvaluateQuality(O option, S currentSolution);
    }
}
