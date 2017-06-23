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
    class KP_Main
    {
        private static List<KP_ProblemSolution> GenerateTrainingSet()
        {
            KP_CPlex_solver solver = new KP_CPlex_solver();
            List<KP_ProblemSolution> training = new List<KP_ProblemSolution>();
            foreach (int seed in new int[] { 0,1,2 })
            {
                Random r = new Random(seed);
                foreach (double correlation in new double[] { 0, .5, .9 })
                    foreach (int n in new int[] { 4, 5 })
                        foreach (double C in new double[] { 10, 100, 1000 })
                            foreach (double fillLevel in new double[] { -0.4, -0.2, 0 })
                            {
                                bool discard = false;
                                //Console.WriteLine("cor=" + correlation + "; " + "n="+n + "; " + "C=" + C +"; fill=" + fillLevel);
                                double avgWeight = C / (n + 0.0);
                                double[] w = new double[n];
                                double[] p = new double[n];
                                for (int i = 0; i < n; i++)
                                {
                                    w[i] = avgWeight + (r.NextDouble() - fillLevel) * avgWeight / 2.0;
                                    p[i] = w[i] + (r.NextDouble()) * avgWeight / 2.0 * (1.0 - correlation);
                                    if (w[i] < 0 || p[i] < 0)
                                    {
                                        Console.WriteLine("Instance is not valid");
                                        Console.ReadLine();
                                    }
                                }
                                if (discard)
                                    continue;
                                KP_ProblemInstance inst = new KP_ProblemInstance(w, p, C, seed + "_" + correlation + "_" + n + "_" + C);
                                KP_ProblemSolution sol = solver.Solve(inst);
                                //Console.WriteLine("\n==================\n" + inst);
                                //Console.WriteLine(sol );
                                //KP_SimpleGreedyRuleSolver greedy = new KP_SimpleGreedyRuleSolver();
                                //Console.WriteLine(greedy.Solve(inst));
                                //Console.ReadLine();
                                training.Add(sol);
                                if (training.Count > 2)
                                    return training;
                            }
            }

            return training;

        }

        public static void KPMain()
        {
            string testDir = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\instances";
            string resultFile = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\results.txt";

            List<KP_ProblemSolution> trainingSet = GenerateTrainingSet();

            double lambda = 0.001;
            int maxSeconds = 60;
            bool expandAttributes = true;
            int maxAttributes = 6;

            ExperimentsFW<KP_ProblemSolution, KP_ProblemInstance, KP_Action> fw = new ExperimentsFW<KP_ProblemSolution, KP_ProblemInstance, KP_Action>();
            fw.Solver = new KP_SimpleGreedyRuleSolver();
            fw.RunExperiments(lambda, trainingSet, testDir, resultFile, new KP_InstanceReader(), maxSeconds, expandAttributes, maxAttributes);
        }
    }
}
