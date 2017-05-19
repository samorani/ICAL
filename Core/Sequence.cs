using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// A sequence of actions taken when building a solution.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public class Sequence<S, I, O> : IComparable<Sequence<S, I, O>> where S : ProblemSolution<S, I, O>,  
        new() where I : ProblemInstance<S, I, O> where O : Action<S, I, O>
    {
        public List<S> Solutions { get; private set; }
        public List<O> Actions { get; private set; }
        public Sequence()
        {
            Solutions = new List<S>();
            Actions = new List<O>();
        }

        public Sequence(Sequence<S, I, O> other) : this()
        {
            foreach (S s in other.Solutions)
                this.Solutions.Add(s);
            foreach (O o in other.Actions)
                this.Actions.Add(o);
        }

        /// <summary>
        /// Adds the specified solution and action. (At step t, we have a solution and chose an action)
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="action">The action.</param>
        public void Add(S solution, O action)
        {
            Actions.Add(action);
            Solutions.Add(solution);
        }

        public int Count { get { return Actions.Count; } }

        public int CompareTo(Sequence<S, I, O> other)
        {
            for (int i = 0; i < Actions.Count; i++)
                if (other.Actions.Count <= i)
                    return 1;
                else
                {
                    int comp = other.Actions[i].ToString().CompareTo(this.Actions[i].ToString());
                    if (comp == 1)
                        return 1;
                    else if (comp == -1)
                        return -1;
                }
            return 0;
        }

        public override string ToString()
        {
            string s = "[";
            for(int i=0;i<Actions.Count;i++)
                s += Actions[i] + (i+1 == Actions.Count? "]": ",");
            return s;
        }
    }
}
