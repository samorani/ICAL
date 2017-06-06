using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Class GreedySolver. Choose the best available action.  When no actions are available, 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public class GreedySolver<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        public S Solve(I instance, GreedyRule<S,I,O> rule)
        {
            S sol = instance.BuildEmptySolution();
            while (true)
            {
                O chosenOpt = null;
                foreach (O opt in rule.Best2WorstActions(sol))
                {
                    chosenOpt = opt;
                    break;
                }
                if (chosenOpt == null)
                    break;
                else
                    sol = sol.ChooseAction(chosenOpt);
            }
            return sol;
        }

        //public S AddOneComponent(S curSol, GreedyRule<S, I, O> rule)
        //{
        //    O chosenOpt = rule.ChooseAction(curSol);
        //    if (chosenOpt != null)
        //        return AddOneComponent(curSol.ChooseAction(chosenOpt), rule);
        //    else
        //    {
        //        // chosenOpt is null. That means that either we are done or we cannot build a feasible solution from here
        //    }
        //}
    }
}
