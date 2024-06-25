using _UNIX_;
using System;
using System.IO;

namespace _TEST_
{
    public class LS : BOSON
    {
        readonly bool rec;

        //----------------------------------------------------------------------------------------------------------

        public LS(in SIGNAL signal) : base(signal)
        {
            rec = signal.TryReadOption('r', "recursive");
        }

        //----------------------------------------------------------------------------------------------------------

        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);
            foreach (string fse in Directory.EnumerateFileSystemEntries(Environment.CurrentDirectory, "*", rec ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                b_out.OnSignal(signal.SubSignal(fse));
            return 0;
        }
    }
}