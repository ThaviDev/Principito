using UnityEngine;

public class C_PlayerCameraController : MonoBehaviour
{
    [SerializeField] private C_PlayerGroundDetector m_Grounded;
    [SerializeField] private C_CameraFollower m_Camera;

    [Header("Grounded")]
    [SerializeField] private float m_SizeGrounded = 2f;
    [SerializeField] private float m_RotSpeedGround = 5f;

    [Header("Air")]
    [SerializeField] private float m_SizeAir = 5f;
    [SerializeField] private float m_RotSpeedAir = 0f;

    private void Awake()
    {
        if (m_Camera == null) m_Camera = FindAnyObjectByType<C_CameraFollower>();
        if (m_Grounded == null) m_Grounded = GetComponent<C_PlayerGroundDetector>();
    }

    private void OnEnable() => m_Grounded.A_OnGroundedChanged += Apply;
    private void OnDisable() => m_Grounded.A_OnGroundedChanged -= Apply;

    private void Apply(bool grounded)
    {
        m_Camera.CameraSize = grounded ? m_SizeGrounded : m_SizeAir;
        //Debug.Log("Camera Rotation is " + grounded);
        m_Camera.RotationSpeed = grounded ? m_RotSpeedGround : m_RotSpeedAir;
    }
}
