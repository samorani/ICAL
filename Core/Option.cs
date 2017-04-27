using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class Option.It represents a feasible option at a given stage of a partially built solution.
    /// </summary>
    public abstract class Option<S, I, O> where S : ProblemSolution<S,I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        /// <summary>
        /// Determines whether this option is the same as another.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if this option is the same as other; otherwise, <c>false</c>.</returns>
        public abstract bool IsSameAs(O other);
    }
}
