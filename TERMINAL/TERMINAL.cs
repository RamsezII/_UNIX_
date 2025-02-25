using System.Collections.Generic;
using _UTIL_;
using UnityEngine.EventSystems;

namespace _UNIX_
{
    public partial class TERMINAL : TERMINAL_base
    {
        readonly ThreadSafe<Queue<object>> allLogs = new(new());

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            AwakeUI();
            base.Awake();
        }

        public override void AssignShell(in SHELL shell)
        {
            base.AssignShell(shell);
            InitEntry();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Update()
        {
            if (Util.PullValue(rebuildOutput))
                RebuildOutput();

            if (rebuildTemp.Value)
                RebuildTemp();
            else
                AutoRebuildTemp();

            if (rebuildInput.PullValue())
                RebuildInput();

            base.Update();

            UpdateColor();
        }

        protected override void Log(in object msg)
        {
            lock (allLogs)
            {
                allLogs._value.Enqueue(msg);
                if (allLogs._value.Count > 100)
                    allLogs._value.Dequeue();
            }

            rebuildOutput.Value = true;
            refreshUI.Value = true;
        }

        public override void TakeLogFocus()
        {
            base.TakeLogFocus();
            EventSystem.current.SetSelectedGameObject(input_tf.field.gameObject);
        }

        public override void Kill()
        {
            base.Kill();
            if (terminals.Count == 1)
                V2.instance.Toggle(false);
        }
    }
}