using _UNIX_;
using UnityEngine;

namespace _UNIX_
{
    public partial class TERMINAL
    {
        public override void OnFocusedUpdate()
        {
            base.OnFocusedUpdate();

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    shell.KillCurrentControl(() =>
                    {
                        if (input_tf.field.contentType == TMPro.TMP_InputField.ContentType.Password)
                            Log(label_tf.field.text + " " + new string('*', input_tf.field.text.Length));
                        else
                            Log(label_tf.field.text + " " + input_tf.field.text);
                        input_tf.field.text = string.Empty;
                    });
                    rebuildInput = true;
                }

                if (Input.GetKeyDown(KeyCode.L))
                {
                    scrollbar.value = 0;
                    refreshUI.Value = true;
                }

                if (Input.GetKeyDown(KeyCode.Backspace))
                    if (!input_tf.field.text.IsNull(false))
                    {
                        int i = input_tf.field.caretPosition;
                        bool cleanOnly = false;

                        while (i > 0 && input_tf.field.text[--i] == ' ')
                            cleanOnly = true;

                        if (!cleanOnly)
                            while (i > 0 && input_tf.field.text[--i] != ' ') ;

                        if (i > 0)
                            ++i;

                        input_tf.field.text = input_tf.field.text[..i];
                        refreshUI.Value = true;
                    }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    input_tf.field.text = string.Empty;
                    refreshUI.Value = true;
                }
            }
            else
            {
                SIGT sig = 0;
                if (Input.GetKeyDown(KeyCode.Tab))
                    OnTab();
                else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        sig = SIGT.ALT_UP;
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                        sig = SIGT.ALT_RIGHT;
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        sig = SIGT.ALT_DOWN;
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                        sig = SIGT.ALT_LEFT;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        sig = SIGT.HIST_UP;
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        sig = SIGT.HIST_DOWN;
                }

                if (sig > 0)
                {
                    SIGNAL signal = new(null, sig, input_tf.field.text, (ushort)input_tf.field.caretPosition, terminal: this);
                    shell.OnSignal(signal);
                    if (signal.completed)
                        input_tf.field.text = signal.completion;
                    MoveCaret((ushort)input_tf.field.text.Length, true);
                }
            }
        }
    }
}