using System.Collections.Generic;
using UnityEngine;

public class C_GravObject : MonoBehaviour
{
    [SerializeField] private List<C_PlanetGrav> m_Planets = new List<C_PlanetGrav>();
    public List<C_PlanetGrav> PlanetsList { get { return m_Planets; } }

    [SerializeField] private Rigidbody2D m_rb;
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    public void AddPlanets(IEnumerable<C_PlanetGrav> planets)
    {
        if (planets == null) return;
        foreach (var p in planets)
        {
            if (p != null && !m_Planets.Contains(p))
                m_Planets.Add(p);
        }
    }

    public void RemovePlanets(IEnumerable<C_PlanetGrav> planets)
    {
        if (planets == null) return;
        foreach (var p in planets)
        {
            if (p == null) continue;
            m_Planets.Remove(p);
        }
    }

    void FixedUpdate()
    {
        if (m_Planets == null || m_Planets.Count == 0) return;

        foreach (C_PlanetGrav planet in m_Planets)
        {
            if (planet == null) continue;

            float dist = Vector2.Distance(transform.position, planet.transform.position);

            Vector2 v = planet.transform.position - transform.position;

            if (dist <= planet.GravityRadius)
            {
                m_rb.AddForce(v.normalized * planet.GravityStrength * (planet.GravityRadius / dist));
            }
        }
    }
}
