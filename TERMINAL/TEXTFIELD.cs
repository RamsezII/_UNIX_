using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _UNIX_
{
    public class TEXTFIELD : MonoBehaviour, IScrollHandler
    {
        public RectTransform rT;
        public TMP_InputField field;
        public TextMeshProUGUI text;

        //----------------------------------------------------------------------------------------------------------

        public void OnAwake()
        {
            rT = (RectTransform)transform;
            field = GetComponent<TMP_InputField>();
            text = transform.Find("Text Area/TEXT").GetComponent<TextMeshProUGUI>();
            text.text = string.Empty;
        }

        //----------------------------------------------------------------------------------------------------------

        void IScrollHandler.OnScroll(PointerEventData eventData) => GetComponentInParent<ScrollRect>().OnScroll(eventData);
    }
}