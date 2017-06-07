using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class CCP_ProblemInstance :
        ProblemInstance<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>
    {
        /// <summary>
        /// Gets the number of objects.
        /// </summary>
        /// <value>The nnumber of objects.</value>
        public int n { get; private set; }

        /// <summary>
        /// Gets the number of clusters.
        /// </summary>
        /// <value>The nnumber of clusters.</value>
        public int p { get; private set; }

        /// <summary>
        /// Gets the reward[i,j].
        /// </summary>
        /// <value>the reward[i,j] accrued to assign i and j to the same cluster.</value>
        public double[,] c { get; private set; }
        /// <summary>
        /// Gets the weights.
        /// </summary>
        /// <value>The w.</value>
        public double[] w { get; private set; }

        public double U { get; private set; }
        public double L { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CCP_ProblemInstance"/> class.
        /// </summary>
        /// <param name="n">The number of elements.</param>
        /// <param name="p">The number of clusters</param>
        /// <param name="w">The weights</param>
        /// <param name="c">A n-by-n matrix with the reward obtained for assigning two objects to the same cluter.</param>
        public CCP_ProblemInstance(int n, int p, double[,] c, double [] w, double L, double U)
        {
            this.n = n;
            this.p = p;
            this.c = c;
            this.L = L;
            this.U = U;
            this.w = w;
        }

        public override CCP_ProblemSolution BuildEmptySolution()
        {
            return new CCP_ProblemSolution(this);
        }

        public override string ToString()
        {
            string s = "";
            s += "n=" + n + ",";
            s += "p=" + p + ",";
            s += "L=" + Math.Round(L,2) + ",";
            s += "U=" + Math.Round(U,2) + ",";
            s += "c=[";
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    s += "(" + i + "," + j + ")->" + Math.Round(c[i, j],2) +  (i  == n-2 && j  == n-1 ? "] " : ",");
            s += "w=[";
            for (int i = 0; i < n; i++)
                s += Math.Round(w[i],2) + (i == n - 1 ? "]" : ",");
            return s;
        }
    }
}
