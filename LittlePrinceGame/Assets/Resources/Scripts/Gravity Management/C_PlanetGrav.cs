using UnityEngine;

public class C_PlanetGrav : MonoBehaviour
{
    [SerializeField] private float m_PlanetSize = 1f;
    [SerializeField] private float m_GravityStrength = 9.81f;
    public float GravityStrength { get { return m_GravityStrength; } }
    [SerializeField] private float m_GravityRadius = 5f;
    public float GravityRadius { get { return m_GravityRadius; } }
    [SerializeField] private TAG_Atmosphere m_Atmosphere;
    [SerializeField] private TAG_PlanetVisual m_PlanetVisual;
    [SerializeField] private CircleCollider2D m_PlanetSurfaceCollider;
    // On Validate Sirve para que cuando se cambien los valores en el inspector,
    // se actualicen las escalas y radios de los componentes relacionados,
    // como la visualización del planeta, el collider de la superficie y la atmósfera.
    // Esto asegura que cualquier cambio en el tamaño del planeta o el radio de gravedad se refleje inmediatamente en la escena,
    // facilitando el diseño y ajuste del planeta sin necesidad de ejecutar el juego.
    private void OnValidate()
    {
        m_Atmosphere = GetComponentInChildren<TAG_Atmosphere>();
        m_PlanetVisual = GetComponentInChildren<TAG_PlanetVisual>();
        m_PlanetSurfaceCollider = GetComponent<CircleCollider2D>();
        m_PlanetVisual.transform.localScale = Vector3.one * m_PlanetSize;
        m_PlanetSurfaceCollider.radius = m_PlanetSize / 2f;
        m_Atmosphere.transform.localScale = Vector3.one * m_GravityRadius * 2f;
    }
    void Start()
    {
        m_Atmosphere = GetComponentInChildren<TAG_Atmosphere>();
        m_PlanetVisual = GetComponentInChildren<TAG_PlanetVisual>();
        m_PlanetSurfaceCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_PlanetVisual.transform.localScale = Vector3.one * m_PlanetSize;
        m_PlanetSurfaceCollider.radius = m_PlanetSize / 2f;
        m_Atmosphere.transform.localScale = Vector3.one * m_GravityRadius * 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_GravityRadius);
    }
}
