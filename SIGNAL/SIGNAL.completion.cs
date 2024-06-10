using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public partial class SIGNAL
    {
        public string completion;
        public bool isOnTab, completed;

        //----------------------------------------------------------------------------------------------------------

        public bool IsTab() => sigtype == SIGT.FIRST_TAB || sigtype == SIGT.SECOND_TAB;
        public bool IsOnTab() => (isOnTab || iread >= caret || iread >= line.Length) && IsTab();

        public bool IsOnTab_move()
        {
            if (!IsTab())
                return false;



            return true;
        }

        public bool TryCompleteCmd<T>(string arg0, in T start, in T end) where T : Enum => TryCompleteCmd(ref arg0, Util.EEnumNames(start, end).Matches2(arg0));
        public bool TryCompleteCmd(ref string arg0, in IEnumerable<string> matches)
        {
            switch (sigtype)
            {
                case SIGT.FIRST_TAB:
                    if (Util.TryComplete(arg0, matches, out string compl))
                    {
                        arg0 = compl;
                        Completion(compl);
                        return true;
                    }
                    break;

                case SIGT.SECOND_TAB:
                    Debug.Log(matches.Join("  "));
                    break;
            }
            return false;
        }

        public void Completion(in string value)
        {
            if (value != null)
            {
                completion = line[..iread_start] + value + line[iread..];
                completed = true;
                caret = iread_start + value.Length;
            }
        }

        public bool TryReadCompletable(out string output, in IEnumerable<string> completions)
        {
            bool yes = TryRead(out output);
            if (IsOnTab())
                yes |= TryCompleteCmd(ref output, completions.Matches2(output));
            return yes;
        }
    }
}