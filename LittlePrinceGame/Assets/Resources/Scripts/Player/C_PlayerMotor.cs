using UnityEngine;

public class C_PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private C_GravObject m_GravObject;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private C_CameraFollower m_CameraManager;
    [SerializeField] private C_PlayerAnimation m_PlayerAnim;
    [SerializeField] private C_GameplayUI m_GameplayUI;

    [Header("Grounded Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private float m_WalkAcceleration = 10f;
    [SerializeField] private float m_Deceleration = 3f;
    [SerializeField] private float m_CameraSizeGrounded = 2f;
    [SerializeField] private float m_CameraRotationSpeedGrounded = 5f;

    [Header("Ungrounded Settings")]
    [SerializeField] private float m_AirMaxMoveSpeed = 10f;
    [SerializeField] private float m_AirAcceleration = 5f;
    private bool m_isPreparingLaunch = false;
    [SerializeField] private float m_LauchForce = 20f;
    [SerializeField] private float m_CameraSizeUngrounded = 5f;
    [SerializeField] private float m_CameraRotationSpeedUngrounded = 0f;

    [Header("Settings")]
    private bool m_jumpBuffer;
    [SerializeField] private float m_JumpForce = 8f;
    [SerializeField] private bool m_IsGrounded;
    [SerializeField] private float m_GroundedRayDistance;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private GameObject m_NearestPlanet;
    [SerializeField] private float m_RotationSpeed = 5f;
    //[SerializeField] private Vector2 m_DirectionToNearestPlanet;

    [Header("Inputs")]
    [SerializeField] private Vector2 m_MoveInput;
    [SerializeField] private bool m_JumpInput;
    // los límites de izquierda y derecha de la pantalla para determinar sí el jugador se movería a la izquierda o la derecha
    [SerializeField] private float m_screenTouchMoveLimit;
    private bool m_inptIsTouching;
    private GameObject m_inptTouchDownObj;
    private GameObject m_inptTouchUpdateObj;
    private GameObject m_inptTouchUpObj;
    private bool m_IsLaunching;
    [SerializeField]private GameObject m_touchedObject;

    void Start()
    {
        m_GravObject = GetComponent<C_GravObject>();
        m_rb = GetComponent<Rigidbody2D>();
        m_CameraManager = FindAnyObjectByType<C_CameraFollower>();
        m_PlayerAnim = GetComponentInChildren<C_PlayerAnimation>();
        m_rb.freezeRotation = true;
    }

    void Update()
    {
        //m_MoveInput = C_PlayerInput.Instance.MovementVector;
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
            C_PlayerAnimation.SetAnimationState?.Invoke(PlayerAnimationStates.fly);
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
        if (m_inptIsTouching)
        {
            m_touchedObject = CheckTouchInteraction();
            if (m_touchedObject != null)
            {
                // Handle the touched object
                m_IsLaunching = true;
            }
            else
            {
                m_IsLaunching = false;
            }
        }
        if (m_IsLaunching)
        {
            Debug.Log("Launching object or player: " + m_touchedObject.name);
            LaunchObjectsAndPlayer();
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
    public void InputMoveRight()
    {
        Debug.Log("InputMoveRight");
        m_MoveInput = Vector2.right;
    }
    public void InputMoveLeft()
    {
        Debug.Log("InputMoveLeft");
        m_MoveInput = Vector2.left;
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
    private GameObject CheckTouchInteraction()
    {
        if (!m_IsGrounded)
        {
            return gameObject; // Return the player object if not grounded
        }
        Vector2 origin = m_inptTouchDownObj.transform.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(origin, Vector2.zero);
        for (int i = 0; i < hit.Length; i++)
        {
            GameObject myObject = hit[i].collider.gameObject;
            Debug.Log("Touched object: " + hit[i].collider.gameObject.name);
            if (myObject.GetComponent<Rigidbody2D>() != null)
            {
                Debug.Log("Object has Rigidbody2D: " + myObject.name);
                return myObject;
            }
        }
        Debug.Log("No object touched.");
        return null;
    }
    private void Grounded()
    {
        m_CameraManager.CameraSize = m_CameraSizeGrounded;
        m_CameraManager.RotationSpeed = m_CameraRotationSpeedGrounded;
        m_IsGrounded = CheckGrounded();
        m_GameplayUI.A_SwitchWalkArrows?.Invoke(true);
    }
    private void NotGrounded()
    {
        m_CameraManager.CameraSize = m_CameraSizeUngrounded;
        m_CameraManager.RotationSpeed = m_CameraRotationSpeedUngrounded;
        m_GameplayUI.A_SwitchWalkArrows?.Invoke(false);
        m_MoveInput = Vector2.zero;
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
        Vector2 localVelocity = transform.InverseTransformDirection(m_rb.linearVelocity);
        if (m_MoveInput.x > 0.1 && localVelocity.x > 0.1f)
        {
            m_PlayerAnim.FlipSprite(false);
        } else if (m_MoveInput.x < -0.1 && localVelocity.x < -0.1f)
        {
            m_PlayerAnim.FlipSprite(true);
        }

        if (m_rb.linearVelocity.magnitude < 0.1f)
        {
            C_PlayerAnimation.SetAnimationState?.Invoke(PlayerAnimationStates.idle);
        }
        else
        {
            C_PlayerAnimation.SetAnimationState?.Invoke(PlayerAnimationStates.walk);
        }
        if (m_inptIsTouching)
        {
            m_MoveInput = Vector2.zero;
        }

        /*
        if (m_inptIsTouching)
        {
            if (m_inptTouchUpdateObj.transform.position.x > m_screenTouchMoveLimit)
            {
                InputMoveRight();
            }
            else if (m_inptTouchUpdateObj.transform.position.x < -m_screenTouchMoveLimit)
            {
                InputMoveLeft();
            }
        }*/
    }

    private void DragObjectsWithMouse()
    {

    }
    private void FlyMovement()
    {
        Vector2 direction = (m_rb.linearVelocity).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleObjective = angle - 90f;
        m_rb.rotation = Mathf.LerpAngle(m_rb.rotation, angleObjective, Time.fixedDeltaTime * m_RotationSpeed);
    }
    private void LaunchObjectsAndPlayer()
    {
        Rigidbody2D touchedRb = m_touchedObject.GetComponent<Rigidbody2D>();
        //Debug.Log("Puedo volar");
        if (m_inptIsTouching)
        {
            touchedRb.linearVelocity = new Vector2(0, 0);
            m_isPreparingLaunch = true;
            //Debug.Log("Preparando un lanzamiento");
        }
        if (!m_inptIsTouching && m_isPreparingLaunch)
        {
            touchedRb.AddForce(
                ((m_inptTouchUpObj.transform.position -
                m_inptTouchDownObj.transform.position).normalized
                * -1)
                * m_LauchForce
                ,ForceMode2D.Force);
            m_isPreparingLaunch = false;
            m_IsLaunching = false;
        }
    }
}
