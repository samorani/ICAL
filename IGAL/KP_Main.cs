using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Problems;
using System.IO;

namespace IGAL
{
    public class KP_Main<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Core.Action<S, I, O>
    {
        public double lambda = 0.0;
        string _InstancesDirectory = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\instances";
        string _resultFile = @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\KP01\results.txt";


        public void RunExperiments(List<S> training, InstanceReader<S,I,O> instanceReader)
        {
            GreedyRule<S, I, O> rule = Train(training);
            Test(rule, instanceReader);
        }

        private void Test(GreedyRule<S, I, O> rule, InstanceReader<S, I, O> instanceReader)
        {
            DirectoryInfo d = new DirectoryInfo(_InstancesDirectory);
            StreamWriter sw = new StreamWriter(_resultFile);
            sw.WriteLine("INSTANCE\tVALUE\tTIME(MILLISECONDS)");
            sw.Close();
            foreach (FileInfo f in  d.EnumerateFiles())
            {
                I instance = instanceReader.LoadInstanceFromFile(f.FullName);
                Console.Write("Solving " + f.Name + ": ");
                DateTime begin = DateTime.Now;
                GreedySolver<S, I, O> greedySolver = new GreedySolver<S, I, O>();
                S sol = greedySolver.Solve(instance, rule);
                DateTime end = DateTime.Now;
                Console.WriteLine(sol.Value + "\t" + (end - begin).TotalMilliseconds);
                sw = new StreamWriter(_resultFile, true);
                sw.WriteLine(f.Name +"\t"+ sol.Value + "\t" + (end - begin).TotalMilliseconds);
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
