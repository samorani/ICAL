using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.IO;

namespace KP
{
    /// <summary>
    /// An in instance reader of the KP01
    /// </summary>
    /// <seealso cref="Core.ProblemInstance"/>
    public class KP_InstanceReader : InstanceReader<KP_ProblemSolution, KP_ProblemInstance, KP_Action>
    {
        /// <summary>
        /// Loads an instance in the format as the instances in the paper "Black Box Scatter Search for General Classes of Binary Optimization Problems"
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>KP_ProblemInstance.</returns>
        public override KP_ProblemInstance LoadInstanceFromFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            double capacity = Double.Parse(line.Split(new char[] { ' ' })[2]);
            int n = Int32.Parse(line.Split(new char[] { ' ' })[0]);
            double[] weights = new double[n];
            double[] profits = new double[n];
            for (int i = 0; i < n; i++)
            {
                line = sr.ReadLine();
                profits[i] = Double.Parse(line.Split(new char[] { ' ' })[0]);
                weights[i] = Double.Parse(line.Split(new char[] { ' ' })[1]);
            }
            sr.Close();
            return new KP_ProblemInstance(weights, profits, capacity, new FileInfo(fileName).Name);
        }
    }
}
