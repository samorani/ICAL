using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP
{
    public class KP_SimpleGreedyRuleSolver : ISolver<KP_ProblemSolution, KP_ProblemInstance, KP_Action>
    {
        public KP_ProblemSolution Solve(KP_ProblemInstance instance)
        {
            int n = instance.N;
            double c = instance.C;
            double[] w = instance.W;
            double[] p = instance.P;
            HashSet<int> available = new HashSet<int>();
            for (int i = 0; i < n; i++)
                available.Add(i);

            bool[] x = new bool[n];
            while (true)
            {
                // find the best p/w
                double maxPW = Double.MinValue;
                int bestIndex = -1;
                for (int i = 0; i < n; i++)
                    foreach (int obj in available)
                    {
                        double val = p[obj] / w[obj];
                        if (w[obj] <= c && val > maxPW)
                        {
                            maxPW = val;
                            bestIndex = obj;
                        }
                    }
                // finished?
                if (bestIndex == -1)
                    return new KP_ProblemSolution(instance,x);

                // add it to solution
                x[bestIndex] = true;
                available.Remove(bestIndex);
                c -= w[bestIndex];
            }
        }
    }
}
