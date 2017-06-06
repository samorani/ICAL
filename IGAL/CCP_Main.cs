using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Problems;

namespace IGAL
{
    class CCP_Main
    {
        public static void CCPMain()
        {
            double lambda = 0.0;

            CCP_ProblemInstance i1 = new CCP_ProblemInstance(4, 2, new double[,] { { 0, 10, 3, 5 },
            {10,0,5,6 }, {3,5,0,20 }, {5,6,20,0 } }, new double[] { 1, 2, 2, 1 }, 2, 4);
            CCP_ProblemSolution s1 = new CCP_ProblemSolution(i1, new int[] { 0, 0, 1, 1 });
            List<CCP_ProblemSolution> training = new List<CCP_ProblemSolution>();
            training.Add(s1);
            CplexGreedyAlgorithmLearner<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> learner = 
                new CplexGreedyAlgorithmLearner<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>(lambda);

            GreedyRule<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> rule = learner.Learn(training);
            Console.WriteLine("\n******** RULE ********");
            Console.WriteLine(rule.ToString());

        }
    }
}
