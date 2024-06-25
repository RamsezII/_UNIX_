namespace _UNIX_
{
    public partial class SIGNAL
    {
        public string VampireCopy()
        {
            if (HasNext())
            {
                string output = Remaining;
                iread_start = iread = (ushort)line.Length;
                for (int i = 0; i < output.Length; i++)
                    if (output[i] != ' ')
                    {
                        output = output[i..];
                        break;
                    }
                for (int i = output.Length - 1; i >= 0; i--)
                    if (output[i] != ' ')
                    {
                        output = output[..++i];
                        break;
                    }
                return output;
            }
            return null;
        }

        public string ReadUntil_OLD()
        {
            if (HasNext())
            {
                int start = iread;
                while (iread < line.Length)
                {
                    char c = line[iread];
                    switch (c)
                    {
                        case '\'':
                        case '\"':
                            for (++iread; iread < line.Length; iread++)
                                if (line[iread] == c)
                                    break;
                            break;

                        case '|':
                        case '>':
                        case '<':
                        case '&':
                            return line[start..iread];

                        case '1':
                        case '2':
                            if (iread < line.Length - 1)
                                switch (line[iread + 1])
                                {
                                    case '>':
                                    case '|':
                                    case '&':
                                        return line[start..iread];
                                }
                            else
                                return line[start..iread];
                            break;
                    }
                    ++iread;
                }
            }
            return null;
        }

        public string ReadUntil()
        {
            if (HasNext())
            {
                iread_start = iread;
                int start = iread;
                bool go = true;
                while (go && iread < line.Length)
                {
                    if (line[iread] != ' ')
                    {
                        switch (line[iread])
                        {
                            case '\'':
                            case '\"':
                                char c2 = line[iread];
                                for (++iread; iread < line.Length; iread++)
                                {
                                    iread_start = iread;
                                    if (line[iread] == c2)
                                        break;
                                }
                                break;

                            case '|':
                            case '>':
                            case '<':
                            case '&':
                                go = false;
                                break;

                            case '1':
                            case '2':
                                if (iread < line.Length - 1)
                                    switch (line[iread + 1])
                                    {
                                        case '>':
                                        case '|':
                                        case '&':
                                            go = false;
                                            break;
                                    }
                                else
                                {
                                    go = false;
                                    break;
                                }
                                break;

                            default:
                                iread_start = iread;
                                break;
                        }
                    }
                    if (go)
                        ++iread;
                }
                return line[start..++iread_start];
            }
            return null;
        }
    }
}