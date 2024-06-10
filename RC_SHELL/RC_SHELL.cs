using _ARK_;
using _CORE_;
using System;
using System.Collections.Generic;

namespace _RC_
{
    internal class RC_SHELL : CORE_SHELL
    {
        new enum CMDs : byte
        {
            _none_ = CORE_SHELL.CMDs._last_,
            Test,
            _last_,
        }

        //----------------------------------------------------------------------------------------------------------

        public RC_SHELL(in SIGNAL signal) : base(signal)
        {
            prefixe = $"<color=#73CC26>user</color>:<color=#73B2D9>~</color>$";
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
                    CMDs.Test => new TestBoson(signal),
                    _ => base.OnTryBoson(arg0, signal),
                };
            return base.OnTryBoson(arg0, signal);
        }
    }
}