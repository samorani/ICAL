using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Problems
{
    public class KP_ProblemSolution : 
        ProblemSolution<KP_ProblemSolution, KP_ProblemInstance, KP_Action> 
    {
        /// <summary>
        /// The object selection array. X[i] = true if i is selected.
        /// </summary>
        /// <value>The current selection.</value>
        public bool [] X { get; private set; }



        public double RemainingCapacity { get; set; }

        public override double Value { get; protected set; }

        /// <summary>
        /// Needed to compile without errors.
        /// </summary>
        public KP_ProblemSolution()
        { }

        /// <summary>
        /// Initializes an empty instance of the <see cref="KP_ProblemSolution"/> class.
        /// </summary>
        /// <param name="inst">The inst.</param>
        public KP_ProblemSolution(KP_ProblemInstance inst) : this()
        {
            this.X = new bool[inst.N];
            this.Value = 0;
            this.Instance = inst;
            this.RemainingCapacity = inst.C;
        }

        public KP_ProblemSolution(KP_ProblemInstance inst, bool [] x) : this(inst)
        {
            for (int i=0;i<x.Length;i++)
            {
                X[i] = x[i];
                if (X[i])
                {
                    Value += Instance.P[i];
                    RemainingCapacity -= Instance.W[i];
                }
            }
        }


        /// <summary>
        /// Gets the attributes of action of selecting the i-th object. The weight and profit of i, the remaining capacity, w/p and p/w
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>System.Double[].</returns>
        public override SortedList<string,double> GetAttributesOfAction(KP_Action o)
        {
            int i = o.Index;
            SortedList<string, double> attributes = new SortedList<string, double>();
            attributes.Add("profit", Instance.P[i]);
            attributes.Add("weight", Instance.W[i]);
            attributes.Add("p/w", Instance.P[i] / Instance.W[i]);
            attributes.Add("w/p", Instance.W[i] / Instance.P[i]);
            attributes.Add("new remaining capacity", RemainingCapacity - Instance.W[i]);
            //attributes.Add("new value", Value + Instance.P[i]);
            return attributes;
        }

        /// <summary>
        /// Gets the feasible actions of the current solution for the current problem instance. It returns the actions of selecting the objects that fit.
        /// </summary>
        /// <returns>IEnumerable&lt;Action&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerable<KP_Action> GetFeasibleActions()
        {
            for (int i = 0; i < Instance.N; i++)
                if (Instance.W[i] <= RemainingCapacity && !this.X[i])
                    yield return new KP_Action(i);
        }

        /// <summary>
        /// Chooses the action and updates this solution. Reduces the capacity by the weight and increases the value by the profit
        /// </summary>
        /// <param name="o">The o.</param>
        public override KP_ProblemSolution ChooseAction(KP_Action o)
        {
            int i = o.Index;
            KP_ProblemSolution newSol = new KP_ProblemSolution(this.Instance);
            X.CopyTo(newSol.X,0);
            newSol.X[i] = true;
            newSol.Value = Value + Instance.P[i];
            newSol.RemainingCapacity = RemainingCapacity - Instance.W[i];
            return newSol;
        }

        /// <summary>
        /// Checks whether the instances are the same and all the objects selected are the same
        /// </summary>
        /// <param name="other">The other solution.</param>
        /// <returns><c>true</c> if the current solution is the same as the other solution; otherwise, <c>false</c>.</returns>
        protected override bool IsSameAs(KP_ProblemSolution other)
        {
            if (other.Instance != this.Instance)
                return false;
            for (int i = 0; i < X.Length; i++)
                if (X[i] != other.X[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Returns true if target solution contains the object corresponding to the action
        /// </summary>
        /// <param name="action">The action in consideration.</param>
        /// <param name="targetSolution">The target solution.</param>
        /// <returns><c>true</c> if the action brings this solution closer to the targetSolution, <c>false</c> otherwise.</returns>
        protected override bool MayLeadToTargetSolution(KP_Action action, KP_ProblemSolution targetSolution)
        {
            int index = action.Index;
            if (targetSolution.X[index])
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            string s = "[";
            foreach (bool b in X)
                s += b ? "1," : "0,";
            return s + "]";
        }
    }
}
