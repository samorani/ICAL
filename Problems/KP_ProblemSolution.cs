using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class KP_ProblemSolution : ProblemSolution<KP_ProblemInstance, KP_Option>
    {
        /// <summary>
        /// Gets or sets the current selection.
        /// </summary>
        /// <value>The current selection.</value>
        public bool [] X { get; private set; }

        /// <summary>
        /// The current value of the objective function
        /// </summary>
        /// <value>The current value.</value>
        public double CurrentValue { get; private set; }

        public double RemainingCapacity { get; set; }

        public KP_ProblemSolution(KP_ProblemInstance inst) : base(inst)
        {
            X = new bool[inst.N];
            RemainingCapacity = inst.C;
        }

        /// <summary>
        /// Gets the attributes of option of selecting the i-th object. The weight and profit of i, the remaining capacity, w/p and p/w
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>System.Double[].</returns>
        public override SortedList<string,double> GetAttributesOfOption(KP_Option o)
        {
            int i = o.Index;
            SortedList<string, double> attributes = new SortedList<string, double>();
            attributes.Add("profit", Instance.P[i]);
            attributes.Add("weight", Instance.W[i]);
            attributes.Add("p/w", Instance.P[i] / Instance.W[i]);
            attributes.Add("w/p", Instance.W[i] / Instance.P[i]);
            attributes.Add("new remaining capacity", RemainingCapacity - Instance.W[i]);
            attributes.Add("new value", CurrentValue + Instance.P[i]);
            return attributes;
        }

        /// <summary>
        /// Gets the feasible options of the current solution for the current problem instance. It returns the options of selecting the objects that fit.
        /// </summary>
        /// <returns>IEnumerable&lt;Option&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerable<KP_Option> GetFeasibleOptions()
        {
            for (int i = 0; i < Instance.N; i++)
                if (Instance.W[i] <= RemainingCapacity)
                    yield return new KP_Option(i);
        }

        /// <summary>
        /// Chooses the option and updates this solution. Reduces the capacity by the weight and increases the value by the profit
        /// </summary>
        /// <param name="o">The o.</param>
        public override void ChooseOption(KP_Option o)
        {
            int i = o.Index;
            this.X[i] = true;
            this.RemainingCapacity -= Instance.W[i];
            this.CurrentValue += Instance.P[i];
        }

    }
}
