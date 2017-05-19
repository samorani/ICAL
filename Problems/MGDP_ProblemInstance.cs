using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class MGDP_ProblemInstance :
        ProblemInstance<MGDP_ProblemSolution, MGDP_ProblemInstance, MGDP_Action>
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

        public int U { get; private set; }
        public int L { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MGDP_ProblemInstance"/> class.
        /// </summary>
        /// <param name="n">The number of elements.</param>
        /// <param name="p">The number of clusters</param>
        /// <param name="c">A n-by-n matrix with the reward obtained for assigning two objects to the same cluter.</param>
        public MGDP_ProblemInstance(int n, int p, double[,] c, int L, int U)
        {
            this.n = n;
            this.p = p;
            this.c = c;
            this.L = L;
            this.U = U;
        }

        public override MGDP_ProblemSolution BuildEmptySolution()
        {
            return new MGDP_ProblemSolution(p, n);
        }

        public override int CompareTo(ProblemInstance<MGDP_ProblemSolution, MGDP_ProblemInstance, MGDP_Action> other)
        {
            throw new NotImplementedException();
        }
    }
}
