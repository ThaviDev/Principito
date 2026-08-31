using UnityEngine;

public class C_TouchInputManager : MonoBehaviour
{   
    public static C_TouchInputManager Instance;

    private bool m_isTouchPressing;
    public bool IsTouchPressing { get { return m_isTouchPressing; } }

    [SerializeField] private GameObject m_touchDownObj;
    public GameObject TouchDownObj { get { return m_touchDownObj; } }
    [SerializeField] private GameObject m_touchFollower;
    public GameObject TouchFollower { get { return m_touchFollower; } }
    [SerializeField] private GameObject m_touchUpObj;
    public GameObject TouchUpObj { get { return m_touchUpObj; } }
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
        m_touchDownObj.transform.position = touchPos;

        //Debug.Log("Touch Down Triggered at position: " + touchPos);
    }
    public void TriggerTouchUpdate(Vector3 touchPos)
    {
        m_touchFollower.transform.position = touchPos;
        //Debug.Log("Touch Update Triggering in position: " + touchPos);
        m_isTouchPressing = true;
    }
    public void TriggerTouchUp(Vector3 touchPos)
    {
        m_touchUpObj.transform.position = touchPos;
        //Debug.Log("Touch Up Triggered at position: " + touchPos);
        m_isTouchPressing = false;
    }
    /*
    public void TriggerTouchScreenDown(Vector2 touchPos)
    {
        //Debug.Log("TouchScreen pos: " + touchPos);
        m_touchPositionUpdate = touchPos;
    }*/
}
