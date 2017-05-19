using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GreedySolver<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        public S Solve(I instance, GreedyRule<S,I,O> rule)
        {
            S sol = instance.BuildEmptySolution();
            while (true)
            {
                O chosenOpt = rule.ChooseAction(sol);
                if (chosenOpt == null)
                    break;
                else
                    sol = sol.ChooseAction(chosenOpt);
            }
            return sol;
        }
    }
}
