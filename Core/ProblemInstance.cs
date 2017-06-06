using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a problem instance of a certain class of problems
    /// </summary>
    public abstract class ProblemInstance<S, I, O> : IComparable<ProblemInstance<S, I, O>> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {

        /// <summary>
        /// Builds an empty solution for this instance. 
        /// </summary>
        public abstract S BuildEmptySolution();

        public int CompareTo(ProblemInstance<S, I, O> other)
        {
            return this.ToString().CompareTo(other.ToString());
        }

        public abstract override string ToString();


        /// <summary>
        /// Returns a list of sequences of actions that may be used to build a certain target solution. It initializes an empty solution; then, 
        /// it builds all sequences of actions that can build the target solution. 
        /// </summary>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns>IEnumerable&lt;List&lt;O&gt;&gt;.</returns>
        public IEnumerable<Sequence<S,I,O>> SequencesThatMayBuild(S targetSolution)
        {
            S empty = BuildEmptySolution();
            foreach (Sequence<S, I, O> list in empty.SequencesThatMayBuildFrom(new Sequence<S, I, O>(), targetSolution))
                yield return list;
        }
    }
}
