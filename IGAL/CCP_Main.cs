using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using CCP;
using KP;
using System.IO;

namespace IGAL
{
    class CCP_Main
    {
        private static void GenerateTrainingSet()
        {
            int seed = 0;
            CPP_InstanceGenerator generator = new CPP_InstanceGenerator(1, 3, 1, .2, 3);
            int n = 5;
            int p = 2;
            for (seed = 0; seed < 3; seed++)
            generator.GenerateInstance(n, p, seed).WriteToFile(@"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\training\train_" + n + "_" + p + "_" + seed + ".txt");
        }

        public static void CCPMain()
        {
            GenerateTrainingSet();

            double lambda = 0.0;
            string trainingDir = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\training";
            string testDir = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\instances";
            ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> fw = new ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>();
            fw.RunExperiments(trainingDir, testDir, new CCP_InstanceReader(), new CPP_InstanceSolver());
            //foreach (FileInfo f in d.GetFiles())
            //    training.Add()

            //CCP_ProblemInstance i1 = new CCP_ProblemInstance(4, 2, new double[,] { { 0, 10, 3, 5 },
            //{10,0,5,6 }, {3,5,0,20 }, {5,6,20,0 } }, new double[] { 1, 2, 2, 1 }, 2, 4);
            //CCP_ProblemSolution s1 = new CCP_ProblemSolution(i1, new int[] { 0, 0, 1, 1 });
            //CCP_ProblemInstance i2 = new CCP_ProblemInstance(4, 2, new double[,] { { 0, 1, 0, 0 },
            //{1,0,0,0 }, {0,0,0,0 }, {0,0,0,0 } }, new double[] { 2, 2, 3, 3 }, 5, 5);
            //CCP_ProblemSolution s2 = new CCP_ProblemSolution(i2, new int[] { 0, 1, 0, 1 });
            //training.Add(s1);
            ////training.Add(s2);
            //CplexGreedyAlgorithmLearner<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> learner = 
            //    new CplexGreedyAlgorithmLearner<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>(lambda);

            //GreedyRule<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> rule = learner.Learn(training);
            //Console.WriteLine("\n******** RULE ********");
            //Console.WriteLine(rule.ToString());

            //GreedySolver<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> solver = new GreedySolver<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>();
            //Console.WriteLine(solver.Solve(i2, rule));
        }
    }
}
