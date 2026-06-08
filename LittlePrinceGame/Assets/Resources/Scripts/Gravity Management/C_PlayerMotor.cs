using UnityEngine;
using UnityEngine.InputSystem;

public class C_PlayerMotor : MonoBehaviour
{
    [SerializeField] private C_GravObject m_GravObject;
    [SerializeField] private Rigidbody2D m_rb;
    //[SerializeField] private InputSystem_Actions m_PlayerInput;
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_WalkAcceleration = 10f;
    [SerializeField] private float m_Deceleration = 3f;
    [SerializeField] private Vector2 m_MoveInput;

    [SerializeField] private bool m_IsGrounded;
    void Start()
    {
        m_GravObject = GetComponent<C_GravObject>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        m_MoveInput = C_PlayerInput.Instance.MovementVector;
    }

    void FixedUpdate()
    {
        // The closest planet is the one that changes the rotation of the player, so it will always hit the planet on his feet
        for (int i = 0; i < m_GravObject.PlanetsList.Length; i++)
        {
            if (Vector2.Distance(transform.position, m_GravObject.PlanetsList[i].transform.position) <= m_GravObject.PlanetsList[i].GravityRadius)
            {
                Vector2 directionToPlanet = (m_GravObject.PlanetsList[i].transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg + 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                //m_IsGrounded = true;
                break;
            }
            else
            {
                //m_IsGrounded = false;
            }
        }


        //if (m_rb != null || m_MoveInput != Vector2.zero && m_IsGrounded)
        if (m_rb == null)
        {
            Debug.LogWarning("PlayerMotor: Rigidbody2D no asignado.");
            return;
        }

        if (m_IsGrounded)
        {
            GroundedMovement();
        }
        else
        {
            NotGroundedMovement();
        }
    }
    private void GroundedMovement()
    {
        Vector2 worldForce = (transform.TransformDirection(m_MoveInput) * m_MoveSpeed);
        m_rb.AddForce(worldForce * m_WalkAcceleration, ForceMode2D.Force);
        if (m_rb.linearVelocity.magnitude > m_MoveSpeed)
        {
            m_rb.linearVelocity = m_rb.linearVelocity.normalized * m_MoveSpeed;
        }
        else
        {
            m_rb.AddForce(m_rb.linearVelocity * -m_Deceleration, ForceMode2D.Force);
        }
    }
    private void NotGroundedMovement()
    {
        // Setting temporal para teclado, luego se cambiara a un flick de movil gesture.

    }
}
