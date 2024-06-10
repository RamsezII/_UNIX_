using _ARK_;
using System.Text.RegularExpressions;

namespace _TEST_
{
    public class GREP : BOSON
    {
        readonly Regex regex;

        //----------------------------------------------------------------------------------------------------------

        public GREP(in SIGNAL signal) : base(signal)
        {
            if (signal.TryRead(out string arg0))
                regex = new(arg0);
            else
                throw new SIG_EXCP("Usage: grep <regex>");
        }

        //----------------------------------------------------------------------------------------------------------

        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);
            if (regex.IsMatch(signal.Remaining))
                b_out.OnSignal(signal);
            return 0;
        }
    }
}