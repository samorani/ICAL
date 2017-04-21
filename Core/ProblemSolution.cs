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
    /// <typeparam name="T"></typeparam>
    public abstract class ProblemSolution<T> where T : ProblemClass
    {
        public ProblemInstance<T> Instance { get; private set; }

        public ProblemSolution(ProblemInstance<T> instance)
        {
            Instance = instance;
        }

        /// <summary>
        /// Gets the feasible options of the current solution for the current problem instance
        /// </summary>
        /// <returns>IEnumerable&lt;Option&lt;T&gt;&gt;.</returns>
        public abstract IEnumerable<Option<T>> GetFeasibleOptions();

        /// <summary>
        /// Gets the attributes of option.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>System.Double[].</returns>
        public abstract double[] GetAttributesOfOption(Option<T> o);
    }
}
