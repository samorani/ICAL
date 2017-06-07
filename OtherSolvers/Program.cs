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
            CPP_InstanceSolver solver = new CPP_InstanceSolver();
            CPP_InstanceGenerator generator = new CPP_InstanceGenerator(0, 1, 3, 1, .2, 3);
            CCP_ProblemInstance i1 = generator.GenerateInstance(6, 3);
            Console.WriteLine("Instance: " + i1);
            CCP_ProblemSolution s1 = solver.Solve(i1);
            Console.WriteLine(s1 + ", val = " + s1.Value);
        }
    }
}
