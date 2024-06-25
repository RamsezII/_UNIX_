using _UNIX_;
using System;

namespace _UNIX_
{
    partial class RPGOD
    {
        internal class CMD_RP : BOSON
        {
            enum CMDs : byte
            {
                _none_,
                ShowCurrentRP,
                SwitchToBuiltInRP,
                SwitchToURP,
                _last_,
            }

            readonly CMDs cmd;

            //----------------------------------------------------------------------------------------------------------

            public CMD_RP(in SIGNAL signal) : base(signal)
            {
                if (signal.TryReadCompletable(out string arg0, Util.EEnumNames(CMDs._none_ + 1, CMDs._last_)))
                    if (Enum.TryParse(arg0, true, out CMDs cmd))
                        this.cmd = cmd;
            }

            //----------------------------------------------------------------------------------------------------------

            public override SIG_WAIT OnSignal(in SIGNAL signal)
            {
                base.OnSignal(signal);
                switch (cmd)
                {
                    case CMDs.ShowCurrentRP:
                        b_out.OnSignal(signal.SubSignal(rpgod.LogCurrentRP()));
                        break;

                    case CMDs.SwitchToBuiltInRP:
                        rpgod.SwitchToBuiltInRP();
                        break;

                    case CMDs.SwitchToURP:
                        rpgod.SwitchToURP();
                        break;
                }
                return 0;
            }
        }
    }
}