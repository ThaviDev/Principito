using UnityEngine;

public class C_RotateAroundObj : MonoBehaviour
{
    [SerializeField] C_PlanetGrav m_NearestPlanet;
    [SerializeField] float m_maxSpeed;
    [SerializeField] float m_minSpeed;
    [SerializeField] bool m_DirectionIsLeft;
    [SerializeField] bool m_canRotate;
    [SerializeField] float m_orbitRadius;

    private float m_currentSpeed;
    private float m_angle;

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        //m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
        var nearest = C_PlanetUtility.GetNearestPlanet(transform.position);
        if (nearest != null) m_NearestPlanet = nearest;
    }
    void Start()
    {
        Vector2 delta = (Vector2)transform.position - (Vector2)m_NearestPlanet.transform.position;
        m_orbitRadius = delta.magnitude;
        m_angle = Mathf.Atan2(delta.y, delta.x);
        m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
        m_currentSpeed = Random.Range(m_minSpeed, m_maxSpeed);
    }
    void Update()
    {
        float angularSpeed = m_orbitRadius > 0.0001f ? m_currentSpeed / m_orbitRadius : 0f;
        float dir =  m_DirectionIsLeft ? 1f : -1f;
        m_angle += angularSpeed * dir * Time.deltaTime;

        Vector2 planetPos = m_NearestPlanet.transform.position;
        Vector2 offset = new Vector2(Mathf.Cos(m_angle), Mathf.Sin(m_angle)) * m_orbitRadius;
        transform.position = planetPos + offset;
    }
}
