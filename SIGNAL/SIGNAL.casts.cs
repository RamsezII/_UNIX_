namespace _UNIX_
{
    public partial class SIGNAL
    {
        public bool TryReadCast<T>(out object output)
        {
            output = default;
            int iread = this.iread;
            if (TryRead(out string arg))
                if (typeof(T) == typeof(byte) && byte.TryParse(arg, out byte _byte))
                    output = _byte;
                else if (typeof(T) == typeof(ushort) && ushort.TryParse(arg, out ushort _ushort))
                    output = _ushort;
                else
                {
                    this.iread = iread;
                    return false;
                }
            else
            {
                this.iread = iread;
                return false;
            }
            return true;
        }

#if UNITY_EDITOR

#endif
    }
}