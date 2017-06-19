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
            KP_ProblemInstance i1 = new KP_ProblemInstance(new double[] { 3, 2, 5, 3 }, new double[] { 4, 2, 5, 2 }, 5, "i1");
            KP_ProblemSolution s1 = new KP_ProblemSolution(i1, new bool[] { true, true, false, false });
            KP_ProblemInstance i2 = new KP_ProblemInstance(new double[] { 200, 500, 100, 100, 500, 500, 500 }, new double[] { 400, 800, 100, 100, 500, 400, 200 }, 600, "i2");
            KP_ProblemSolution s2 = new KP_ProblemSolution(i2, new bool[] { false, true, true, false, false, false, false });
            KP_ProblemInstance i3 = new KP_ProblemInstance(new double[] { 1, 2, 3, 5, 5, 1, 3 }, new double[] { 4, 5, 5, 8, 7, 1, 2 }, 4, "i3");
            KP_ProblemSolution s3 = new KP_ProblemSolution(i3, new bool[] { true, true, false, false, false, true, false });
            KP_ProblemInstance i4 = new KP_ProblemInstance(new double[] { 4, 5, 4, 4, 3, 2, 4 }, new double[] { 2, 8, 3, 1, 6, 2, 3 }, 10, "i4");
            KP_ProblemSolution s4 = new KP_ProblemSolution(i4, new bool[] { false, true, false, false, true, true, false });
            KP_ProblemInstance i5 = new KP_ProblemInstance(new double[] { 1, 3, 3, 3, 1, 1, 2 }, new double[] { 4, 6, 6, 4, 1, 1, 1 }, 5, "i5");
            KP_ProblemSolution s5 = new KP_ProblemSolution(i5, new bool[] { true, true, false, false, false, true, false });
            KP_ProblemInstance i6 = new KP_ProblemInstance(new double[] { 4.97551809578609, 3.23645150391967, 3.69435943547207, 3.55179587063616, 4.52838861107504, 2.9138093244874, 0.84694973911746, 4.20813794471918, 0.346253570889781, 0.466378486380901, 4.97355776419221, 0.604530894446334, 3.85676726434271, 3.28458570621893, 0.440989012102276, 2.62428840926317, 0.995941349325443 },
                new double[] { 4.67304406010049, 3.03851055008114, 3.44653508875115, 3.18154561757257, 4.03779012671738, 2.58625666164887, 0.743705257434527, 3.65615162746975, 0.3, 0.4, 4.23895182266155, 0.5, 3.15429659479386, 2.49059473336697, 0.291307735268, 1.64024962040007, 0.293711616358581 }, 25, "i6");
            KP_ProblemSolution s6 = new KP_ProblemSolution(i6, new bool[] { true, true, true, true, true, true, true,
            false,false,true,false,true,false,false,false,false,false});
            List<KP_ProblemSolution> training = new List<KP_ProblemSolution>();
            training.Add(s1);
            training.Add(s2);
            training.Add(s3);
            training.Add(s4);
            training.Add(s5);
            //training.Add(s6);
            return training;

        }

        public static void KPMain()
        {
            string testDir = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\instances";
            string resultFile = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\results.txt";

            List<KP_ProblemSolution>  trainingSet = GenerateTrainingSet();

            double lambda = 0.0;
            int maxSeconds = 60;
            int maxAttributes = 2;

            IGreedyAlgorithmLearner<KP_ProblemSolution, KP_ProblemInstance, KP_Action> greedyLearner = 
                new CplexGreedyAlgorithmLearner<KP_ProblemSolution, KP_ProblemInstance, KP_Action>(lambda, maxSeconds, maxAttributes);
            ExperimentsFW<KP_ProblemSolution, KP_ProblemInstance, KP_Action> fw = new ExperimentsFW<KP_ProblemSolution, KP_ProblemInstance, KP_Action>();
            fw.RunExperiments(trainingSet, testDir,resultFile, new KP_InstanceReader(), greedyLearner,maxSeconds);
        }
    }
}
