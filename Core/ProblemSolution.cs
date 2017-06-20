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
    /// <typeparam name="O">The type of Action</typeparam>
    public abstract class ProblemSolution<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
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
                // what are the actions that I can choose from right now?
                foreach (O action in GetFeasibleActions())
                    if (MayLeadToTargetSolution(action, targetSolution))
                    {
                        S newSol = ChooseAction(action);
                        Sequence<S, I, O> newList = new Sequence<S, I, O>(currentList);
                        // add the solution reached to newList
                        newList.Add((S)this, action);
                        foreach (Sequence<S, I, O> l in newSol.SequencesThatMayBuildFrom(newList, targetSolution))
                            yield return l;
                    }
            }
        }

        /// <summary>
        /// Determines whether this instance is feasible.
        /// </summary>
        /// <returns><c>true</c> if this instance is feasible; otherwise, <c>false</c>.</returns>
        public abstract bool IsFeasible();

        /// <summary>
        /// Determines whether the current solution is the same as another solution
        /// </summary>
        /// <param name="other">The other solution.</param>
        /// <returns><c>true</c> if the current solution is the same as the other solution; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected abstract bool IsSameAs(S other);

        /// <summary>
        /// Returns true if the action brings this solution closer to the targetSolution. This method is used in the 
        /// procedure of sequence generation to build the sequences that reach the target solution.
        /// </summary>
        /// <param name="action">The action in consideration.</param>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns><c>true</c> if the action brings this solution closer to the targetSolution, <c>false</c> otherwise.</returns>
        protected abstract bool MayLeadToTargetSolution(O action, S targetSolution);

        /// <summary>
        /// Gets the feasible actions of the current solution for the current problem instance
        /// </summary>
        /// <returns>IEnumerable&lt;Action&gt;.</returns>
        public abstract IEnumerable<O> GetFeasibleActions();

        /// <summary>
        /// Gets the attributes of action.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>the attributes</returns>
        public abstract DataSupport.Row GetAttributesOfAction(O o);

        /// <summary>
        /// Returns a copy of this solution where action o is chosen
        /// </summary>
        /// <param name="o">The o.</param>
        public abstract S ChooseAction(O o);

        /// <summary>
        /// Gets the objective value.
        /// </summary>
        /// <returns>System.Double.</returns>
        public abstract double Value { get; protected set; }

        public abstract override string ToString();
    }
}
