using _ARK_;
using UnityEngine;

namespace _RC_
{
    internal class TestBoson : BOSON
    {
        public TestBoson(in SIGNAL signal) : base(signal)
        {

        }

        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);
            Debug.Log("TestBoson.OnSignal");
            return 0;
        }
    }
}