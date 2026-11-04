using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class C_LevelBounds : MonoBehaviour
{
    public static C_LevelBounds Instance { get; private set; }
    private BoxCollider2D m_Bounds;

    public float MinX => m_Bounds.bounds.min.x;
    public float MaxX => m_Bounds.bounds.max.x;
    public float MinY => m_Bounds.bounds.min.y;
    public float MaxY => m_Bounds.bounds.max.y;

    public float Width => m_Bounds.bounds.size.x;
    public float Height => m_Bounds.bounds.size.y;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        m_Bounds = GetComponent<BoxCollider2D>();
        m_Bounds.isTrigger = true; // Asegurarse de que no choque con nada
    }
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        if (m_Bounds == null) m_Bounds = GetComponent<BoxCollider2D>();
        if (m_Bounds != null)
        {
            Gizmos.color = Color.purple;
            Gizmos.DrawWireCube(m_Bounds.bounds.center, m_Bounds.bounds.size);
        }
    }
}
