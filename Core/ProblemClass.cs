using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// A class of problem is a type of problem, such as KP-01, LongestPathProblem, etc
    /// </summary>
    public class ProblemClass
    {
        public string Name { get; private set; }
        public ProblemClass(string name)
        {
            Name = name;
        }
    }
}
