using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Problems;

namespace OtherSolvers
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateTrainingSet();
            //CCP_InstanceReader r = new CCP_InstanceReader();
            //CPP_InstanceSolver solver = new CPP_InstanceSolver();
            //CPP_InstanceGenerator generator = new CPP_InstanceGenerator(0, 1, 3, 1, .2, 3);
            //CCP_ProblemInstance i1 = generator.GenerateInstance(10, 3);
            //i1.WriteToFile("..\\..\\..\\inst.txt");
            //Console.WriteLine("Instance: " + i1);
            //CCP_ProblemSolution s1 = solver.Solve(i1);
            //Console.WriteLine(s1 + ", val = " + s1.Value);

            //CCP_ProblemInstance i2 = r.LoadInstanceFromFile("..\\..\\..\\inst.txt");
            //CCP_ProblemSolution s2 = solver.Solve(i2);
            //Console.WriteLine(s2 + ", val = " + s2.Value);

            //CCP_ProblemInstance inst = r.LoadInstanceFromFile(
            //    @"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\instances\Sparse82_01.txt");
            //Console.WriteLine(inst);
            //CCP_ProblemSolution s1 = solver.Solve(inst);
            //Console.WriteLine(s1 + ", val = " + s1.Value);
        }

        private static void GenerateTrainingSet()
        {
            int seed = 0;
            CPP_InstanceGenerator generator = new CPP_InstanceGenerator(seed, 1, 3, 1, .2, 3);
            int n = 7;
            int p = 3;
            generator.GenerateInstance(n, p).WriteToFile(@"D:\Dropbox\Documents\research\Greedy Algorithm Learner\computational experiments\CCP\training\train_" + n + "_"+ p +"_"+ seed + ".txt");
        }
    }
}
