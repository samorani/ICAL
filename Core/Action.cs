using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class Action.It represents a feasible action at a given stage of a partially built solution.
    /// </summary>
    public abstract class Action<S, I, O> where S : ProblemSolution<S,I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        /// <summary>
        /// Determines whether this action is the same as another.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if this action is the same as other; otherwise, <c>false</c>.</returns>
        public abstract bool IsSameAs(O other);
    }
}
