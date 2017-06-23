using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.IO;
using System.Collections.Concurrent;

namespace IGAL
{
    public class ExperimentsFW<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Core.Action<S, I, O>
    {
        public double _lambda = 0.0;
        public int _maxSeconds = 5;
        string _resultFile;
        public ISolver<S, I, O> Solver;
        public void RunExperiments(double lambda, string trainingDirectory, string testDirectory, string resultFile,
    InstanceReader<S, I, O> instanceReader, ISolver<S, I, O> solver,
    int maxSeconds, bool expandAttributes, int maxAttributes)
        {
            Solver = solver;
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

            // queue1 contains the lists of instances
            var queue1 = new BlockingCollection<I>();
            foreach (I instance in testSet)
                queue1.Add(instance);
            queue1.CompleteAdding();

            // here, all the threads will deposit the output
            var queue2 = new BlockingCollection<Tuple<I, S, double, double>>(100000);

            // producers get an instance from queue1, solve it, and add them to queue2
            var producers = Enumerable.Range(1, 10).Select(_ => Task.Factory.StartNew(() =>
            {
                foreach (I inst in queue1.GetConsumingEnumerable())
                {
                    DateTime begin = DateTime.Now;

                    ISolver<S, I, O> solver = Solver == null ? new GreedySolver<S, I, O>(rule, _maxSeconds) : Solver;
                    S sol = solver.Solve(inst);
                    DateTime end = DateTime.Now;
                    double millis = (end - begin).TotalMilliseconds;
                    queue2.Add(new Tuple<I, S, double, double>(inst, sol, sol.Value, millis));
                }
            })).ToArray();

            // a single consumer gets the solutions from queue2 and adds them to the file
            var consumers = Enumerable.Range(1, 1).Select(_ => Task.Factory.StartNew(() =>
            {
                // the consumer gets the result and stores it in the similarity matrix
                foreach (Tuple<I, S, double, double> t in queue2.GetConsumingEnumerable())
                {
                    string toWrite = "";
                    S sol = t.Item2;
                    double value = t.Item3;
                    double millis = t.Item4;
                    if (sol != null)
                        toWrite = t.Item1.GetShortName() + "\t" + sol.Value + "\t" + millis;
                    else
                        toWrite = t.Item1.GetShortName() + "\t" + "infeasible" + "\t" + millis;
                    Console.WriteLine(toWrite);
                    sw = new StreamWriter(_resultFile, true);
                    sw.WriteLine(toWrite);
                    sw.Close();
                }
            })).ToArray();

            Task.WaitAll(producers);
            queue2.CompleteAdding();
            Task.WaitAll(consumers);



            //foreach (I instance in testSet)
            //{
            //    Console.Write("Solving " + instance.GetShortName() + ": ");
            //    DateTime begin = DateTime.Now;
                
            //    ISolver<S, I, O> solver = Solver == null? new GreedySolver<S, I, O>(rule, _maxSeconds) : Solver;
            //    S sol = solver.Solve(instance);
            //    DateTime end = DateTime.Now;
            //    string toWrite = "";
            //    if (sol != null)
            //        toWrite = instance.GetShortName() + "\t" + sol.Value + "\t" + (end - begin).TotalMilliseconds;
            //    else
            //        toWrite = instance.GetShortName() + "\t" + "infeasible" + "\t" + (end - begin).TotalMilliseconds;
            //    Console.WriteLine(toWrite);
            //    sw = new StreamWriter(_resultFile, true);
            //    sw.WriteLine(toWrite);
            //    sw.Close();

            //}
        }

        public GreedyRule<S, I, O> Train(List<S> training, bool expandAttributes, int maxAttributes)
        {
            CplexGreedyAlgorithmLearner<S, I, O> learner = new CplexGreedyAlgorithmLearner<S, I, O>(_lambda, _maxSeconds * 10, expandAttributes, maxAttributes);
            GreedyRule<S, I, O> rule = learner.Learn(training);
            Console.WriteLine("\n******** RULE ********");
            Console.WriteLine(rule.ToString());

            return rule;
        }
    }
}
