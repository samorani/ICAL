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
            CplexGreedyAlgorithmLearner<KP_ProblemSolution, KP_ProblemInstance, KP_Option> learner = new CplexGreedyAlgorithmLearner<KP_ProblemSolution, KP_ProblemInstance, KP_Option>();
            KP_ProblemInstance inst = new KP_ProblemInstance(new double[] { 1, 2, 3, 4 }, new double[] { 2, 2, 2, 1 }, 4);
            KP_ProblemSolution sol = new KP_ProblemSolution(inst, new bool[] { true, true, false, false });
            List<KP_ProblemSolution> list = new List<KP_ProblemSolution>();
            list.Add(sol);
            learner.Learn(list);
        }
    }
}
