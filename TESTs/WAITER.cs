using UnityEngine;

namespace _UNIX_
{
    public class WAITER : BOSON
    {
        float time;

        //----------------------------------------------------------------------------------------------------------

        public WAITER(in SIGNAL signal) : base(signal, false)
        {
            if (signal.TryReadCast<int>(out object output))
                time = (float)output;
            else
                time = 1;

            if (signal.sigtype >= SIGT.BUILD)
                SCHEDULER_old.scheduler.ADD_PARALLEL(this);
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnTick()
        {
            base.OnTick();
            if ((time -= Time.unscaledDeltaTime) <= 0)
            {
                terminal.TakeLogFocus();
                Debug.Log("WAITER: " + time);
                Dispose();
            }
        }
    }
}