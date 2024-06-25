using System;
using _UTIL_;

namespace _UNIX_
{
    public enum SIGT : byte
    {
        _none_,
        KILL,
        FIRST_TAB,
        SECOND_TAB,
        HIST_UP,
        HIST_DOWN,
        ALT_UP,
        ALT_DOWN,
        ALT_LEFT,
        ALT_RIGHT,
        CHECK,
        BUILD,
        EXEC,
        LOG,
        SUBLOG,
        WARN,
        ERR,
        EXCP,
        _last_,
    }

    public class SIG_EXCP : BOSON_EXCP
    {
        public SIG_EXCP(in string message) : base(message)
        {
        }
    }

    public partial class SIGNAL
    {
        public static readonly SIGNAL
            SIG_EXEC = new(null, SIGT.EXEC),
            SIG_VOID = new(null, SIGT._none_);

        static byte _id;
        readonly byte id = ++_id == 0 ? ++_id : _id;
        public readonly BOSON emitter;
        public readonly TERMINAL_base terminal;
        public readonly string line;
        public readonly object data;
        public SIGT sigtype;
        public int caret, iread_start, iread;
        public string Remaining => iread < line.Length ? line[iread..] : string.Empty;
        public override string ToString() => $"sig[{id}].{nameof(SIGNAL)}.{sigtype}(\"{line[..iread_start]}`{line[iread_start..iread]}`{line[iread..]}\", emitter:{emitter}, terminal:{terminal})";

        //----------------------------------------------------------------------------------------------------------

        public SIGNAL SubSignal(in string line = null, in object data = null) => new(emitter, sigtype, line, 0, terminal, data);
        public SIGNAL(in BOSON emitter, in SIGT sigtype, in string line = null, in int caret = 0, in TERMINAL_base terminal = null, in object data = null)
        {
            this.emitter = emitter;
            this.line = line ?? string.Empty;
            this.sigtype = sigtype;
            this.caret = caret;
            this.terminal = terminal;
            this.data = data;
        }

        //----------------------------------------------------------------------------------------------------------

        public void Reset(in SIGT sigtype, in ushort iread = 0)
        {
            this.sigtype = sigtype;
            this.iread = iread;
            iread_start = 0;
            isOnTab = false;
            completed = false;
            completion = null;
        }

        public void Consume()
        {
            sigtype = SIGT._none_;
            completion = null;
            completed = false;
            iread_start = iread = (ushort)(line?.Length ?? 0);
            isOnTab = false;
        }

        public bool HasNext()
        {
            while (iread < line.Length)
                if (line[iread] != ' ')
                    return true;
                else
                    ++iread;
            return false;
        }
    }
}