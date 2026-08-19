using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class C_PlanetChunk : MonoBehaviour
{
    [SerializeField] private C_PlanetGrav[] m_Planets;
    [SerializeField] private float m_ChunkRadius;
    public ContactFilter2D contactFilter;

    private CircleCollider2D m_trigger;

    private void OnValidate()
    {
        // keep inspector radius in sync with collider in edit mode
        m_trigger = GetComponent<CircleCollider2D>();
        if (m_trigger != null)
        {
            m_trigger.isTrigger = true;
            m_trigger.radius = m_ChunkRadius;
        }
    }

    void Start()
    {
        m_trigger = GetComponent<CircleCollider2D>();
        if (m_trigger == null)
            m_trigger = gameObject.AddComponent<CircleCollider2D>();

        m_trigger.isTrigger = true;
        m_trigger.radius = m_ChunkRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var gravObj = other.GetComponent<C_GravObject>();
        if (gravObj != null && m_Planets != null && m_Planets.Length > 0)
        {
            gravObj.AddPlanets(m_Planets);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var gravObj = other.GetComponent<C_GravObject>();
        if (gravObj != null && m_Planets != null && m_Planets.Length > 0)
        {
            gravObj.RemovePlanets(m_Planets);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_ChunkRadius);
    }
}
