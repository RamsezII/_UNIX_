namespace _UNIX_
{
    public class BS : BOSON
    {
        readonly ushort bid;

        //----------------------------------------------------------------------------------------------------------

        public BS(in SIGNAL signal) : base(signal)
        {
            if (signal.TryReadCast<ushort>(out object _bid))
                bid = (ushort)_bid;
        }

        //----------------------------------------------------------------------------------------------------------

        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);

            if (bid > 0 && BOSONGOD.instance.bosons.TryGetValue(bid, out BOSON target))
                target.OnSignal(signal);
            else
            {
                const sbyte
                    c_PID = -6,
                    c_ID = -6,
                    c_STATE = -15,
                    c_NAME = -20,
                    c_TASK = -20;

                b_out.OnSignal(signal.SubSignal($"{"PID",c_PID}{"ID",c_ID}{"STATE",c_STATE}{"NAME",c_NAME}{"TASK",c_TASK}TIME"));

                lock (BOSONGOD.instance.bosons)
                    foreach (BOSON boson in BOSONGOD.instance.bosons.Values)
                        b_out.OnSignal(signal.SubSignal($"{boson.PBID,c_PID}{boson.BID,c_ID}{boson.state.Value,c_STATE}{boson.GetType().Name,c_NAME}{boson.TASK_LOG,c_TASK}{boson.startTime.TimeLog()}"));
            }

            return 0;
        }
    }
}