using _ARK_;

namespace _CORE_
{
    public partial class TERMINAL
    {
        byte tab_ARMED;

        //----------------------------------------------------------------------------------------------------------

        void OnTab()
        {
            if (tab_ARMED >= 2)
                return;

            SIGT sig = tab_ARMED++ == 0 ? SIGT.FIRST_TAB : SIGT.SECOND_TAB;

            SIGNAL signal = new(null, sig, input_tf.field.text, (ushort)input_tf.field.caretPosition, terminal: this);
            shell.OnSignal(signal);

            if (signal.completed)
            {
                input_tf.field.text = signal.completion;
                MoveCaret(signal.caret);
            }
        }

        protected override void MoveCaret(int caret, in bool moveEndFlag = false)
        {
            base.MoveCaret(caret, moveEndFlag);
            if (caret < 0)
                caret = input_tf.field.text.Length;
            input_tf.field.caretPosition = caret;
            input_tf.field.selectionAnchorPosition = caret;
            input_tf.field.selectionFocusPosition = caret;
        }
    }
}