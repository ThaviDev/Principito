using UnityEngine;

public class C_TouchInputManager : MonoBehaviour
{   
    public static C_TouchInputManager Instance;

    [System.Serializable]
    public struct TouchGesture
    {
        public float Duration;   // segundos que duro el toque
        public float Distance;   // distancia total down->up (misma unidad que uses)
        public Vector2 StartPos;
        public Vector2 EndPos;

        /// <summary>Velocidad promedio del gesto. Unidad/s.</summary>
        public float Speed => Duration > 0.0001f ? Distance / Duration : 0f;
    }

    private bool m_isTouchPressing;
    private float m_touchStartTime;
    private Vector2 m_touchStartPos;
    private Vector2 m_touchCurrentPos;
    //public bool IsTouchPressing { get { return m_isTouchPressing; } }

    public bool IsTouchPressing => m_isTouchPressing;

    [SerializeField] private GameObject m_touchDownObj;
    [SerializeField] private GameObject m_touchFollower;
    [SerializeField] private GameObject m_touchUpObj;
    public GameObject TouchDownObj { get { return m_touchDownObj; } }
    public GameObject TouchFollower { get { return m_touchFollower; } }
    public GameObject TouchUpObj { get { return m_touchUpObj; } }
    public TouchGesture LastGesture { get;private set; }
    //public float TouchDuration { get { return m_isTouchPressing ? (Time.unscaledTime - m_touchStartTime) : LastGesture.Duration; } }
    public float TouchDuration => m_isTouchPressing ? (Time.unscaledTime - m_touchStartTime) : LastGesture.Duration;

    //public float TouchDistance { get { return m_isTouchPressing ? Vector2.Distance(m_touchStartPos, m_touchCurrentPos) : LastGesture.Distance; } }
    public float TouchDistance => m_isTouchPressing ? Vector2.Distance(m_touchStartPos, m_touchCurrentPos) : LastGesture.Distance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (m_touchFollower != null)
        {
            m_touchFollower = Instantiate(m_touchFollower, transform);
        }
    }
    public void TriggerTouchDown(Vector3 touchPos)
    {
        m_touchStartTime = Time.unscaledTime;
        m_touchStartPos = touchPos;
        m_touchCurrentPos = touchPos;

        if (m_touchDownObj != null)
        {
            m_touchDownObj.transform.position = touchPos;
        }
        //Debug.Log("Touch Down Triggered at position: " + touchPos);
    }
    public void TriggerTouchUpdate(Vector3 touchPos)
    {
        m_touchCurrentPos = touchPos;
        if (m_touchFollower != null) m_touchFollower.transform.position = touchPos;
        //Debug.Log("Touch Update Triggering in position: " + touchPos);
        m_isTouchPressing = true;
    }
    public void TriggerTouchUp(Vector3 touchPos)
    {
        m_touchCurrentPos = touchPos;
        if (m_touchUpObj != null) m_touchUpObj.transform.position = touchPos;
        //Debug.Log("Touch Up Triggered at position: " + touchPos);
        m_isTouchPressing = false;

        // Calculo de gesto completo
        CalculateLastGesture(touchPos);
    }

    private void CalculateLastGesture(Vector3 touchUpPosition)
    {
        LastGesture = new TouchGesture
        {
            Duration = Time.unscaledTime - m_touchStartTime,
            Distance = Vector2.Distance(m_touchStartPos, touchUpPosition),
            StartPos = m_touchStartPos,
            EndPos = touchUpPosition
        };
    }
    /*
    public void TriggerTouchScreenDown(Vector2 touchPos)
    {
        //Debug.Log("TouchScreen pos: " + touchPos);
        m_touchPositionUpdate = touchPos;
    }*/
}
