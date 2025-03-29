using System.Collections.Generic;
using _UTIL_;
using UnityEngine;

namespace _UNIX_
{
    public abstract partial class TERMINAL_base : NUCLEON, ISignal
    {
        public static TERMINAL_base frontFocus;
        public static readonly List<TERMINAL_base> instances = new();

        protected SHELL shell;
        protected GLUON gluon;
        public bool rebuildInput = true;
        protected readonly ThreadSafe_struct<bool>
            refreshUI = new(true),
            rebuildOutput = new(true);
        bool moveCaretToEnd;
        public bool IsFocus => this == frontFocus;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            instances.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            gluon = new GLUON(new(null, SIGT.BUILD, terminal: this), this);
            BOSONGOD.instance.PropagateIDs(gluon);
            instances.Add(this);
        }

        public virtual void AssignShell(in SHELL shell)
        {
            if (this.shell != null)
                throw new System.Exception($"{this} Shell already set");
            this.shell = shell;
            shell.b_out = shell.b_err = gluon;
            BOSONGOD.instance.PropagateIDs(shell);
            shell.OnSignal(new(null, SIGT.BUILD, terminal: this));
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();
            TakeFrontWindowsFocus();

            lock (LOG_CATCHER.logqueue)
            {
                foreach (var log in LOG_CATCHER.logqueue)
                    Log(log);
                LOG_CATCHER.logqueue.Clear();
            }
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void TakeFrontWindowsFocus()
        {
            frontFocus = this;
        }

        protected virtual void MoveCaret(int caret, in bool moveEndFlag = false)
        {
            if (moveEndFlag)
                moveCaretToEnd = true;
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Update()
        {
            if (moveCaretToEnd)
            {
                moveCaretToEnd = false;
                MoveCaret(-1, false);
            }
            if (refreshUI.PullValue())
                OnRefreshUI();
        }

        public virtual void OnFocusedUpdate()
        {

        }

        public virtual void Kill()
        {
            Destroy(gameObject);
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnRefreshUI()
        {
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();
            instances.Remove(this);
            shell.Dispose();
            gluon.Dispose();
        }
    }
}