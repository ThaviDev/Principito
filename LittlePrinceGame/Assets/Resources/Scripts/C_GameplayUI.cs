using System;
using UnityEngine;

public class C_GameplayUI : MonoBehaviour
{
    [SerializeField] private C_HoldButton m_WalkRightBtn;
    [SerializeField] private C_HoldButton m_WalkLeftBtn;

    [SerializeField] private GameObject m_WalkArrows;

    public event Action A_OnWalkLeftPressed;
    public event Action A_OnWalkRightPressed;
    public event Action A_OnWalkLeftReleased;
    public event Action A_OnWalkRightReleased;
    public Action<bool> A_SwitchWalkArrows;

    private void Awake()
    {
        if (m_WalkLeftBtn != null)
        {
            m_WalkLeftBtn.OnPressed += () => A_OnWalkLeftPressed?.Invoke();
            m_WalkLeftBtn.OnReleased += () => A_OnWalkLeftReleased?.Invoke();
        }
        if (m_WalkRightBtn != null)
        {
            m_WalkRightBtn.OnPressed += () => A_OnWalkRightPressed?.Invoke();
            m_WalkRightBtn.OnReleased += () => A_OnWalkRightReleased?.Invoke();
        }

        A_SwitchWalkArrows += SwitchWalkArrowsEvent;
    }
    private void OnDestroy()
    {
        A_SwitchWalkArrows -= SwitchWalkArrowsEvent;
    }
    private void SwitchWalkArrowsEvent(bool value)
    {
        m_WalkArrows.SetActive(value);
    }
}
