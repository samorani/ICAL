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
    /// <typeparam name="P"></typeparam>
    public abstract class ProblemSolution <P,O> where P : ProblemInstance where O : Option
    {
        /// <summary>
        /// Gets the problem instance of this solution.
        /// </summary>
        /// <value>The instance.</value>
        public P Instance { get; protected set; }
        

        public ProblemSolution(P instance)
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
        public abstract void ChooseOption(O o);

    }
}
