using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class MGDP_Action :
        Core.Action<MGDP_ProblemSolution, MGDP_ProblemInstance, MGDP_Action>
    {
        public int Cluster { get; private set; }
        public int Object { get; private set; }

        /// <summary>
        /// The action of assigning an object to a cluster
        /// </summary>
        /// <param name="cluster">The cluster.</param>
        /// <param name="obj">The object.</param>
        public MGDP_Action(int cluster, int obj)
        {
            Cluster = cluster;
            Object = obj;
        }

        public override bool IsSameAs(MGDP_Action other)
        {
            return Cluster == other.Cluster && Object == other.Object;
        }
    }
}
