using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _CORE_
{
    public class DRAGGABLE : MonoBehaviour, IDragHandler
    {
        public Action<DRAGGABLE, PointerEventData> onDrag;
        public void OnDrag(PointerEventData eventData) => onDrag(this, eventData);
    }
}