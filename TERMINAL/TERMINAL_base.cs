using System.Collections.Generic;
using _UTIL_;

namespace _UNIX_
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public abstract partial class TERMINAL_base : NUCLEON, ISignal
    {
        public static TERMINAL_base frontFocus;
        public static readonly HashSet<TERMINAL_base> terminals = new();

        protected SHELL shell;
        protected GLUON gluon;
        public bool rebuildInput = true;
        protected readonly ThreadSafe<bool>
            refreshUI = new(true),
            rebuildOutput = new(true);
        bool moveCaretToEnd;
        public bool IsFocus => this == frontFocus;

        //----------------------------------------------------------------------------------------------------------

        static TERMINAL_base()
        {
            terminals.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            gluon = new GLUON(new(null, SIGT.BUILD, terminal: this), this);
            BOSONGOD.bgod.PropagateIDs(gluon);
            terminals.Add(this);
        }

        public virtual void AssignShell(in SHELL shell)
        {
            if (this.shell != null)
                throw new System.Exception($"{this} Shell already set");
            this.shell = shell;
            shell.b_out = shell.b_err = gluon;
            BOSONGOD.bgod.PropagateIDs(shell);
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
            terminals.Remove(this);
            shell.Dispose();
            gluon.Dispose();
        }
    }
}