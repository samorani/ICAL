using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using DataSupport;

namespace KP
{
    public class KP_ProblemSolution :
        ProblemSolution<KP_ProblemSolution, KP_ProblemInstance, KP_Action>
    {
        Random _rand = new Random(0);
        /// <summary>
        /// The object selection array. X[i] = true if i is selected.
        /// </summary>
        /// <value>The current selection.</value>
        public bool[] X { get; private set; }

        public override bool IsFeasible()
        {
            return true;
        }

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

        public KP_ProblemSolution(KP_ProblemInstance inst, bool[] x) : this(inst)
        {
            for (int i = 0; i < x.Length; i++)
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
        public override Row GetAttributesOfAction(KP_Action o)
        {
            int i = o.Index;

            // simple columns
            List<Column> columns = new List<Column>();
            columns.Add(new Column("p", "$", ColumnType.Numeric));
            columns.Add(new Column("w", "lb", ColumnType.Numeric));
            columns.Add(new Column("newC", "lb", ColumnType.Numeric));
            columns.Add(new Column("objectsThatFit", "#", ColumnType.Numeric));
            //columns.Add(new Column("totprofit", "$", ColumnType.Numeric));
            //columns.Add(new Column("totweight", "lb", ColumnType.Numeric));
            Row attributes = new Row(columns);
            double n = Instance.N;
            double c = RemainingCapacity - Instance.W[i];
            int tot = 0;
            double totProfit = 0;
            double totWeight = 0;
            for (int j = 0; j < X.Length; j++)
            {
                if (!X[j] && Instance.W[j] < c)
                    tot++;
                totProfit += Instance.P[j];
                totWeight += Instance.W[j];
            }
            attributes["p"] = Instance.P[i] / totProfit;
            attributes["w"] = Instance.W[i] / totWeight;
            attributes["newC"] = c / totWeight;
            attributes["objectsThatFit"] = tot / n;
            //attributes["totprofit"] = totProfit;
            //attributes["totweight"] = totWeight;

            // complex columns
            //List<Column> columns = new List<Column>();
            //columns.Add(new Column("p/w", "$/lb", ColumnType.Numeric));
            //columns.Add(new Column("w/p", "lb/$", ColumnType.Numeric));
            //columns.Add(new Column("p/c", "$/lb", ColumnType.Numeric));
            //columns.Add(new Column("c/p", "lb/$", ColumnType.Numeric));
            //columns.Add(new Column("p", "$", ColumnType.Numeric));
            //columns.Add(new Column("w", "", ColumnType.Numeric));
            //columns.Add(new Column("newC", "lb", ColumnType.Numeric));
            //columns.Add(new Column("objectsThatFit", "#", ColumnType.Numeric));
            //double p = Instance.P[i];
            //double w = Instance.W[i];
            //double c = RemainingCapacity - Instance.W[i];
            //Row attributes = new Row(columns);
            //int tot = 0;
            //double totProfit = 0;
            //double totWeight = 0;
            //for (int j = 0; j < X.Length; j++)
            //{
            //    if (!X[j] && Instance.W[j] < c)
            //        tot++;
            //    totProfit += Instance.P[j];
            //    totWeight += Instance.W[j];
            //}
            //attributes["objectsThatFit"] = tot / (Instance.N + 0.0);
            //attributes["p/w"] = (p/ totProfit) / (w / totWeight);
            //attributes["w/p"] = (w / totWeight) / (p / totProfit);
            //attributes["p/c"] = (p / totProfit) / (c / totWeight);
            //attributes["c/p"] = (c/totWeight) / (p / totProfit);
            //attributes["p"] = (p / totProfit);
            //attributes["w"] = (w / totWeight);
            //attributes["newC"] = c / totWeight;


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
            X.CopyTo(newSol.X, 0);
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