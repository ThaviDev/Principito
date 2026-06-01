using UnityEngine;

public class C_PlanetGrav : MonoBehaviour
{
    [SerializeField] private float m_GravityStrength = 9.81f;
    public float GravityStrength { get { return m_GravityStrength; } }
    [SerializeField] private float m_GravityRadius = 5f;
    public float GravityRadius { get { return m_GravityRadius; } }
    [SerializeField] private TAG_Atmosphere m_Atmosphere;
    void Start()
    {
        m_Atmosphere = GetComponentInChildren<TAG_Atmosphere>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Atmosphere.transform.localScale = Vector3.one * m_GravityRadius * 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_GravityRadius);
    }
}
