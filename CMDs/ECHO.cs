using System;
using UnityEngine;

namespace _ARK_
{
    public class ECHO : BOSON
    {
        readonly string msg;
        readonly SIGT opt;

        //----------------------------------------------------------------------------------------------------------

        public ECHO(in SIGNAL signal) : base(signal)
        {
            if (signal.TryReadOption('t', "type"))
            {
                if (!signal.TryRead(out string arg) && signal.sigtype >= SIGT.BUILD)
                    throw new SIG_EXCP("Missing log type");

                var matches = Util.EEnumNames(SIGT.EXEC, SIGT._last_).Matches2(arg);
                if (signal.IsOnTab())
                {
                    if (signal.sigtype == SIGT.FIRST_TAB)
                    {
                        if (Util.TryComplete(arg, matches, out string compl))
                            signal.Completion(compl);
                    }
                    else if (signal.sigtype == SIGT.SECOND_TAB)
                        Debug.Log(matches.Join("  "));
                }
                else if (Enum.TryParse(arg, true, out SIGT type) && type >= SIGT.EXEC)
                    opt = type;
                else
                    throw new SIG_EXCP($"Invalid log type: {arg}");
            }
            else
                opt = SIGT.EXEC;
            msg = signal.ReadUntil();
        }

        //----------------------------------------------------------------------------------------------------------

        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);
            b_out.OnSignal(new SIGNAL(signal.emitter, opt, msg));
            return 0;
        }
    }
}