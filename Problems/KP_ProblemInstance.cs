using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    /// <summary>
    /// An in instance of the KP01
    /// </summary>
    /// <seealso cref="Core.ProblemInstance"/>
    public class KP_ProblemInstance : ProblemInstance<KP_ProblemSolution, KP_ProblemInstance, KP_Option>
    {
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int N { get { return W.Length; } } 

        /// <summary>
        /// Gets the weights
        /// </summary>
        /// <value>The weights</value>
        public double[] W { get; private set; }

        /// <summary>
        /// Gets the profits.
        /// </summary>
        /// <value>The profits.</value>
        public double[] P { get; private set; }

        /// <summary>
        /// Gets the capacity
        /// </summary>
        /// <value>The capacity.</value>
        public double C { get; private set; }
        public KP_ProblemInstance(double [] weights, double [] profits, double capacity)
        {
            W = weights;
            P = profits;
            C = capacity;
        }

        public override KP_ProblemSolution BuildEmptySolution()
        {
            return new KP_ProblemSolution(this);
        }

        public override string ToString()
        {
            string s = "W=[";
            foreach (double d in W)
                s += d + ",";
            s += "], P=[";
            foreach (double d in P)
                s += d + ",";
            s += "], C=" + C;
            return s;
        }
    }
}
