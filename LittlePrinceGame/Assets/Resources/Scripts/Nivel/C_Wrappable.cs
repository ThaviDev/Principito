using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class C_Wrappable : MonoBehaviour
{
    [SerializeField] private bool m_IsPlayer = false;
    private Rigidbody2D m_rb;
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (LevelBounds.Size == Vector2.zero) return;
    }
}
