using System.Collections.Generic;

namespace _ARK_
{
    public partial class BOSON
    {
        protected void AddToHistory(in List<string> history, in string input, in int max_a = 20, in int max_b = 35)
        {
            lock (history)
            {
                if (history.Count >= max_b)
                    history.RemoveRange(0, history.Count - max_a);
                history.Remove(input);
                history.Add(input);
            }
        }
    }
}