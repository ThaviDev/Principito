using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class C_HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnPressed;
    public event Action OnReleased;
    public void OnPointerDown(PointerEventData eventData) => OnPressed?.Invoke();
    public void OnPointerUp(PointerEventData eventData) => OnReleased?.Invoke(); 
}
