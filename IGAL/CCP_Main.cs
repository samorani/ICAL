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
            bool expandAttributes = false;
            int maxAttributes = 3;

            ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> fw = new ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>();

            // solve using learner
            fw.RunExperiments(lambda, trainingDir, testDir, resultFile, new CCP_InstanceReader(), new CCP_Grasp_Solver(0, 0.05), maxSeconds, expandAttributes, maxAttributes);

            // solve using GRASP
            //fw.RunExperiments(lambda, trainingDir, testDir, resultFile, new CCP_InstanceReader(), new CCP_Grasp_Solver(0, 0.05), maxSeconds);
        }
    }
}
