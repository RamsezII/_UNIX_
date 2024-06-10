namespace _ARK_
{
    /// <summary>
    /// GLUON is a type of BOSON that is used to manage the interaction between NUCLEON and SIGNAL.
    /// </summary>
    public class GLUON : BOSON
    {
        public readonly ISignal nucleon;

        //----------------------------------------------------------------------------------------------------------

        public GLUON(in SIGNAL signal, in ISignal nucleon) : base(signal, false)
        {
            this.nucleon = nucleon;
        }

        //----------------------------------------------------------------------------------------------------------

        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);
            return nucleon.OnSignal(signal);
        }
    }
}