using Core;

namespace Problems
{
    public class KP_Option : Option<KP_ProblemInstance>
    {
        public int Index { get; private set; }
        public KP_Option(int index)
        {
            Index = index;
        }
    }
}