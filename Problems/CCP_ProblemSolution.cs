using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class CCP_ProblemSolution :
        ProblemSolution<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>
    {
        /// <summary>
        /// Gets the cluster number of an object.
        /// </summary>
        /// <value>The cluster number of an object.</value>
        public int[] X { get; set; }

        /// <summary>
        /// Gets the current total weight  in each cluster.
        /// </summary>
        /// <value>The current total weight  in each cluster.</value>
        public double[] CurWeights { get; set; }

        /// <summary>
        /// Gets or sets the objects in a cluster.
        /// </summary>
        /// <value>The objects in a cluster.</value>
        public List<HashSet<int>> ObjectsInCluster { get; set; }

        public override double Value { get; protected set; }


        public CCP_ProblemSolution()
        { }

        /// <summary>
        /// Initializes a new instance with n objects unassigned to clusters
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="n">The n.</param>
        public CCP_ProblemSolution(CCP_ProblemInstance inst)
        {
            Instance = inst;
            X = new int[inst.n];
            for (int i = 0; i < inst.n; i++)
                X[i] = -1;
            ObjectsInCluster = new List<HashSet<int>>();
            for (int i = 0; i < inst.p; i++)
                ObjectsInCluster.Add(new HashSet<int>());
            CurWeights = new double[inst.p];
        }

        /// <summary>
        /// Initializes a new instance with the assignment x (x[i] is the cluster of object i)
        /// </summary>
        public CCP_ProblemSolution(CCP_ProblemInstance inst, int [] x) : this(inst)
        {
            x.CopyTo(X, 0);
            for (int i = 0; i < x.Length; i++)
                ObjectsInCluster[x[i]].Add(i);
            // set the weights and value
            for (int k = 0; k < inst.p; k++)
            {
                foreach (int i in ObjectsInCluster[k])
                {
                    CurWeights[k] += inst.w[i];
                    foreach (int j in ObjectsInCluster[k])
                        if (i > j)
                            Value += inst.c[i, j];
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCP_ProblemSolution"/> class. It deep-copies all of the assignments
        /// </summary>
        /// <param name="other">The other solution.</param>
        public CCP_ProblemSolution(CCP_ProblemSolution other) : this(other.Instance)
        {
            Instance = other.Instance;
            Value = other.Value;
            other.CurWeights.CopyTo(CurWeights,0);
            for (int i = 0; i < Instance.n; i++)
                X[i] = other.X[i];
            for (int k = 0; k < Instance.p; k++)
                ObjectsInCluster[k] = new HashSet<int>(other.ObjectsInCluster[k]);
        }

        public override CCP_ProblemSolution ChooseAction(CCP_Action o)
        {
            CCP_ProblemSolution toRet = new CCP_ProblemSolution(this);
            toRet.ObjectsInCluster[o.Cluster].Add(o.Object);
            toRet.X[o.Object] = o.Cluster;
            // update value
            foreach (int i in toRet.ObjectsInCluster[o.Cluster])
                toRet.Value += toRet.Instance.c[i, o.Object];
            // update weight
            toRet.CurWeights[o.Cluster] += Instance.w[o.Object];
            return toRet;
        }

        public override SortedList<string, double> GetAttributesOfAction(CCP_Action o)
        {
            SortedList<string, double> att = new SortedList<string, double>();
            att.Add("curval", this.Value);
            double newVal = this.Value;
            // violation
            bool violation = CurWeights[o.Cluster] + o.Object > Instance.U;
            att.Add("violation", violation ? 1.0 : 0.0);
            foreach (int i in ObjectsInCluster[o.Cluster])
                newVal += Instance.c[i, o.Object];
            att.Add("newval", newVal);
            return att;
        }

        /// <summary>
        /// The list of obj-clu (i,k) that do not overcome U.
        /// </summary>
        /// <returns>IEnumerable&lt;Action&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerable<CCP_Action> GetFeasibleActions()
        {
            for (int i=0;i<Instance.n;i++)
                if (X[i] == -1) // unassigned
                {
                    for (int k = 0; k < Instance.p; k++)
                        if (CurWeights[k] + Instance.w[i] <= Instance.U)
                            // there is room
                            yield return new CCP_Action(k, i);
                }
        }

        /// <summary>
        /// For each object, finds the cluster in this solution and in the other solution, and then checks whether the 
        /// objects are the same
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>System.Boolean.</returns>
        protected override bool IsSameAs(CCP_ProblemSolution other)
        {
            for (int i = 0; i < Instance.n; i++)
            {
                int k1 = X[i];
                int k2 = other.X[i];
                if (k2 == -1 && k1 == -1)
                    continue; // the object is unassigned in both
                if (k1 == -1 && k2 >= 0)
                    return false; // the object is not assigned in this solution yet 
                // I need to check whether the two clusters contain the same objects
                if (ObjectsInCluster[k1].Count != other.ObjectsInCluster[k2].Count)
                    return false;
                foreach (int j in ObjectsInCluster[k1])
                    if (!other.ObjectsInCluster[k2].Contains(j))
                        return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the action brings this solution closer to the targetSolution. In this case, 
        /// it returns false if either of the following is true:
        /// 1) the object is placed away from the objects that should be with it in the end.
        /// 2) the object is placed together with objects that should not be with it in the end.
        /// </summary>
        /// <param name="action">The action in consideration.</param>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns><c>true</c> if the action brings this solution closer to the targetSolution, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override bool MayLeadToTargetSolution(CCP_Action a, CCP_ProblemSolution targetSolution)
        {
            // 1. find the objects that should be with this object.
            int kfinal = targetSolution.X[a.Object];
            HashSet<int> targetNeighbors = targetSolution.ObjectsInCluster[kfinal];
            foreach (int targetNeighbor in targetNeighbors)
                if (X[targetNeighbor] >= 0 && X[targetNeighbor] != a.Cluster)
                    return false;

            // 2. find if the object is placed together with foreign objects
            HashSet<int> possibleForeignObjects = ObjectsInCluster[a.Cluster];
            foreach (int possibleForeignObject in possibleForeignObjects)
                if (!targetNeighbors.Contains(possibleForeignObject))
                    return false;

            return true;
        }

        public override string ToString()
        {
            string s = "";
            for (int k =0;k<Instance.p;k++)
            {
                s += "Cluster " + k + ": " + (ObjectsInCluster[k].Count == 0? "empty" : "");
                foreach (int i in ObjectsInCluster[k])
                    s += i + ",";
            }
            return s;
        }
    }
}
