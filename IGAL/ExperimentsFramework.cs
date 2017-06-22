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
        public double _lambda = 0.0;
        public int _maxSeconds = 5;
        string _resultFile;
        ISolver<S, I, O> _solver;
        public void RunExperiments(double lambda, string trainingDirectory, string testDirectory, string resultFile,
    InstanceReader<S, I, O> instanceReader, ISolver<S, I, O> solver,
    int maxSeconds, bool expandAttributes, int maxAttributes)
        {
            _solver = solver;
            List<S> trainingSet = new List<S>();
            DirectoryInfo d = new DirectoryInfo(trainingDirectory);
            foreach (FileInfo f in d.GetFiles())
            {
                I inst = instanceReader.LoadInstanceFromFile(f.FullName);
                //try
                //{
                    S sol = solver.Solve(inst);
                if (sol != null)
                    trainingSet.Add(sol);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("**************");
                //    Console.WriteLine(e.Message);
                //    Console.WriteLine("**************");
                //}
            }
            RunExperiments(lambda, trainingSet, testDirectory, resultFile, instanceReader, maxSeconds, expandAttributes, maxAttributes);
        }

        public void RunExperiments(double lambda, List<S> trainingSet, string testDirectory, string resultFile,
            InstanceReader<S, I, O> instanceReader, int maxSeconds, bool expandAttributes, int maxAttributes)
        {
            _lambda = lambda;
            _maxSeconds = maxSeconds;
            _resultFile = resultFile;

            // train
            GreedyRule<S, I, O> rule = Train(trainingSet, expandAttributes, maxAttributes);

            List<I> testSet = new List<I>();
            DirectoryInfo d = new DirectoryInfo(testDirectory);
            foreach (FileInfo f in d.GetFiles())
            {
                I inst = instanceReader.LoadInstanceFromFile(f.FullName);
                testSet.Add(inst);
            }
            Test(rule, testSet);
        }

        private void Test(GreedyRule<S, I, O> rule, List<I> testSet)
        {
            StreamWriter sw = new StreamWriter(_resultFile);
            sw.WriteLine("INSTANCE\tVALUE\tTIME(MILLISECONDS)");
            sw.Close();
            foreach (I instance in testSet)
            {
                Console.Write("Solving " + instance.GetShortName() + ": ");
                DateTime begin = DateTime.Now;
                //ISolver<S, I, O> greedySolver = _solver;
                ISolver<S, I, O> greedySolver = new GreedySolver<S, I, O>(rule, _maxSeconds);
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

        public GreedyRule<S, I, O> Train(List<S> training, bool expandAttributes, int maxAttributes)
        {
            CplexGreedyAlgorithmLearner<S, I, O> learner = new CplexGreedyAlgorithmLearner<S, I, O>(_lambda, _maxSeconds, expandAttributes, maxAttributes);
            GreedyRule<S, I, O> rule = learner.Learn(training);
            Console.WriteLine("\n******** RULE ********");
            Console.WriteLine(rule.ToString());

            return rule;
        }
    }
}
