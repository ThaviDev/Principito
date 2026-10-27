using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class C_PlayerMotor : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private C_PlayerInputAdapter m_Input;
    [SerializeField] private C_PlayerGroundDetector m_Grounded;
    [SerializeField] private C_PlayerMovement m_Movement;
    [SerializeField] private C_PlayerLaunchController m_Launch;
    [SerializeField] private C_PlayerCameraController m_Camera;

    private Rigidbody2D m_rb;
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.freezeRotation = true;

        if (m_Input == null) m_Input = GetComponent<C_PlayerInputAdapter>();
        if (m_Grounded == null) m_Grounded = GetComponent<C_PlayerGroundDetector>();
        if (m_Movement == null) m_Movement = GetComponent<C_PlayerMovement>();
        if (m_Launch == null) m_Launch = GetComponent<C_PlayerLaunchController>();
        if (m_Camera == null) m_Camera = GetComponent<C_PlayerCameraController>();
    }

    private void OnEnable()
    {
        m_Input.A_OnTouchDown += HandleTouchDown;
        m_Input.A_OnTouchUp += HandleTouchUp;
    }

    private void OnDisable()
    {
        m_Input.A_OnTouchDown -= HandleTouchDown;
        m_Input.A_OnTouchUp -= HandleTouchUp;
    }

    private void FixedUpdate()
    {
        // Mientras preparas un lanzamiento, congelas velocidad y no procesas movimiento
        if (m_Launch.IsPreparing)
        {
            m_rb.linearVelocity = Vector2.zero;
            m_Launch.Preparing();
            return;
        }

        if (m_Grounded.IsGrounded)
            m_Movement.Grounded(m_Input.WalkInput);
        else
            m_Movement.Fly();
    }

    private void HandleTouchDown(Vector2 worldPos)
        => m_Launch.TryBegin(worldPos, m_Grounded.IsGrounded);

    private void HandleTouchUp(Vector2 fromWorld, Vector2 toWorld)
        => m_Launch.TryRelease(fromWorld, toWorld);
}
