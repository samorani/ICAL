using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using CCP;
using KP;
using System.IO;
using DataSupport;

namespace IGAL
{
    class CCP_Main
    {
        private static List<CCP_ProblemSolution> GenerateTrainingSet(string trainingDir)
        {
            int seed = 0;
            List<CCP_ProblemSolution> solutions = new List<CCP_ProblemSolution>();
            DirectoryInfo di = new DirectoryInfo(trainingDir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            CPP_Cplex_InstanceSolver solver = new CPP_Cplex_InstanceSolver(60);
            CPP_InstanceGenerator generator = new CPP_InstanceGenerator(1, 10, 1, .5, 3);
            for (int p = 2; p < 3; p++)
                for (int n = 4; n < 5; n++)
                    for (seed = 0; seed < 10; seed++)
                    {
                        CCP_ProblemInstance inst = generator.GenerateInstance(n, p, seed);
                        inst.WriteToFile(trainingDir + @"\train_" + n + "_" + p + "_" + seed + ".txt");
                        CCP_ProblemSolution sol = solver.Solve(inst);
                        if (sol != null)
                            solutions.Add(sol);
                    }
            return solutions;
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

            List<CCP_ProblemSolution>  trainingSet = GenerateTrainingSet(trainingDir);

            double lambda = 0.001;
            int maxSeconds = 120;
            int maxAttributes = 1;
            string resultFile = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\CCP "+maxAttributes + " att.txt";
            AbstractTableModifier modifier = new SymbolicExpansionTableModifier(true);

            ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action> fw = new ExperimentsFW<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>();

            // solve using learner
            //fw.Solver = new CCP_Grasp_Solver(0, 0.05);
            fw.RunExperiments(lambda, trainingSet, testDir, resultFile, new CCP_InstanceReader(), maxSeconds, modifier, maxAttributes);
        }
    }
}
