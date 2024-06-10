using _ARK_;

namespace _V2_
{
    public class V2 : UIBUON
    {
        public static V2 instance;

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            instance = this;
            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
            toggled = true;
            Toggle(false);
        }

        //--------------------------------------------------------------------------------------------------------------

        public void OnClick_background()
        {
            Toggle(false);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            if (this == instance)
                instance = null;
        }
    }
}