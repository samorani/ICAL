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
    /// <typeparam name="S">The type of ProblemSolution</typeparam>
    /// <typeparam name="I">The type of ProblemInstance</typeparam>
    /// <typeparam name="O">The type of Option</typeparam>
    public abstract class ProblemSolution<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        /// <summary>
        /// Gets the problem instance of this solution.
        /// </summary>
        /// <value>The instance.</value>
        public I Instance { get; protected set; }

        /// <summary>
        /// Find all the possible ways to complete the current solution so that it becomes the target solution
        /// </summary>
        /// <param name="currentList">The current list.</param>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns>IEnumerable&lt;List&lt;O&gt;&gt;.</returns>
        public IEnumerable<Sequence<S, I, O>> SequencesThatMayBuildFrom(Sequence<S, I, O> currentList, S targetSolution)
        {
            // check if we are done
            if (this.IsSameAs(targetSolution))
                yield return currentList;
            else
            {
                // what are the options that I can choose from right now?
                foreach (O option in GetFeasibleOptions())
                    if (BringsCloser(option, targetSolution))
                    {
                        S newSol = ChooseOption(option);
                        Sequence<S, I, O> newList = new Sequence<S, I, O>(currentList);
                        // add the solution reached to newList
                        newList.Add((S)this, option);
                        foreach (Sequence<S, I, O> l in newSol.SequencesThatMayBuildFrom(newList, targetSolution))
                            yield return l;
                    }
            }
        }

        /// <summary>
        /// Determines whether the current solution is the same as another solution
        /// </summary>
        /// <param name="other">The other solution.</param>
        /// <returns><c>true</c> if the current solution is the same as the other solution; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected abstract bool IsSameAs(S other);

        /// <summary>
        /// Returns true if the option brings this solution closer to the targetSolution. This method is used in the 
        /// procedure of sequence generation to build the sequences that reach the target solution.
        /// </summary>
        /// <param name="option">The option in consideration.</param>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns><c>true</c> if the option brings this solution closer to the targetSolution, <c>false</c> otherwise.</returns>
        protected abstract bool BringsCloser(O option, S targetSolution);

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
        /// Returns a copy of this solution where option o is chosen
        /// </summary>
        /// <param name="o">The o.</param>
        public abstract S ChooseOption(O o);

    }
}
