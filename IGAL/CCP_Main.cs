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
        private static void GenerateTrainingSet(string trainingDir)
        {
            int seed = 0;
            DirectoryInfo di = new DirectoryInfo(trainingDir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            CPP_InstanceGenerator generator = new CPP_InstanceGenerator(1, 3, 1, .2, 3);
            for (int p = 2; p < 4; p++)
                for (int n = 4; n < 6; n++)
                    for (seed = 0; seed < 2; seed++)
            generator.GenerateInstance(n, p, seed).WriteToFile(trainingDir + @"\train_" + n + "_" + p + "_" + seed + ".txt");
        }

        public static void CCPMain()
        {
            //CCP_ProblemInstance inst = new CCP_ProblemInstance(4, 2, new double[,] { { 0, 10, 3, 5 },
            //{10,0,5,6 }, {3,5,0,20 }, {5,6,20,0 } }, new double[] { 1, 2, 2, 1 }, 2, 4, "noname");
            //CCP_Grasp_Solver solv = new CCP_Grasp_Solver(0, 0.001);
            //solv.Solve(inst);
            //return;

            string trainingDir = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\training";
            string testDir = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\instances";
            string resultFile = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\results.txt";

            GenerateTrainingSet(trainingDir);

            double lambda = 0.001;
            int maxSeconds = 60;
            int maxAttributes = 1;

            CPP_Cplex_InstanceSolver trainingSetSolver = new CPP_Cplex_InstanceSolver(maxSeconds);
            IGreedyAlgorithmLearner<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> greedyLearner = new CplexGreedyAlgorithmLearner<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>(lambda, maxSeconds,maxAttributes);
            ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> fw = new ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>();
            fw.RunExperiments(trainingDir, testDir,resultFile, new CCP_InstanceReader(),
                trainingSetSolver,greedyLearner,maxSeconds);
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
