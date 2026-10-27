using System;
using UnityEngine;

public class C_PlayerInputAdapter : MonoBehaviour
{
    public enum GroundControlMode { Buttons, Swipe }
    private GroundControlMode m_GroundMode = GroundControlMode.Buttons;
    [SerializeField] private C_GameplayUI m_UI;

    public Vector2 WalkInput {  get; private set; }
    public bool IsTouching { get; private set; }

    public event Action<Vector2> A_OnTouchDown;
    public event Action<Vector2, Vector2> A_OnTouchUp;
    public event Action A_OnJump;

    private bool m_WasTouching;
    private Vector2 m_TouchWorldDown;

    public GroundControlMode MyGroundControlMode => m_GroundMode;

    private void Awake()
    {
        if (m_UI == null) m_UI = FindAnyObjectByType<C_GameplayUI>();
    }
    private void OnEnable()
    {
        if (m_UI == null) return;
        m_UI.A_OnWalkLeftPressed += HandleWalkLeftPressed;
        m_UI.A_OnWalkRightPressed += HandleWalkRightPressed;
        m_UI.A_OnWalkLeftReleased += HandleWalkLeftReleased;
        m_UI.A_OnWalkRightReleased += HandleWalkRightReleased;
    }
    private void OnDisable()
    {
        if (m_UI == null) return;
        m_UI.A_OnWalkLeftPressed -= HandleWalkLeftPressed;
        m_UI.A_OnWalkRightPressed -= HandleWalkRightPressed;
        m_UI.A_OnWalkLeftReleased -= HandleWalkLeftReleased;
        m_UI.A_OnWalkRightReleased -= HandleWalkRightReleased;
    }
    private void HandleWalkLeftPressed() => WalkInput = Vector2.left;
    private void HandleWalkRightPressed() => WalkInput = Vector2.right;
    private void HandleWalkLeftReleased() { if (WalkInput.x < 0) WalkInput = Vector2.zero; }
    private void HandleWalkRightReleased() { if (WalkInput.x > 0) WalkInput = Vector2.zero; }

    private void Update()
    {
        bool m_inptIsTouching;
        GameObject m_inptTouchDownObj;
        GameObject m_inptTouchUpdateObj;
        GameObject m_inptTouchUpObj;

        m_inptIsTouching = C_TouchInputManager.Instance.IsTouchPressing;
        m_inptTouchDownObj = C_TouchInputManager.Instance.TouchDownObj;
        m_inptTouchUpdateObj = C_TouchInputManager.Instance.TouchFollower;
        m_inptTouchUpObj = C_TouchInputManager.Instance.TouchUpObj;

        IsTouching = m_inptIsTouching;
        if (IsTouching)
        {
            if (m_inptTouchDownObj != null)
                m_TouchWorldDown = m_inptTouchDownObj.transform.position;
            if (!m_WasTouching)
                A_OnTouchDown?.Invoke(m_TouchWorldDown);
        }
        else if (m_WasTouching)
        {
            Vector2 upWorld = m_inptTouchUpObj != null ? (Vector2)m_inptTouchUpObj.transform.position : m_TouchWorldDown;
            A_OnTouchUp?.Invoke(m_TouchWorldDown, upWorld);
        }
        m_WasTouching = IsTouching;
    }
}
