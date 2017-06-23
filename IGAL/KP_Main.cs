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
            foreach (int seed in new int[] { 0 })
            {
                Random r = new Random(seed);
                foreach (double correlation in new double[] { 0, .5, .9 })
                    foreach (int n in new int[] { 4, 5 })
                        foreach (double C in new double[] { 10, 1000 })
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
                                KP_ProblemInstance inst = new KP_ProblemInstance(w, p, C, seed + "_" + correlation + "_" + "_" + fillLevel + "_" + n + "_" + C);
                                KP_ProblemSolution sol = solver.Solve(inst);
                                //Console.WriteLine("\n==================\n" + inst);
                                //Console.WriteLine(sol);
                                //KP_SimpleGreedyRuleSolver greedy = new KP_SimpleGreedyRuleSolver();
                                //Console.WriteLine(greedy.Solve(inst));
                                //Console.ReadLine();
                                training.Add(sol);
                                //if (training.Count > 2)
                                //    return training;
                            }
            }

            return training;

        }

        public static void KPMain()
        {
            string trainingDir = @"Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\myinstances";
            string testDir = @"Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\instances";
            //List<KP_ProblemSolution> trainingSet = GenerateTrainingSet();
            List<KP_ProblemInstance> trainingSet = LoadMyInstances(trainingDir, new string[] { "weakly" },new int[] { 5 }, 1, 2);
            List<KP_ProblemInstance> testSet = LoadMyInstances(testDir, new string[] { "knapsack_weakly_corr" }, new int[] { 100, 300, 1000,3000 }, 1, 50);
            List<KP_ProblemSolution> trainingSetSolutions = new List<KP_ProblemSolution>();
            KP_CPlex_solver exactSolver = new KP_CPlex_solver();
            foreach (KP_ProblemInstance i in trainingSet)
                trainingSetSolutions.Add(exactSolver.Solve(i));

            double lambda = 0.001;
            int maxSeconds = 120;
            bool expandAttributes = false;
            int maxAttributes = 2;

            ExperimentsFW<KP_ProblemSolution, KP_ProblemInstance, KP_Action> fw = new ExperimentsFW<KP_ProblemSolution, KP_ProblemInstance, KP_Action>();
            //fw.Solver = new KP_SimpleGreedyRuleSolver();
            //fw.Solver = new KP_CPlex_solver();
            string resultFile = GetDrive() + @"Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\";
            resultFile += "KP knapsack_weakly_corr " + maxAttributes + ".txt";
            fw.RunExperiments(lambda, trainingSetSolutions, testSet, resultFile, new KP_InstanceReader(), maxSeconds, expandAttributes, maxAttributes);
        }

        private static string GetDrive()
        {
            string drive = "e:\\";
            if (!Directory.Exists("e:\\"))
                drive = "d:\\";
            return drive;
        }
        /// <summary>
        /// Loads the training set from myinstances.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        /// <returns>List&lt;KP_ProblemSolution&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private static List<KP_ProblemInstance> LoadMyInstances(string dir, string[] types, int[] n, int minSeed, int maxSeed)
        {
            KP_InstanceReader read = new KP_InstanceReader();
            List<KP_ProblemInstance> inst = new List<KP_ProblemInstance>();
            DirectoryInfo di = new DirectoryInfo(GetDrive() + dir);
            foreach (FileInfo f in di.GetFiles())
            {
                string simpleName = f.Name;
                int nPos = simpleName.IndexOf("_n");
                string type = simpleName.Substring(0, nPos);
                int thisn = Int32.Parse(simpleName.Substring(nPos-1).Split(new char[] { '_' })[2]);
                int seed = Int32.Parse(simpleName.Substring(nPos - 1).Split(new char[] { '_' }).Last());
                //Console.WriteLine(simpleName + ": n = " + thisn);
                if (types.Contains(type) && minSeed <= seed && seed <= maxSeed && n.Contains(thisn))
                    inst.Add(read.LoadInstanceFromFile(f.FullName));
            }
            return inst;
        }
    }
}
