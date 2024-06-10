using _TEST_;
using System;
using System.Collections.Generic;

namespace _ARK_
{
    public partial class SHELL
    {
        protected enum CMDs : byte
        {
            _none_,
            ls,
            grep,
            bs,
            echo,
            adduser,
            wait,
            _last_,
        }

        //----------------------------------------------------------------------------------------------------------

        protected override IEnumerable<string> ECmds() => Util.EEnumNames(CMDs._none_ + 1, CMDs._last_);
        protected override BOSON OnTryBoson(in string arg0, in SIGNAL signal)
        {
            if (Enum.TryParse(arg0, true, out CMDs cmd) && cmd > CMDs._none_ && cmd < CMDs._last_)
                return cmd switch
                {
                    CMDs.ls => new LS(signal),
                    CMDs.grep => new GREP(signal),
                    CMDs.bs => new BS(signal),
                    CMDs.echo => new ECHO(signal),
                    CMDs.adduser => new ADD_USER(signal),
                    CMDs.wait => new WAITER(signal),
                    _ => base.OnTryBoson(arg0, signal),
                };
            return base.OnTryBoson(arg0, signal);
        }
    }
}