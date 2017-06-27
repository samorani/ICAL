using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.IO;

namespace CCP
{
    public class CCP_ProblemInstance :
        ProblemInstance<CCP_ProblemSolution, CCP_ProblemInstance, CCP_Action>
    {
        string _name;

        /// <summary>
        /// Gets the number of objects.
        /// </summary>
        /// <value>The nnumber of objects.</value>
        public int n { get; set; }

        /// <summary>
        /// Gets the number of clusters.
        /// </summary>
        /// <value>The nnumber of clusters.</value>
        public int p { get; set; }

        /// <summary>
        /// Gets the reward[i,j].
        /// </summary>
        /// <value>the reward[i,j] accrued to assign i and j to the same cluster.</value>
        public double[,] c { get; set; }
        /// <summary>
        /// Gets the weights.
        /// </summary>
        /// <value>The w.</value>
        public double[] w { get; set; }
        private double _totalReward = 0;
        public double TotalReward
        {
            get
            {
                if (_totalReward == 0)
                    lock (this)
                    {
                        for (int i = 0; i < n; i++)
                            for (int j = 0; j < n; j++)
                                _totalReward += c[i, j];
                    }
                return _totalReward;
            }
        }

        public double U { get; set; }
        public double L { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CCP_ProblemInstance"/> class.
        /// </summary>
        /// <param name="n">The number of elements.</param>
        /// <param name="p">The number of clusters</param>
        /// <param name="w">The weights</param>
        /// <param name="c">A n-by-n matrix with the reward obtained for assigning two objects to the same cluter.</param>
        public CCP_ProblemInstance(int n, int p, double[,] c, double [] w, double L, double U, string name)
        {
            this.n = n;
            this.p = p;
            this.c = c;
            this.L = L;
            this.U = U;
            this.w = w;
            _name = name;
        }

        public CCP_ProblemInstance(string name)
        {
            _name = name;
        }
        public CCP_ProblemInstance()
        {
            _name = "no name";
        }

        public override CCP_ProblemSolution BuildEmptySolution()
        {
            return new CCP_ProblemSolution(this);
        }

        public override string GetShortName()
        {
            return _name;
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

        /// <summary>
        /// Writes to file using the same format as the ds instances
        /// </summary>
        public void WriteToFile(string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(n + " " + p + " ds " + L + " " + U + " W");
            for (int i=0;i<n; i++)
                sw.Write(" " + w[i]);
            sw.WriteLine();
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    sw.WriteLine(i + " " + j + " " + c[i, j]);
            sw.Close();
        }
    }
}
