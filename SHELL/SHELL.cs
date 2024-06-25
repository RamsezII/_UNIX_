namespace _UNIX_
{
    public abstract partial class SHELL : BOSON
    {
        public SHELL(in SIGNAL signal) : base(signal, false)
        {
            prefixe = ">";
            if (signal.sigtype >= SIGT.BUILD)
                SCHEDULER_old.scheduler.ADD_PARALLEL(this);
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnTick()
        {
            base.OnTick();
            while (controlBosons.TryPeek(out BOSON control))
                if (control.state.Value != BOSON_STAT.DISPOSED)
                    break;
                else
                {
                    controlBosons.Dequeue();
                    terminal.rebuildInput = true;
                }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            lock (controlBosons)
                while (controlBosons.TryDequeue(out BOSON control))
                    control.Dispose();
        }
    }
}