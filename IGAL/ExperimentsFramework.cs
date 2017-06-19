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
        public int _maxSeconds = 5;
        string _resultFile;
        ISolver<S, I, O> _trainingSetSolver;
        IGreedyAlgorithmLearner<S, I, O> _learner;

        public void RunExperiments(string trainingDirectory, string testDirectory, string resultFile,
    InstanceReader<S, I, O> instanceReader, ISolver<S, I, O> trainingSetSolver,
    IGreedyAlgorithmLearner<S,I,O> learner,int maxSeconds)
        {
            _trainingSetSolver = trainingSetSolver;
            _learner = learner;
            _maxSeconds = maxSeconds;

            List<S> trainingSet = new List<S>();
            DirectoryInfo d = new DirectoryInfo(trainingDirectory);
            foreach (FileInfo f in d.GetFiles())
            {
                I inst = instanceReader.LoadInstanceFromFile(f.FullName);
                //try
                //{
                    S sol = trainingSetSolver.Solve(inst);
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
            RunExperiments(trainingSet, testDirectory, resultFile, instanceReader,learner,maxSeconds);
        }

        public void RunExperiments(List<S> trainingSet, string testDirectory, string resultFile,
            InstanceReader<S, I, O> instanceReader,  IGreedyAlgorithmLearner<S, I, O> learner,
            int maxSeconds)
        {
            _learner = learner;
            _resultFile = resultFile;
            _maxSeconds = maxSeconds;

            // train
            GreedyRule<S, I, O> rule = Train(trainingSet);

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
                ISolver<S, I, O> solver = new GreedySolver<S, I, O>(rule, _maxSeconds);
                //S sol = solver.Solve(instance);
                S sol = _trainingSetSolver.Solve(instance);
                DateTime end = DateTime.Now;
                string toWrite = "";
                if (sol != null)
                    toWrite = instance.GetShortName() + "\t" + sol.Value + "\t" + (end - begin).TotalMilliseconds;
                else
                    toWrite = instance.GetShortName() + "\t" + "infeasible" + "\t" + (end - begin).TotalMilliseconds;
                Console.WriteLine(toWrite);
                Console.WriteLine(sol);
                sw = new StreamWriter(_resultFile, true);
                sw.WriteLine(toWrite);
                sw.Close();

            }
        }

        public GreedyRule<S, I, O> Train(List<S> training)
        {
            GreedyRule<S, I, O> rule = _learner.Learn(training);
            Console.WriteLine("\n******** RULE ********");
            Console.WriteLine(rule.ToString());

            return rule;
        }
    }
}
