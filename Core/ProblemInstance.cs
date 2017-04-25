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
    public abstract class ProblemInstance<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {

        /// <summary>
        /// Builds an empty solution for this instance. 
        /// </summary>
        public abstract S BuildEmptySolution();

        /// <summary>
        /// Returns a list of sequences of options that may be used to build a certain target solution. It initializes an empty solution; then, 
        /// it builds all sequences of options that can build the target solution. 
        /// </summary>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns>IEnumerable&lt;List&lt;O&gt;&gt;.</returns>
        public IEnumerable<List<O>> SequencesThatMayBuild(S targetSolution)
        {
            S empty = BuildEmptySolution();
            foreach (List<O> list in empty.SequencesThatMayBuildFrom(new List<O>(), targetSolution))
                yield return list;
        }
    }
}
