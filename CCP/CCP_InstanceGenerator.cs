using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCP
{
    public class CPP_InstanceGenerator
    {
        private Random _rand = new Random(0);
        private double _minW = 1;
        private double _maxW = 3;
        private double _marginCapacity = 1.2; // 20% more
        private double _spreadLU = .2; // Example: if _spreadLU = 0.2, then L = exp capacity * 0.8 and U = exp capacity * 1.2
        private double _stdDevC = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="CPP_InstanceGenerator"/> class.  w and c are generated 
        /// using a unifrom distribution.  
        /// </summary>
        /// <param name="minW">The minimum weight.</param>
        /// <param name="maxW">The maximum weight.</param>
        /// <param name="marginCapacity">The expected capacity of each cluster. For instance, 1.2 means that the mid-point of L and U will be 20% higher than the expected capacity (sum weights / clusters)</param>
        /// <param name="spreadLU">The spread between L and U. Example: if _spreadLU = 0.2, then L = exp capacity * 0.8 and U = exp capacity * 1.2</param>
        /// <param name="stdDevC">The standard deviation for c. The values of c will be uniformly distributed from 0 to b, where b = Sqrt(12) * stdC</param>
        public CPP_InstanceGenerator(double minW, double maxW, double marginCapacity,
            double spreadLU, double stdDevC)
        {
            _minW = minW;
            _maxW = maxW;
            _marginCapacity = marginCapacity;
            _spreadLU = spreadLU;
            _stdDevC = stdDevC;
        }
        public CCP_ProblemInstance GenerateInstance(int n, int p, int seed)
        {
            _rand = new Random(seed);
            // every object's weight is uniform between 1 and 3
            double[] weights = new double[n];
            double sumW = 0;
            for (int i = 0; i < n; i++)
            {
                weights[i] = _minW + _rand.NextDouble() * _maxW;
                sumW += weights[i];
            }

            double targetCapacity = sumW * _marginCapacity;
            // generate L and U
            double expCapacity = targetCapacity / (p + 0.0);
            double L = expCapacity * (1 - _spreadLU);
            double U = expCapacity * (1 + _spreadLU);

            // generate c using a uniform distribution from 0 to x, where x is chosen so that the std deviation is the one established above
            double a = 0;
            double b = Math.Sqrt(12) * _stdDevC;
            double[,] c = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                {
                    c[i, j] = _rand.NextDouble() * b;
                    c[j, i] = c[i, j];
                }

            return new CCP_ProblemInstance(n, p, c, weights, L, U, "CPP_" + n + "_" + p + "_" + seed);
        }
    }
}
