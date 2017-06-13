using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace CCP
{
    public class CCP_Grasp_Solver : ISolver<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>
    {
        Random _rand;
        CCP_ProblemInstance _instance;
        double _alpha;
        public CCP_Grasp_Solver(int seed, double alpha)
        {
            _rand = new Random(seed);
            _alpha = alpha;
        }
            
        public CCP_ProblemSolution Solve(CCP_ProblemInstance instance)
        {
            _instance = instance;
            List<int> currentlyAvailable = new List<int>();
            for (int i = 0; i < instance.n; i++)
                currentlyAvailable.Add(i);
            CCP_ProblemSolution sol = instance.BuildEmptySolution();

            // seed clusters with one object
            for (int k=0;k<instance.p;k++)
            {
                int toInsert = RemoveRandomObjectOfAtMost(currentlyAvailable, instance.U);
                sol  = sol.ChooseAction(new CCP_Action(k, toInsert));
            }

            // step 2. Assign enough objects to each cluster so as to satisfy L
            for (int k = 0; k < instance.p; k++)
            {
                while (sol.CurWeights[k] < instance.L)
                {
                    // for each object i, the I(i,k)
                    Dictionary<int, double> CL = new Dictionary<int, double>();
                    foreach (int i in currentlyAvailable)
                        CL.Add(i, ComputeI(i, k, sol));
                    double maxInCL = 0;
                    foreach (KeyValuePair<int, double> kv in CL)
                        if (kv.Value > maxInCL)
                            maxInCL = kv.Value;

                    // restrictedCL
                    Dictionary<int, double> RCL = new Dictionary<int, double>();
                    foreach (int i in currentlyAvailable)
                        if (CL[i] > (1 - _alpha) * maxInCL)
                            RCL.Add(i, CL[i]);
                    int toInsert = RemoveRandomObjectOfAtMost(new List<int>(RCL.Keys), instance.U - sol.CurWeights[k]);
                    if (toInsert == -1)
                        return null;
                    sol = sol.ChooseAction(new CCP_Action(k, toInsert));
                    currentlyAvailable.Remove(toInsert);
                }
            }

            // step 3. here the CL contains pairs of i,k that fit
            while (true)
            {
                if (currentlyAvailable.Count == 0)
                    break;
                Dictionary<int[], double> CL_pairs = new Dictionary<int[], double>();
                foreach (int i in currentlyAvailable)
                    for (int k = 0; k < instance.p; k++)
                        if (instance.w[i] <= instance.U - sol.CurWeights[k])
                            CL_pairs.Add(new int[] { i, k }, ComputeI(i, k, sol));
                double maxInCL_pairs = 0;
                foreach (double v in CL_pairs.Values)
                    if (v > maxInCL_pairs)
                        maxInCL_pairs = v;

                if (maxInCL_pairs == 0)
                    Console.Write("");
                foreach (int[] ik in new List<int[]> (CL_pairs.Keys))
                    if (CL_pairs[ik] < (1- _alpha) * maxInCL_pairs)
                        CL_pairs.Remove(ik);
                if (CL_pairs.Count == 0) // infeasible
                    return null;
                int [] toInsert = RemoveRandomObjectOfAtMost(new List<int[]>(CL_pairs.Keys));
                sol = sol.ChooseAction(new CCP_Action(toInsert[1], toInsert[0]));
                currentlyAvailable.Remove(toInsert[0]);
            }
            return sol;
        }

        private double ComputeI(int i, int k, CCP_ProblemSolution sol)
        {
            double ret = 0;
            foreach (int other in sol.ObjectsInCluster[k])
                ret += sol.Instance.c[i, other];
            return ret;
        }

        private T RemoveRandomObjectOfAtMost<T>(List<T> toChooseFrom)
        {
            int randomIndex = -1;
            T i;
            randomIndex = _rand.Next(toChooseFrom.Count);
            i = toChooseFrom[randomIndex];
            toChooseFrom.RemoveAt(randomIndex);
            return i;
        }
        private int RemoveRandomObjectOfAtMost(List<int> toChooseFrom, double weight)
        {
            int randomIndex = -1;
            int i = -1;
            bool found = false;
            foreach (int a in toChooseFrom)
                if (_instance.w[a] <= weight)
                {
                    found= true;
                    break;
                }
            if (!found)
                return -1;
            while (true)
            {
                randomIndex = _rand.Next(toChooseFrom.Count);
                i = toChooseFrom[randomIndex];
                if (_instance.w[i] <= weight)
                    break;
            }
            toChooseFrom.RemoveAt(randomIndex);
            return i;
        }
    }
}
