using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class FunctionGreedyRule<S, I, O> : GreedyRule<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        /// <summary>
        /// Gets the beta, that is, the function coefficients used to evaluate the goodness of the rule
        /// </summary>
        /// <value>The beta.</value>
        public double [] Beta { get; private set; }
        public FunctionGreedyRule(double [] beta)
        {
            Beta = beta;
        }

        /// <summary>
        /// Evaluates the fit of the greedy rule of choosing an option from a certain current solution. It returns the 
        /// sumproduct of the attribute values and the beta coefficients.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>System.Double.</returns>
        protected override double EvaluateQuality(O option, S currentSolution)
        {
            double sum = 0;
            int i = 0;
            foreach (double d in currentSolution.GetAttributesOfOption(option).Values)
                sum += d * Beta[i++];
            return sum;
        }
    }
}
