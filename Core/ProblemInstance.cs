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
    public abstract class ProblemInstance<S, I, O> where S : ProblemSolution<S, I, O> where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        public ProblemInstance ()
        { }

        public abstract IEnumerable<List<O>> SequencesThatMayBuild(S targetSolution);
    }
}
