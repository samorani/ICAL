using Core;

namespace Problems
{
    public class KP_Option : Option<KP_ProblemSolution, KP_ProblemInstance, KP_Option>
    {
        public int Index { get; private set; }
        public KP_Option(int index)
        {
            Index = index;
        }
    }
}