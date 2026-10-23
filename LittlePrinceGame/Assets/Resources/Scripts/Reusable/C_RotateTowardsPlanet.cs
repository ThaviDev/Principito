using UnityEngine;

[ExecuteAlways]
public class C_RotateTowardsPlanet : MonoBehaviour
{
    [SerializeField] C_PlanetGrav m_NearestPlanet;
    [SerializeField] bool m_CanRotate = true;
    [SerializeField] bool m_Update;

    public bool CanRotate { set { m_CanRotate = value; } }
    private void OnValidate()
    {
        if(m_CanRotate == false) return;
        AplicarRotacion();
        m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
    }
    private void Start()
    {
        if (m_CanRotate == false) return;
        if (m_NearestPlanet == null && !m_Update)
        {
            m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
            AplicarRotacion();
        }
    }
    private void Update()
    {
        if (m_CanRotate == false) return;
        if (m_NearestPlanet == null) 
            m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
        if (m_Update)
        {
            AplicarRotacion();
            m_NearestPlanet = C_PlanetUtility.GetNearestPlanet(transform.position);
        }
    }
    private void AplicarRotacion()
    {
        if (m_NearestPlanet == null) return;
        Vector2 direction = m_NearestPlanet.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }
}
