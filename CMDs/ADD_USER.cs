using System.Collections.Generic;

namespace _ARK_
{
    public class ADD_USER : BOSON
    {
        string name, password;

        //----------------------------------------------------------------------------------------------------------

        public ADD_USER(in SIGNAL signal) : base(signal, false, true)
        {
            signal.TryRead(out name);
            signal.TryRead(out password);

            if (signal.sigtype >= SIGT.BUILD)
                sig_wait = ERoutine();
        }

        //----------------------------------------------------------------------------------------------------------

        IEnumerator<SIG_WAIT> ERoutine()
        {
            if (name.IsNull())
            {
                prefixe = "name:";
                while (!sig_last.TryRead(out name))
                    yield return SIG_WAIT.NAME;
            }

            if (password.IsNull())
            {
                prefixe = "password:";
                while (!sig_last.TryRead(out password))
                    yield return SIG_WAIT.PASSWORD;
            }

            b_out.OnSignal(new(pboson, SIGT.EXEC, $"name: \"{name}\"; password \"{password}\";"));

            Dispose();
        }
    }
}