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
    public abstract class ProblemInstance 
    {
        public ProblemInstance ()
        { }

        //public abstract IEnumerable<List<O>> SequencesThatMayBuild<P, O>(ProblemSolution<P, O> targetSolution) where P : ProblemInstance where O : Option<P>;
    }
}
