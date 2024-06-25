using System;
using UnityEngine;

namespace _UNIX_
{
    public partial class SHELL
    {
        public override SIG_WAIT OnSignal(in SIGNAL signal)
        {
            base.OnSignal(signal);

            void NavHist(in int nav)
            {
                lock (history)
                    if (history.Count > 0)
                    {
                        hist_i += nav;
                        hist_i.Repeat0(1 + history.Count);

                        if (hist_i == 0)
                            sig_last.completion = string.Empty;
                        else
                            sig_last.completion = history[^hist_i];

                        sig_last.completed = true;
                    }
            }

            switch (sig_last.sigtype)
            {
                case SIGT.HIST_UP:
                    NavHist(1);
                    break;

                case SIGT.HIST_DOWN:
                    NavHist(-1);
                    break;

                default:
                    while (controlBosons.TryPeek(out BOSON control))
                    {
                        terminal.rebuildInput = true;
                        if (control.state.Value == BOSON_STAT.DISPOSED)
                            controlBosons.Dequeue();
                        else
                            return control.OnSignal(sig_last);
                    }

                    if (sig_last.HasNext())
                        try
                        {
                            BOSON previous = null;
                            bool background = false;

                            bool
                                chain = false,
                                pipe_OUT = false,
                                pipe_ERR = false,
                                save1 = false,
                                save2 = false,
                                append1 = false,
                                append2 = false,
                                fusion = false,
                                expectsSpecial = false;

                            while (!chain && sig_last.TryRead(out string arg0, true, false))
                            {
                                BOSON current = null;
                                bool resetSpecials = true;
                                switch (arg0)
                                {
                                    case "|":
                                    case "1|":
                                        pipe_OUT = true;
                                        break;

                                    case "2|":
                                        pipe_ERR = true;
                                        break;

                                    case "\n":
                                    case "&&":
                                    case ";":
                                        chain = true;
                                        break;

                                    case ">>":
                                    case "1>>":
                                        append1 = true;
                                        goto case ">";
                                    case ">":
                                    case "1>":
                                        save1 = true;
                                        break;

                                    case "2>>":
                                        append2 = true;
                                        goto case "2>";
                                    case "2>":
                                        save2 = true;
                                        break;

                                    case "2>&1":
                                        fusion = true;
                                        break;

                                    case "&":
                                        background = true;
                                        break;

                                    default:
                                        if (expectsSpecial)
                                            throw new SIG_EXCP($"Unexpected argument: '{arg0}', expected command delimiter or end of line");
                                        if ((current = TryBoson(arg0, sig_last)) == null && sig_last.sigtype >= SIGT.BUILD)
                                            throw new SIG_EXCP($"Unknown command: '{arg0}'");
                                        expectsSpecial = true;
                                        resetSpecials = false;
                                        break;
                                }

                                if (resetSpecials)
                                    expectsSpecial = false;

                                if (current != null)
                                {
                                    current.pboson = this;
                                    current.b_out = current.b_err = b_out;

                                    if (sig_last.sigtype >= SIGT.BUILD)
                                        controlBosons.Enqueue(current);

                                    if (fusion)
                                    {
                                        previous.b_out = previous.b_err = current;
                                        fusion = false;
                                    }

                                    if (pipe_OUT && previous != null)
                                    {
                                        previous.b_out = current;
                                        pipe_OUT = false;
                                    }

                                    if (pipe_ERR && previous != null)
                                    {
                                        previous.b_err = current;
                                        pipe_ERR = false;
                                    }

                                    previous = current;
                                }
                            }

                            if (BosonControl(out BOSON control))
                                if (sig_last.sigtype >= SIGT.BUILD)
                                {
                                    BOSONGOD.bgod.PropagateIDs(control);
                                    if (background)
                                    {
                                        background = false;
                                        lock (controlBosons)
                                        {
                                            foreach (BOSON boson in controlBosons)
                                            {
                                                boson.background = true;
                                                Debug.Log(boson.ToSubLog());
                                            }
                                            controlBosons.Clear();
                                        }
                                    }
                                    AddToHistory(sig_last.line);
                                    // control.OnSignal(new(this, SIGT.CHECK));
                                    SIG_WAIT sig = control.OnSignal(new(this, SIGT.EXEC));
                                    control.PropagateAutoDispose();
                                    terminal.rebuildInput = true;
                                    return sig;
                                }
                        }
                        catch (Exception e)
                        {
                            foreach (BOSON control in controlBosons)
                                control.Dispose();
                            controlBosons.Clear();
                            if (e is SIG_EXCP)
                                Debug.LogWarning(e.TrimMessage());
                            else
                                Debug.LogException(e);
                            sig_last.Consume();
                        }
                    break;
            }
            return SIG_WAIT.STANDARD;
        }
    }
}