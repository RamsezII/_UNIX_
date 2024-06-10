using _ARK_;
using System;
using System.Collections.Generic;

namespace _CORE_
{
    public class CORE_SHELL : SHELL
    {
        protected new enum CMDs : byte
        {
            _none_ = SHELL.CMDs._last_,
            RenderPipeline,
            _last_,
            rp,
        }

        //----------------------------------------------------------------------------------------------------------

        public CORE_SHELL(in SIGNAL signal) : base(signal)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        protected override IEnumerable<string> ECmds()
        {
            foreach (var cmd in base.ECmds())
                yield return cmd;
            foreach (var cmd in Util.EEnumNames(CMDs._none_ + 1, CMDs._last_))
                yield return cmd;
        }

        protected override BOSON OnTryBoson(in string arg0, in SIGNAL signal)
        {
            if (Enum.TryParse(arg0, true, out CMDs cmd))
                return cmd switch
                {
                    CMDs.RenderPipeline or CMDs.rp => new RPGOD.CMD_RP(signal),
                    _ => base.OnTryBoson(arg0, signal),
                };
            return base.OnTryBoson(arg0, signal);
        }
    }
}