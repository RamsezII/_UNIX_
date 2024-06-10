using System;

namespace _ARK_
{
    public partial class SIGNAL
    {
        public bool TryRead(out string output, in bool skipOptions = true, in bool filterSpecials = true)
        {
            iread_start = iread;
            output = string.Empty;
            while (iread < line.Length)
            {
                if (skipOptions && line[iread] == '-')
                    while (++iread < line.Length && line[iread] != ' ') ;

                char c = line[iread];
                switch (c)
                {
                    case '\'':
                    case '\"':
                        iread_start = ++iread;
                        for (; iread < line.Length; iread++)
                            if (line[iread] == c)
                            {
                                output = line[iread_start..iread++];
                                break;
                            }
                        isOnTab = caret >= iread_start && caret <= iread;
                        return true;

                    case '|':
                    case ';':
                    case '&':
                    case '\n':
                    case '\r':
                        iread_start = iread;
                        if (filterSpecials)
                            return false;
                        else
                        {
                            output = line[iread_start..++iread];
                            return true;
                        }

                    default:
                        if (c != ' ')
                        {
                            iread_start = iread++;
                            while (iread < line.Length)
                                if (line[iread] == ' ')
                                    break;
                                else
                                    ++iread;
                            output = line[iread_start..iread];
                            isOnTab = caret >= iread_start && caret <= iread;
                            return true;
                        }
                        break;
                }
                ++iread;
            }
            iread_start = iread;
            return false;
        }

        public bool TryReadOption(in char sopt, in string lopt)
        {
            int cursor = iread;
            while (TryRead(out string value, false, true) && value.Length > 1)
            {
                iread = cursor;
                if (value[0] == '-')
                {
                    // short option
                    if (value[1] != '-')
                    {
                        value = value[1..];
                        for (int i = 0; i < value.Length; ++i)
                            if (value[i] == sopt)
                                return true;
                    }
                    // long option
                    else
                    {
                        value = value[2..];
                        return value.Equals(lopt, StringComparison.OrdinalIgnoreCase);
                    }
                }
                else
                    break;
            }
            iread = cursor;
            return false;
        }
    }
}