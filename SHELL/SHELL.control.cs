using System;
using System.Collections.Generic;
using UnityEngine;

namespace _UNIX_
{
    public partial class SHELL
    {
        readonly Queue<BOSON> controlBosons = new();

        //----------------------------------------------------------------------------------------------------------

        public void KillCurrentControl(in Action justBeforeKill)
        {
            lock (controlBosons)
            {
                foreach (BOSON control in controlBosons)
                    if (!control.killable)
                    {
                        Debug.Log("^C");
                        Debug.LogWarning($"Attempt to kill unkillable process: {control}");
                        return;
                    }
                justBeforeKill?.Invoke();
                Debug.Log("^C");
                foreach (BOSON control in controlBosons)
                    control.Dispose();
                controlBosons.Clear();
            }
        }

        bool BosonControl(out BOSON control)
        {
            while (controlBosons.TryPeek(out control))
                lock (control.state)
                    if (control.state.Value != BOSON_STAT.DISPOSED)
                        return true;
                    else
                    {
                        controlBosons.Dequeue();
                        terminal.rebuildInput = true;
                    }
            return false;
        }

        public bool OnDrawInputAttempt(out string prefixe, out SIG_WAIT sig)
        {
            if (!BosonControl(out BOSON control))
                control = this;
            prefixe = control.prefixe;
            return (sig = control.EmptySignal(0)) > SIG_WAIT.BLOCK;
        }
    }
}