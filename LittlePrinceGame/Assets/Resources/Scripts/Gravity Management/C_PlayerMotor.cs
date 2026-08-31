using UnityEngine;

public class C_PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private C_GravObject m_GravObject;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private C_CameraFollower m_CameraManager;

    [Header("Grounded Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_WalkAcceleration = 10f;
    [SerializeField] private float m_Deceleration = 3f;

    [Header("Ungrounded Settings")]
    [SerializeField] private float m_AirMaxMoveSpeed = 10f;
    [SerializeField] private float m_AirAcceleration = 5f;
    private bool m_isPreparingLaunch = false;
    [SerializeField] private float m_LauchForce = 20f;

    [Header("Testing")]
    [SerializeField] private Vector2 m_MoveInput;
    [SerializeField] private bool m_JumpInput;
    private bool m_jumpBuffer;
    [SerializeField] private float m_JumpForce = 8f;
    [SerializeField] private bool m_IsGrounded;
    [SerializeField] private float m_GroundedRayDistance;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private GameObject m_NearestPlanet;
    //[SerializeField] private Vector2 m_DirectionToNearestPlanet;

    [Header("Inputs")]
    // los límites de izquierda y derecha de la pantalla para determinar sí el jugador se movería a la izquierda o la derecha
    [SerializeField] private float m_screenTouchMoveLimit;
    private bool m_inptIsTouching;
    private GameObject m_inptTouchDownObj;
    private GameObject m_inptTouchUpdateObj;
    private GameObject m_inptTouchUpObj;

    void Start()
    {
        m_GravObject = GetComponent<C_GravObject>();
        m_rb = GetComponent<Rigidbody2D>();
        m_CameraManager = FindAnyObjectByType<C_CameraFollower>();
    }

    void Update()
    {
        m_MoveInput = C_PlayerInput.Instance.MovementVector;
        m_JumpInput = C_PlayerInput.Instance.JumpBool;

        m_inptIsTouching = C_TouchInputManager.Instance.IsTouchPressing;
        m_inptTouchDownObj = C_TouchInputManager.Instance.TouchDownObj;
        m_inptTouchUpdateObj = C_TouchInputManager.Instance.TouchFollower;
        m_inptTouchUpObj = C_TouchInputManager.Instance.TouchUpObj;

        //print(m_JumpInput);
        if (m_JumpInput)
        {
            m_jumpBuffer = true;
        }
        if (m_IsGrounded)
        {
            Grounded();
        } else
        {
            NotGrounded();
        }
        GetNearestPlanetOrMoreRecent();
    }

    void FixedUpdate()
    {
        if (m_rb == null)
        {
            Debug.LogWarning("PlayerMotor: Rigidbody2D no asignado.");
            return;
        }

        if (m_IsGrounded)
        {
            GroundedBody();
        }
        else
        {
            NotGroundedBody();
        }
        if (m_jumpBuffer)
        {
            m_rb.AddForce(transform.up * m_JumpForce, ForceMode2D.Impulse);
            m_jumpBuffer = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_IsGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_IsGrounded = false;
        }*/
    }

    private bool CheckGrounded()
    {
        Vector2 origin = transform.position;
        //Vector2 dir = -transform.up;
        Vector2 dir = (new Vector2
            (m_NearestPlanet.transform.position.x, 
            m_NearestPlanet.transform.position.y) 
            - origin).normalized;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, m_GroundedRayDistance,m_GroundLayer);
        Debug.DrawRay(origin, dir * m_GroundedRayDistance, Color.red);
        return hit.collider != null;
    }
    private void Grounded()
    {
        m_CameraManager.CameraSize = 2f;
        m_CameraManager.RotationSpeed = 5f;
        m_IsGrounded = CheckGrounded();
    }
    private void NotGrounded()
    {
        m_CameraManager.CameraSize = 5f;
        m_CameraManager.RotationSpeed = 0f;
    }
    private void GroundedBody()
    {
        RotateTowardsPlanet();
        GroundedMovement();
    }
    private void NotGroundedBody()
    {
        FlyMovement();
    }
    private void GetNearestPlanetOrMoreRecent()
    {
        for (int i = 0; i < m_GravObject.PlanetsList.Count; i++)
        {
            if (Vector2.Distance(transform.position, m_GravObject.PlanetsList[i].transform.position) <= m_GravObject.PlanetsList[i].GravityRadius)
            {
                m_NearestPlanet = m_GravObject.PlanetsList[i].gameObject;
                break;
            }
        }
    }
    private void RotateTowardsPlanet()
    {
        Vector2 NearestPlanetPosition = m_NearestPlanet.transform.position;
        Vector2 Origin = new Vector2(transform.position.x,transform.position.y);
        Vector2 DirectionToPlanet = (NearestPlanetPosition - Origin).normalized;
        float angle = Mathf.Atan2(DirectionToPlanet.y, DirectionToPlanet.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        //m_IsGrounded = true;
        /*
        // The closest planet is the one that changes the rotation of the player, so it will always hit the planet on his feet
        for (int i = 0; i < m_GravObject.PlanetsList.Count; i++)
        {
            if (Vector2.Distance(transform.position, m_GravObject.PlanetsList[i].transform.position) <= m_GravObject.PlanetsList[i].GravityRadius)
            {
                m_NearestPlanet = m_GravObject.PlanetsList[i].gameObject;
                Vector2 DirectionToPlanet = (m_GravObject.PlanetsList[i].transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(DirectionToPlanet.y, DirectionToPlanet.x) * Mathf.Rad2Deg + 90f;
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
        */
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
    private void FlyMovement()
    {
        Debug.Log("Puedo volar");
        if (m_inptIsTouching)
        {
            m_rb.linearVelocity = new Vector2(0, 0);
            m_isPreparingLaunch = true;
            Debug.Log("Preparando un lanzamiento");
        }
        if (!m_inptIsTouching && m_isPreparingLaunch)
        {
            m_rb.AddForce(
                ((m_inptTouchUpObj.transform.position -
                m_inptTouchDownObj.transform.position).normalized
                * -1)
                * m_LauchForce
                ,ForceMode2D.Force);
            m_isPreparingLaunch = false;
        }

    }
}
