using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class MGDP_ProblemSolution :
        ProblemSolution<MGDP_ProblemSolution, MGDP_ProblemInstance, MGDP_Action>
    {
        /// <summary>
        /// Gets the cluster number of an object.
        /// </summary>
        /// <value>The cluster number of an object.</value>
        private int [] _clusterOf { get; set; }

        /// <summary>
        /// Gets or sets the objects in a cluster.
        /// </summary>
        /// <value>The objects in a cluster.</value>
        private List<List<int>> _objectsInCluster { get; set; }

        public override double Value { get; protected set; }


        public MGDP_ProblemSolution()
        { }

        /// <summary>
        /// Initializes a new instance with n objects unassigned to clusters
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="n">The n.</param>
        public MGDP_ProblemSolution(int p, int n)
        {
            _clusterOf = new int[n];
            for (int i = 0; i < n; i++)
                _clusterOf[i] = -1;
            _objectsInCluster = new List<List<int>>();
            for (int i = 0; i < p; i++)
                _objectsInCluster[i] = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MGDP_ProblemSolution"/> class. It deep-copies all of the assignments
        /// </summary>
        /// <param name="other">The other solution.</param>
        public MGDP_ProblemSolution(MGDP_ProblemSolution other) : this(other.Instance.p, other.Instance.n)
        {
            Instance = other.Instance;
            Value = other.Value;
            for (int i = 0; i < Instance.n; i++)
                _clusterOf[i] = other._clusterOf[i];
            for (int k = 0; k < Instance.p; k++)
                _objectsInCluster[k] = new List<int>(other._objectsInCluster[k]);
        }

        public override MGDP_ProblemSolution ChooseAction(MGDP_Action o)
        {
            MGDP_ProblemSolution toRet = new MGDP_ProblemSolution(this);
            toRet._objectsInCluster[o.Cluster].Add(o.Object);
            toRet._clusterOf[o.Object] = o.Cluster;
            foreach (int i in toRet._objectsInCluster[o.Cluster])
                toRet.Value += toRet.Instance.c[i, o.Object];
            return toRet;
        }

        public override SortedList<string, double> GetAttributesOfAction(MGDP_Action o)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The list of obj-clu (i,k) that do not overcome U.
        /// </summary>
        /// <returns>IEnumerable&lt;Action&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerable<MGDP_Action> GetFeasibleActions()
        {
            for (int i=0;i<Instance.n;i++)
                if (_clusterOf[i] == -1) // unassigned
                {
                    for (int k = 0; k < Instance.p; k++)
                        if (_objectsInCluster[k].Count + 1 <= Instance.U)
                            // there is room
                            yield return new MGDP_Action(k, i);
                }
        }

        /// <summary>
        /// For each object, finds the cluster in this solution and in the other solution, and then checks whether the 
        /// objects are the same
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>System.Boolean.</returns>
        protected override bool IsSameAs(MGDP_ProblemSolution other)
        {
            for (int i = 0; i < Instance.n; i++)
            {
                int k1 = _clusterOf[i];
                int k2 = other._clusterOf[i];
                if (k1 == k2 && k1 == -1)
                    continue; // the object is unassigned in both
                // I need to check whether the two clusters contain the same objects
                if (_objectsInCluster[k1].Count != other._objectsInCluster[k2].Count)
                    return false;
                foreach (int j in _objectsInCluster[k1])
                    if (!other._objectsInCluster[k2].Contains(j))
                        return false;
            }
            return true;
        }

        protected override bool MayLeadToTargetSolution(MGDP_Action action, MGDP_ProblemSolution targetSolution)
        {
            throw new NotImplementedException();
        }
    }
}
