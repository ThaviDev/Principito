using UnityEngine;

public class C_GravObject : MonoBehaviour
{
    [SerializeField] private C_PlanetGrav[] m_Planets;
    [SerializeField] private Rigidbody2D m_rb;
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        foreach (C_PlanetGrav planet in m_Planets)
        {
            float dist = Vector2.Distance(transform.position, planet.transform.position);

            Vector2 v = planet.transform.position - transform.position;

            if (dist <= planet.GravityRadius)
            {
                m_rb.AddForce(v.normalized * planet.GravityStrength * (planet.GravityRadius / dist));
            }
        }
        /*
        float dist = Vector2.Distance(transform.position, m_Planet.transform.position);

        Vector2 v = m_Planet.transform.position - transform.position;

        if (dist <= m_GravityRadius)
        {
            m_rb.AddForce(v.normalized * m_GravityStrength * (m_GravityRadius / dist));
        }
        */
    }
}
