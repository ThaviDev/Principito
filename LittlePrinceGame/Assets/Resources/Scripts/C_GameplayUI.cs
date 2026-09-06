using System;
using UnityEngine;

public class C_GameplayUI : MonoBehaviour
{
    public Action<bool> A_SwitchWalkArrows;
    [SerializeField] private GameObject walkArrows;
    void Start()
    {
        A_SwitchWalkArrows += SwitchWalkArrowsEvent;
    }
    private void SwitchWalkArrowsEvent(bool value)
    {
        walkArrows.SetActive(value);
    }
    private void OnDestroy()
    {
        A_SwitchWalkArrows -= SwitchWalkArrowsEvent;
    }
}
