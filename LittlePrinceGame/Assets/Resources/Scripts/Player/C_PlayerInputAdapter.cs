using System;
using UnityEngine;
public enum GroundDirection
{
    Up,
    Down,
    Left,
    Right
}
public class C_PlayerInputAdapter : MonoBehaviour
{
    public enum GroundControlMode { Buttons, Swipe }
    private GroundControlMode m_GroundMode = GroundControlMode.Swipe;
    private GroundDirection m_Direction;
    [SerializeField] private C_GameplayUI m_UI;
    [SerializeField] private C_PlayerGroundDetector m_GroundDetector;
    [SerializeField] private float m_WalkAngle;
    private float m_UpDownAngle;

    public Vector2 WalkInput {  get; private set; }
    public bool IsTouching { get; private set; }

    public event Action<Vector2> A_OnTouchDown;
    public event Action<Vector2, Vector2> A_OnTouchUp;
    public event Action A_OnJump;

    private bool m_WasTouching;
    private Vector2 m_TouchWorldDown;

    public GroundControlMode MyGroundControlMode => m_GroundMode;
    public GroundDirection Direction => m_Direction;

    private void Awake()
    {
        if (m_UI == null) m_UI = FindAnyObjectByType<C_GameplayUI>();
        if (m_GroundDetector == null) m_GroundDetector = GetComponent<C_PlayerGroundDetector>();
    }
    private void Start()
    {
        m_UpDownAngle = 90 - m_WalkAngle;
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
        bool inptIsTouching;
        GameObject inptTouchDownObj;
        GameObject inptTouchUpdateObj;
        GameObject inptTouchUpObj;

        inptIsTouching = C_TouchInputManager.Instance.IsTouchPressing;
        inptTouchDownObj = C_TouchInputManager.Instance.TouchDownObj;
        inptTouchUpdateObj = C_TouchInputManager.Instance.TouchFollower;
        inptTouchUpObj = C_TouchInputManager.Instance.TouchUpObj;

        IsTouching = inptIsTouching;
        if (IsTouching)
        {
            if (inptTouchDownObj != null)
                m_TouchWorldDown = inptTouchDownObj.transform.position;
            if (!m_WasTouching)
            {
                // Acabamos de tocar la pantalla
                A_OnTouchDown?.Invoke(m_TouchWorldDown);
            }
            else
            {
                if (m_GroundDetector != null && m_GroundDetector.IsGrounded)
                {
                    GetGroundGesture(C_TouchInputManager.Instance);
                }
            }
        }
        else if (m_WasTouching)
        {
            Vector2 upWorld = inptTouchUpObj != null ? (Vector2)inptTouchUpObj.transform.position : m_TouchWorldDown;
            A_OnTouchUp?.Invoke(m_TouchWorldDown, upWorld);
        }
        m_WasTouching = IsTouching;
    }

    private void GetGroundGesture(C_TouchInputManager touch)
    {
        if (touch.TouchFollower == null) { Debug.LogWarning("C_PlayerInputAdapter no" +
            "puede conseguir a C_TouchInputManager para GetGroundGesture"); return; }
        Vector2 currentPos = touch.TouchFollower.transform.position;
        Vector2 dragVector = (currentPos - m_TouchWorldDown);

        if (dragVector.sqrMagnitude < 0.2f) { WalkInput = Vector2.zero; return; }

        Vector2 localDragVector = this.transform.InverseTransformDirection(dragVector);
        float degrees = Mathf.Atan2(localDragVector.y, localDragVector.x) * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }
        //Debug.Log("Angulo del Swipe: " + degrees);
        if (degrees <= m_WalkAngle || degrees >= 360f - m_WalkAngle)
        {
            WalkInput = Vector2.left;
            Debug.Log("Izquierda");
            m_Direction = GroundDirection.Left;
        } else if (Mathf.Abs(degrees - 90f) <= m_UpDownAngle)
        {
            WalkInput = Vector2.zero;
            Debug.Log("Arriba");
            m_Direction = GroundDirection.Up;
        }
        else if (Mathf.Abs(degrees - 180f) <= m_WalkAngle)
        {
            WalkInput = Vector2.right;
            Debug.Log("Derecha");
            m_Direction = GroundDirection.Right;
        }
        else if (Mathf.Abs(degrees - 270f) <= m_UpDownAngle)
        {
            WalkInput = Vector2.zero;
            Debug.Log("Abajo");
            m_Direction = GroundDirection.Down;
        }
    }
}
