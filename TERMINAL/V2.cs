using UnityEngine;

namespace _UNIX_
{
    public class V2 : MonoBehaviour
    {
        public static V2 instance;
        public bool toggled;

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            instance = this;
        }

        //--------------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
            toggled = true;
            Toggle(false);
        }

        //--------------------------------------------------------------------------------------------------------------

        public void Toggle(in bool value)
        {
            if (toggled == value)
                return;
            toggled = value;
            gameObject.SetActive(value);
        }

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