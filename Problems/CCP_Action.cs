using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class CCP_Action :
        Core.Action<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>
    {
        public int Cluster { get; private set; }
        public int Object { get; private set; }

        /// <summary>
        /// The action of assigning an object to a cluster
        /// </summary>
        /// <param name="cluster">The cluster.</param>
        /// <param name="obj">The object.</param>
        public CCP_Action(int cluster, int obj)
        {
            Cluster = cluster;
            Object = obj;
        }

        public override bool IsSameAs(CCP_Action other)
        {
            return Cluster == other.Cluster && Object == other.Object;
        }

        public override string ToString()
        {
            return Object + "->" + Cluster;
        }
    }
}
