using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class C_PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private C_PlayerAnimation m_Anim;
    [SerializeField] private C_RotateTowardsPlanet m_RotToPlanet;
    [Header("Config")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_Acceleration = 10f;
    [SerializeField] private float m_Deceleration = 3f;
    [SerializeField] private float m_AirRotationSpeed = 5f;

    private void Awake()
    {
        if (m_Anim == null) m_Anim = GetComponent<C_PlayerAnimation>();
        if (m_rb == null) m_rb = GetComponent<Rigidbody2D>();
        if (m_RotToPlanet == null) m_RotToPlanet = GetComponent<C_RotateTowardsPlanet>();
    }

    public void Grounded(Vector2 input)
    {
        m_RotToPlanet.CanRotate = true;

        Vector2 worldForce = transform.TransformDirection(input) * m_MoveSpeed;
        m_rb.AddForce(worldForce * m_Acceleration, ForceMode2D.Force);

        if (m_rb.linearVelocity.magnitude > m_MoveSpeed)
            m_rb.linearVelocity = m_rb.linearVelocity.normalized * m_MoveSpeed;
        else
            m_rb.AddForce(m_rb.linearVelocity * -m_Deceleration, ForceMode2D.Force);

        // Flip Sprite
        Vector2 localVel = transform.InverseTransformDirection(m_rb.linearVelocity);
        if (input.x > 0.1f && localVel.x > 0.1f)
            m_Anim.FlipSprite(false);
        else if (input.x < -0.1f &&  localVel.x < -0.1f)
            m_Anim.FlipSprite(true);

        m_Anim.SetAnimationState?.Invoke(m_rb.linearVelocity.magnitude < 0.1f ? 
            PlayerAnimationStates.idle : 
            PlayerAnimationStates.walk);
    }

    public void Fly()
    {
        m_RotToPlanet.CanRotate = false;

        // Rotacion hacia donde se dirije
        if (m_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = m_rb.linearVelocity.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            m_rb.rotation = Mathf.LerpAngle(m_rb.rotation, angle, Time.fixedDeltaTime * m_AirRotationSpeed);
        }
        m_Anim.SetAnimationState?.Invoke(PlayerAnimationStates.fly);
    }
}
