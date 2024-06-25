using System.Collections.Generic;
using System.IO;

namespace _UNIX_
{
    public partial class SHELL
    {
        static readonly List<string> history = new(35) { string.Empty };
        public int hist_i;

        //----------------------------------------------------------------------------------------------------------

        protected void AddToHistory(in string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;
            AddToHistory(history, input);
        }

        public static void ReadHistory()
        {
            lock (history)
            {
                history.Clear();

                string path = Path.Combine(Util.HOME_DIR.FullName, nameof(SHELL) + ".history" + JSon.txt);
                if (!File.Exists(path))
                    return;

                history.AddRange(File.ReadAllLines(path));
            }
        }

        public static void WriteHistory()
        {
            lock (history)
            {
                if (history.Count == 0)
                    return;

                string path = Path.Combine(Util.HOME_DIR.FullName, nameof(SHELL) + ".history" + JSon.txt);
                using StreamWriter sw = File.CreateText(path);

                for (int i = 0; i < history.Count; i++)
                    sw.WriteLine(history[i]);
            }
        }
    }
}