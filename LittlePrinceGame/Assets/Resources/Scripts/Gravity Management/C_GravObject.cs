using UnityEngine;

public class C_GravObject : MonoBehaviour
{
    [SerializeField] private C_PlanetGrav[] m_Planets;
    public C_PlanetGrav[] PlanetsList { get => m_Planets; set => m_Planets = value; }
    [SerializeField] private Rigidbody2D m_rb;
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
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
    }
}
