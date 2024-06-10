using System.Collections.Generic;
using _ARK_;
using _UTIL_;
using UnityEngine;
using _V2_;

namespace _CORE_
{
    public partial class TERMINAL : TERMINAL_base
    {
        readonly ThreadSafe<Queue<object>> allLogs = new(new());

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            AwakeUI();
            base.Awake();
            Application.targetFrameRate = 75;
            QualitySettings.vSyncCount = 1;
        }

        public override void AssignShell(in SHELL shell)
        {
            base.AssignShell(shell);
            InitEntry();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnUpdate()
        {
            if (Util.PullValue(rebuildOutput))
                RebuildOutput();

            if (rebuildTemp.Value)
                RebuildTemp();
            else
                AutoRebuildTemp();

            if (rebuildInput.PullValue())
                RebuildInput();

            base.OnUpdate();

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
            try
            {
                UIGOD.instance.eventSystem.SetSelectedGameObject(input_tf.field.gameObject);
            }
            catch
            {

            }
        }

        public override void Kill()
        {
            base.Kill();
            if (terminals.Count == 1)
                V2.instance.Toggle(false);
        }
    }
}