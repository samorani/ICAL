using DataSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class FunctionGreedyRule<S, I, O> : GreedyRule<S, I, O> where S : ProblemSolution<S, I, O>, new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        /// <summary>
        /// Gets the beta, that is, the function coefficients used to evaluate the goodness of the rule
        /// </summary>
        /// <value>The beta.</value>
        public SortedList<string,double> Beta { get; private set; }
        AbstractTableModifier _tableModifier;

        public FunctionGreedyRule(SortedList<string, double> beta, AbstractTableModifier modifier = null)
        {
            Beta = beta;
            _tableModifier = modifier;
        }

        /// <summary>
        /// Evaluates the fit of the greedy rule of choosing each action from a certain current solution. It returns the 
        /// sumproduct of the attribute values and the beta coefficients.
        /// </summary>
        /// <param name="currentSolution">The current solution.</param>
        /// <returns>System.Double.</returns>
        protected override List<KeyValuePair<O, double>> GetActionValues(S currentSolution)
        {
            List<KeyValuePair<O, double>> actions = new List<KeyValuePair<O, double>>();

            // make table
            DateTime now = DateTime.Now;
            Table t = new Table(new List<Column>());
            bool firstIteration = true;
            foreach (O action in currentSolution.GetFeasibleActions())
            {
                Row attributes = currentSolution.GetAttributesOfAction(action);
                actions.Add(new KeyValuePair<O, double>(action, Double.NaN));
                // add columns to table
                if (firstIteration)
                {
                    firstIteration = false;
                    foreach (Column c in attributes.AttributeValues.Keys)
                        t.AddColumn(c);
                }
                t.AddRow(attributes);
            }
            //Console.Write("Create table: " + Math.Round((DateTime.Now - now).TotalMilliseconds, 0) + "\t");

            if (_tableModifier != null)
            {
                lock (this)
                {
                    t = _tableModifier.Modify(t);
                }
            }

            now = DateTime.Now;
            // for each row, compute its value
            for (int i = 0; i < t.Rows.Count; i++)
            {
                double sum = 0;
                Row attributes = t.Rows[i];
                foreach (Column col in t.Columns)
                    sum += attributes[col.Name] * Beta[col.Name];
                if (sum > 0)
                    Console.Write("");
                actions[i] = new KeyValuePair<O, double>(actions[i].Key, sum);
            }
            //Console.Write("Compute value: " + Math.Round((DateTime.Now - now).TotalMilliseconds, 0) + "\n");

            return actions;
        }

        public override string ToString()
        {
            string s = "";
            foreach (string att in Beta.Keys)
                s += att + ": " + Math.Round(Beta[att],4) + "\n";
            s += "\n=============\nNON-ZERO COEFFICIENTS:\n";
            foreach (string att in Beta.Keys)
                if (Beta[att] != 0)
                    s += att + ": " + Math.Round(Beta[att], 10) + "\n";
            return s;
        }
    }
}
