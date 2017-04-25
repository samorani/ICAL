using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class ProblemSolution. It may be a partial or complete solution to a problem instance of a given problem class
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public abstract class ProblemSolution<S, I, O> where S : ProblemSolution<S, I, O> where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        /// <summary>
        /// Gets the problem instance of this solution.
        /// </summary>
        /// <value>The instance.</value>
        public I Instance { get; protected set; }
        

        public ProblemSolution(I instance)
        {
            Instance = instance;
        }

        /// <summary>
        /// Gets the feasible options of the current solution for the current problem instance
        /// </summary>
        /// <returns>IEnumerable&lt;Option&gt;.</returns>
        public abstract IEnumerable<O> GetFeasibleOptions();

        /// <summary>
        /// Gets the attributes of option.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>the attributes</returns>
        public abstract SortedList<string, double> GetAttributesOfOption(O o);

        /// <summary>
        /// Chooses the option and updates this solution
        /// </summary>
        /// <param name="o">The o.</param>
        public abstract S ChooseOption(O o);

    }
}
