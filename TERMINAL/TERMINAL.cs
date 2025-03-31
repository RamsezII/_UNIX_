using System.Collections.Generic;
using _ARK_;
using _UTIL_;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _UNIX_
{
    public partial class TERMINAL : TERMINAL_base
    {
        readonly ThreadSafe_class<Queue<object>> allLogs = new(new());

        //----------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.getInputs += () =>
            {
                if (instances.Count == 0)
                    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.T))
                    {
                        Util.InstantiateOrCreate<TERMINAL>(NUCLEOR.instance.canvas2D.transform);
                        ((TERMINAL)instances[0]).AssignShell(new CORE_SHELL(SIGNAL.SIG_VOID));
                    }
            };
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            AwakeUI();
            base.Awake();

            USAGES.ToggleUser(this, true, UsageGroups.Keyboard, UsageGroups.Typing, UsageGroups.IngameMouse);
        }

        public override void AssignShell(in SHELL shell)
        {
            base.AssignShell(shell);
            InitEntry();
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Update()
        {
            if (rebuildOutput.PullValue())
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
            if (instances.Count == 1)
                V2.instance.Toggle(false);
        }
    }
}