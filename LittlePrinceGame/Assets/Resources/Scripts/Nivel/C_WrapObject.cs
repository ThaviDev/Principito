using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class C_WrapObject : MonoBehaviour
{
    [Tooltip("Margen extra para que el objeto no se teletransporte exactamente en el borde y cause parpadeos.")]
    [SerializeField] private float m_Padding = 0.1f;
    private Rigidbody2D m_Rb;
    void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        // Si no existe un LevelBounds en la escena, no hacemos nada
        if (C_LevelBounds.Instance == null) return;

        Vector2 pos = m_Rb.position;

        // --- EJE X (Izquierda / Derecha) ---
        if (pos.x > C_LevelBounds.Instance.MaxX + m_Padding)
        {
            // Salimos por la derecha -> Aparecemos por la izquierda
            pos.x = C_LevelBounds.Instance.MinX - m_Padding;
        }
        else if (pos.x < C_LevelBounds.Instance.MinX - m_Padding)
        {
            // Salimos por la izquierda -> Aparecemos por la derecha
            pos.x = C_LevelBounds.Instance.MaxX + m_Padding;
        }

        // --- EJE Y (Arriba / Abajo) ---
        if (pos.y > C_LevelBounds.Instance.MaxY + m_Padding)
        {
            // Salimos por arriba -> Aparecemos por abajo
            pos.y = C_LevelBounds.Instance.MinY - m_Padding;
        }
        else if (pos.y < C_LevelBounds.Instance.MinY - m_Padding)
        {
            // Salimos por abajo -> Aparecemos por arriba
            pos.y = C_LevelBounds.Instance.MaxY + m_Padding;
        }

        // Aplicamos la nueva posición al Rigidbody
        m_Rb.position = pos;
    }
}
