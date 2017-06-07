using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.IO;

namespace IGAL
{
    public class ExperimentsFW<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Core.Action<S, I, O>
    {
        public double lambda = 0.0;
        public int _maxSeconds = 5;
        string _InstancesDirectory = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\instances";
        string _resultFile = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\results.txt";


        public void RunExperiments(string trainingDirectory, string testDirectory, InstanceReader<S, I, O> instanceReader, ISolver<S, I, O> solver)
        {
            List<S> trainingSet = new List<S>();
            DirectoryInfo d = new DirectoryInfo(trainingDirectory);
            foreach (FileInfo f in d.GetFiles())
            {
                I inst = instanceReader.LoadInstanceFromFile(f.FullName);
                S sol = solver.Solve(inst);
                trainingSet.Add(sol);
            }

            // train
            GreedyRule<S, I, O> rule = Train(trainingSet);

            List<I> testSet = new List<I>();
            d = new DirectoryInfo(testDirectory);
            foreach (FileInfo f in d.GetFiles())
            {
                I inst = instanceReader.LoadInstanceFromFile(f.FullName);
                testSet.Add(inst);
            }
            Test(rule, testSet);
        }

        private void Test(GreedyRule<S, I, O> rule, List<I> testSet)
        {
            DirectoryInfo d = new DirectoryInfo(_InstancesDirectory);
            StreamWriter sw = new StreamWriter(_resultFile);
            sw.WriteLine("INSTANCE\tVALUE\tTIME(MILLISECONDS)");
            sw.Close();
            foreach (I instance in testSet)
            {
                Console.Write("Solving " + instance.GetShortName() + ": ");
                DateTime begin = DateTime.Now;
                GreedySolver<S, I, O> greedySolver = new GreedySolver<S, I, O>(rule, _maxSeconds);
                S sol = greedySolver.Solve(instance);
                DateTime end = DateTime.Now;
                string toWrite = "";
                if (sol != null)
                    toWrite = instance.GetShortName() + "\t" + sol.Value + "\t" + (end - begin).TotalMilliseconds;
                else
                    toWrite = instance.GetShortName() + "\t" + "infeasible" + "\t" + (end - begin).TotalMilliseconds;
                Console.WriteLine(toWrite);
                sw = new StreamWriter(_resultFile, true);
                sw.WriteLine(toWrite);
                sw.Close();

            }
        }

        public GreedyRule<S, I, O> Train(List<S> training)
        {
            CplexGreedyAlgorithmLearner<S, I, O> learner = new CplexGreedyAlgorithmLearner<S, I, O>(lambda);
            GreedyRule<S, I, O> rule = learner.Learn(training);
            Console.WriteLine("\n******** RULE ********");
            Console.WriteLine(rule.ToString());

            return rule;
        }
    }
}
