using System.Collections.Generic;

namespace _ARK_
{
    public partial class BOSON
    {
        protected virtual IEnumerable<string> ECmds()
        {
            yield break;
        }

        protected virtual BOSON OnTryBoson(in string arg0, in SIGNAL signal) => null;
        protected BOSON TryBoson(string arg0, in SIGNAL signal)
        {
            BOSON boson = null;
            if (signal.IsOnTab())
                signal.TryCompleteCmd(ref arg0, ECmds().Matches2(arg0));
            else
                boson = OnTryBoson(arg0, signal);
            return boson;
        }
    }
}