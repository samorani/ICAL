using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Problems;

namespace IGAL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            double lambda = 0.05;
            CplexGreedyAlgorithmLearner<KP_ProblemSolution, KP_ProblemInstance, KP_Option> learner = new CplexGreedyAlgorithmLearner<KP_ProblemSolution, KP_ProblemInstance, KP_Option>(lambda);
            KP_ProblemInstance inst = new KP_ProblemInstance(new double[] { 3, 2, 5, 3 }, new double[] { 4, 2, 5, 2 }, 5);
            KP_ProblemSolution sol = new KP_ProblemSolution(inst, new bool[] { true, true, false, false });
            List<KP_ProblemSolution> list = new List<KP_ProblemSolution>();
            list.Add(sol);
            GreedyRule<KP_ProblemSolution,KP_ProblemInstance,KP_Option> rule = learner.Learn(list);
            Console.WriteLine("\n\n" + rule.ToString());
        }
    }
}
