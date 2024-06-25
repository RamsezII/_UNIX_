using _UTIL_;

namespace _UNIX_
{
    public partial class TERMINAL_base
    {
        internal static readonly ThreadSafe<TERMINAL_base> logfocus = new();
        public virtual void TakeLogFocus() => logfocus.Value = this;

        //----------------------------------------------------------------------------------------------------------

        internal void Log(LOG_CATCHER.LogMsg log)
        {
            if (logfocus.Value == this)
                Log(log.msg);
        }

        protected abstract void Log(in object log);

        public virtual SIG_WAIT OnSignal(in SIGNAL signal)
        {
            string msg = signal.VampireCopy();

            switch (signal.sigtype)
            {
                case SIGT.SUBLOG:
                    msg = msg.ToSubLog();
                    break;
                case SIGT.WARN:
                    msg = msg.SetColor(Colors.yellow);
                    break;
                case SIGT.ERR:
                    msg = msg.SetColor(Colors.orange);
                    break;
                case SIGT.EXCP:
                    if (signal.data is SIG_EXCP)
                        goto case SIGT.WARN;
                    else if (signal.data is string stacktrace)
                        msg = $"{msg.SetColor(Colors.red)}\n{stacktrace.SetColor("#AA0000AA")}";
                    else
                        msg = msg.SetColor(Colors.red);
                    break;
            }

            Log(msg);
            return 0;
        }
    }
}