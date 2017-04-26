using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// A sequence of options taken when building a solution.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public class Sequence<S, I, O> : IComparable<Sequence<S, I, O>> where S : ProblemSolution<S, I, O>,  
        new() where I : ProblemInstance<S, I, O> where O : Option<S, I, O>
    {
        public List<S> Solutions { get; private set; }
        public List<O> Options { get; private set; }
        public Sequence()
        {
            Solutions = new List<S>();
            Options = new List<O>();
        }

        public Sequence(Sequence<S, I, O> other) : this()
        {
            foreach (S s in other.Solutions)
                this.Solutions.Add(s);
            foreach (O o in other.Options)
                this.Options.Add(o);
        }

        /// <summary>
        /// Adds the specified solution and option. (At step t, we have a solution and chose an option)
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="option">The option.</param>
        public void Add(S solution, O option)
        {
            Options.Add(option);
            Solutions.Add(solution);
        }

        public int Count { get { return Options.Count; } }

        public int CompareTo(Sequence<S, I, O> other)
        {
            for (int i = 0; i < Options.Count; i++)
                if (other.Options.Count <= i)
                    return 1;
                else
                {
                    int comp = other.Options[i].ToString().CompareTo(this.Options[i].ToString());
                    if (comp == 1)
                        return 1;
                    else if (comp == -1)
                        return -1;
                }
            return 0;
        }

        //public override  
    }
}
