using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TriggerSide {
    Left,
    Right
}

public class ItemTrigger : MonoBehaviour, IPointerEnterHandler {
        
    public TriggerSide side;
    public event Action<TriggerSide> onPointerEnter;
        
    public void OnPointerEnter(PointerEventData eventData) {
        onPointerEnter?.Invoke(side);
    }
}