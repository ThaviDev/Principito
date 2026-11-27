using UnityEngine;

public class C_RotateAroundObj : MonoBehaviour
{
    [SerializeField] C_PlanetGrav m_NearestPlanet;
    [SerializeField] float m_maxSpeed;
    [SerializeField] float m_minSpeed;
    [SerializeField] bool m_DirectionIsLeft;
    [SerializeField] bool m_canRotate;
    [SerializeField] float m_orbitRadius;
    [SerializeField] bool m_updateNearestPlanet;
    [SerializeField] bool m_useRigidBody;
    [SerializeField] Rigidbody2D m_rigidBody;
    private float m_currentSpeed;
    private float m_angle;

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (m_useRigidBody && m_rigidBody == null)
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
        }
        //m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
        var nearest = C_PlanetUtility.GetNearestPlanet(transform.position);
        if (nearest != null) m_NearestPlanet = nearest;
    }
    void Start()
    {
        if (m_updateNearestPlanet)
        m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);

        Vector2 delta = (Vector2)transform.position - (Vector2)m_NearestPlanet.transform.position;
        m_orbitRadius = delta.magnitude;
        m_angle = Mathf.Atan2(delta.y, delta.x);
        m_currentSpeed = Random.Range(m_minSpeed, m_maxSpeed);
    }

    private void FixedUpdate()
    {
        if (m_NearestPlanet == null || !m_canRotate) return;
        Vector2 nextPosition = GetNextPosition();

        if (m_useRigidBody && m_rigidBody != null)
        {
            m_rigidBody.MovePosition(nextPosition);
        }
        else
        {
            transform.position = nextPosition;
        }
    }
    Vector2 GetNextPosition()
    {
        float angularSpeed = m_orbitRadius > 0.0001f ? m_currentSpeed / m_orbitRadius : 0f;
        float dir = m_DirectionIsLeft ? 1f : -1f;
        m_angle += angularSpeed * dir * Time.fixedDeltaTime;

        Vector2 planetPos = m_NearestPlanet.transform.position;
        Vector2 offset = new Vector2(Mathf.Cos(m_angle), Mathf.Sin(m_angle)) * m_orbitRadius;
        //transform.position = planetPos + offset;
        return planetPos + offset;
    }
}
