using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Interface IGreedyAlgorithmLearner.  It learns a greedy rule from a list of solutions.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public interface IGreedyAlgorithmLearner<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        GreedyRule<S, I, O> Learn(List<S> solutions);
    }
}
