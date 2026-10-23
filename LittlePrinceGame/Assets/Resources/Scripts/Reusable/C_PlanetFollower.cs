using UnityEngine;

// Si se necesita acceso continuo al planeta (jugador, cámara, linterna, etc.).
// Si solo lo necesitas una vez, usa C_PlanetUtility directamente.
public class C_PlanetFollower : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Cada cuántos segundos recalcula. 0.1 = 10 veces/s. Baja el valor solo si es necesario.")]
    [SerializeField] private float m_UpdateInterval = 0.1f;

    [Tooltip("Si está activo, solo devuelve planetas cuyo GravityRadius contenga al objeto.")]
    [SerializeField] private bool m_RequireInGravityRadius = true;

    // Para usarlo en otros codigos: m_nearestPlanet = m_PlanetFollower.NearestPlanet
    //=======================================
    public C_PlanetGrav NearestPlanet { get; private set; }
    //=======================================
    public Vector2 DirectionToPlanet =>
        NearestPlanet != null
            ? C_PlanetUtility.DirectionTo(NearestPlanet, transform.position)
            : Vector2.zero;

    private float m_Timer;

    private void OnEnable() => Refresh();

    private void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer < m_UpdateInterval) return;
        m_Timer = 0f;
        Refresh();
    }

    public void Refresh()
    {
        NearestPlanet = m_RequireInGravityRadius
            ? C_PlanetUtility.GetNearestPlanetInGravRadius(transform.position)
            : C_PlanetUtility.GetNearestPlanet(transform.position);
    }
}