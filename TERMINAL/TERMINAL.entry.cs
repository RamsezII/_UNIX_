using _ARK_;
using _V2_;
using TMPro;

namespace _CORE_
{
    public partial class TERMINAL
    {
        bool drawInput;
        static readonly bool closeV2whenEnter = true;

        //----------------------------------------------------------------------------------------------------------

        void InitEntry()
        {
            input_tf.field.onSelect.AddListener(input => TakeFrontWindowsFocus());
            output_tf.field.onSelect.AddListener(input => TakeFrontWindowsFocus());
            label_tf.field.onSelect.AddListener(input => TakeFrontWindowsFocus());

            input_tf.field.onValueChanged.AddListener(input =>
            {
                tab_ARMED = 0;
                refreshUI.Value = true;
                if (input != null && string.IsNullOrWhiteSpace(input) && input.Contains('\n'))
                    input_tf.field.text = input.Replace("\n", string.Empty);
            });

            input_tf.field.onValidateInput = (text, charIndex, addedChar) =>
            {
                if (addedChar == '\t')
                    return '\0';
                tab_ARMED = 0;
                return addedChar;
            };

            input_tf.field.onSubmit.AddListener(input =>
            {
                tab_ARMED = 0;
                rebuildInput = true;
                shell.hist_i = 0;
                if (input.IsNull())
                    if (closeV2whenEnter)
                        V2.instance.Toggle(false);
                    else
                        Log(label_tf.field.text);
                else
                {
                    switch (input_tf.field.contentType)
                    {
                        case TMP_InputField.ContentType.Password:
                            Log(label_tf.field.text + " " + new string('*', input.Length));
                            break;
                        default:
                            Log(label_tf.field.text + " " + input);
                            break;
                    }
                    ushort caret = (ushort)input_tf.field.caretPosition;
                    shell.OnSignal(new(null, SIGT.BUILD, input, caret, terminal: this));
                }

                TakeFrontWindowsFocus();
            });
        }

        public override void TakeFrontWindowsFocus()
        {
            base.TakeFrontWindowsFocus();

            transform.SetAsLastSibling();

            input_tf.field.ActivateInputField();
            input_tf.field.Select();
            input_tf.field.caretPosition = 0;
            input_tf.field.selectionAnchorPosition = 0;
            input_tf.field.selectionFocusPosition = 0;
            input_tf.field.text = string.Empty;
        }

        //----------------------------------------------------------------------------------------------------------

        void RebuildInput()
        {
            if (drawInput = shell.OnDrawInputAttempt(out string prefixe, out SIG_WAIT sigw))
            {
                label_tf.field.text = prefixe;
                input_tf.field.contentType = sigw switch
                {
                    SIG_WAIT.NAME => TMP_InputField.ContentType.Name,
                    SIG_WAIT.EMAIL => TMP_InputField.ContentType.EmailAddress,
                    SIG_WAIT.PASSWORD => TMP_InputField.ContentType.Password,
                    _ => TMP_InputField.ContentType.Standard,
                };
            }

            label_tf.gameObject.SetActive(drawInput);
            input_tf.gameObject.SetActive(drawInput);

            refreshUI.Value = true;
        }
    }
}