using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Interface ISolver. Solves a problem instance
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public interface ISolver<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        S Solve(I instance);
    }
}
