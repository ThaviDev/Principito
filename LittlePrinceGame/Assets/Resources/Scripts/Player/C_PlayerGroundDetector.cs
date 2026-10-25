using System;
using UnityEngine;

[RequireComponent(typeof(C_PlanetFollower))]
public class C_PlayerGroundDetector : MonoBehaviour
{
    [SerializeField] private float m_RayDistance = 0.5f;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private C_PlanetFollower m_PlanetFollower;

    public bool IsGrounded { get; private set; }
    public event Action<bool> A_OnGroundedChanged;

    private void Awake()
    {
        if (m_PlanetFollower == null) 
            m_PlanetFollower = GetComponent<C_PlanetFollower>();
    }

    private void Update()
    {
        var planet = m_PlanetFollower.NearestPlanet;
        bool grounded = false;

        if (planet != null)
        {
            Vector2 origin = transform.position;
            Vector2 dir = C_PlanetUtility.DirectionTo(planet, origin);
            var hit = Physics2D.Raycast(origin, dir, m_RayDistance, m_GroundLayer);
            grounded = hit.collider != null;
            Debug.DrawRay(origin, dir * m_RayDistance, Color.red);
        }

        if (grounded != IsGrounded)
        {
            IsGrounded = grounded;
            A_OnGroundedChanged?.Invoke(grounded);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((m_GroundLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        var planet = collision.gameObject.GetComponent<C_PlanetGrav>();
        if (planet != null && planet.IsWalkable && !IsGrounded)
        {
            IsGrounded = true;
            A_OnGroundedChanged.Invoke(true);
        }
    }
}
