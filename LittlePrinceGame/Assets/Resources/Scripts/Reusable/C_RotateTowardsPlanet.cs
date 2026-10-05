using UnityEngine;

[ExecuteAlways]
public class C_RotateTowardsPlanet : MonoBehaviour
{
    [SerializeField] C_PlanetGrav m_Planet;
    private void OnValidate()
    {
        AplicarRotacion();
    }
    private void Update()
    {
        AplicarRotacion();
    }
    private void AplicarRotacion()
    {
        if (m_Planet == null) return;
        Vector2 direction = m_Planet.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }
}
