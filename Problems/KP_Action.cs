using System;
using Core;

namespace Problems
{
    public class KP_Action : Core.Action<KP_ProblemSolution, KP_ProblemInstance, KP_Action>
    {
        public int Index { get; private set; }
        public KP_Action(int index)
        {
            Index = index;
        }

        public override string ToString()
        {
            return Index.ToString();
        }

        public override bool IsSameAs(KP_Action other)
        {
            return Index == other.Index;
        }
    }
}