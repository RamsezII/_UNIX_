using _UTIL_;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _UNIX_
{
    public class BOSON_EXCP : Exception
    {
        public BOSON_EXCP(in string message) : base(message)
        {
        }
    }

    public enum BOSON_STAT : byte
    {
        _none_,
        DAEMON,
        BACKGROUND,
        CONTROL,
        PENDING,
        SCHEDULED,
        RUNNING,
        DONE,
        FAILURE,
        DISPOSED,
        _last_,
    }

    public enum SIG_WAIT : byte
    {
        _null_,
        BLOCK,
        STANDARD,
        LINE,
        AUTO_CORRECT,
        INTEGER,
        DECIMAL,
        ALPHA_NUM,
        NAME,
        EMAIL,
        PASSWORD,
        PIN,
    }

    public interface ISignal { SIG_WAIT OnSignal(in SIGNAL signal); }

    /// <summary>
    /// BOSON is a process, running on SIGNALs, that can be scheduled, run, and disposed.
    /// </summary>
    public abstract partial class BOSON : IDisposable
    {
        public ushort BID;
        public ushort PBID => pboson?.BID ?? 0;

        public readonly TERMINAL_base terminal;
        public BOSON pboson;
        public BOSON b_out, b_err;

        public string prefixe, TASK_LOG = ".";
        protected SIGNAL sig_last = SIGNAL.SIG_VOID;
        public IEnumerator<SIG_WAIT> sig_wait;
        public IEnumerator etask;

        public readonly bool autoDispose;
        public bool background, killable;
        public readonly ThreadSafe<BOSON_STAT> state = new();
        public readonly float startTime = Time.realtimeSinceStartup;
        public override string ToString() => $"{nameof(BOSON)}(pid:{PBID}; id:{BID}; name:{GetType().Name}; state:{state.Value}; task:{TASK_LOG}; start:{startTime.TimeLog()})";

        //----------------------------------------------------------------------------------------------------------

        public BOSON(in SIGNAL signal, in bool autoDispose = true, in bool killable = false)
        {
            pboson = signal.emitter;
            this.autoDispose = autoDispose;
            this.killable = killable;

            terminal = signal.terminal;
            if (terminal != null)
                terminal.TakeLogFocus();

            // --help
            // --example
            // --manual

            if (signal.sigtype < SIGT.BUILD)
                return;

            state._value = BOSON_STAT.RUNNING;
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnSchedule()
        {
            state._value = BOSON_STAT.RUNNING;
        }

        public virtual void OnTick()
        {
            if (etask != null && !etask.MoveNext())
            {
                etask = null;
                Dispose();
            }
        }

        public SIG_WAIT EmptySignal(in SIGT sigtype) => OnSignal(new(pboson, sigtype, terminal: terminal));
        public virtual SIG_WAIT OnSignal(in SIGNAL signal)
        {
            sig_last = signal;

            if (signal.terminal != null)
                signal.terminal.TakeLogFocus();
            else if (terminal != null)
                terminal.TakeLogFocus();

            if (this == signal.emitter)
            {
                signal.Consume();
                throw new SIG_EXCP($"{"SIG_LOOP".Bold()} error: \"{signal.line.Bold()}\" will not be handled by {$"{nameof(BOSON)}[{BID}]".Bold()} because it is the emitter.");
            }
            else if (sig_wait != null)
                if (sig_wait != null && sig_wait.MoveNext())
                    return sig_wait.Current;
                else
                    sig_wait = null;
            return 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public void PropagateAutoDispose()
        {
            if (autoDispose)
                Dispose();
        }

        public void Dispose()
        {
            BOSONGOD.collectDeadBosons.Value = true;

            sig_wait = null;

            b_out?.PropagateAutoDispose();
            b_err?.PropagateAutoDispose();

            lock (state)
                if (state._value == BOSON_STAT.DISPOSED)
                    return;
                else
                    state._value = BOSON_STAT.DISPOSED;

            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}