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

    // Timestamp hasta el cual ignoramos la detección de suelo.
    private float m_SuppressUntil = -1f;

    private void Awake()
    {
        if (m_PlanetFollower == null) 
            m_PlanetFollower = GetComponent<C_PlanetFollower>();
    }

    private void Update()
    {
        // Modo suprimido: forzamos "no grounded" sin importar el raycast.
        if (Time.time < m_SuppressUntil)
        {
            SetGrounded(false);
            return;
        }

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

        SetGrounded(grounded);
        /*
        if (grounded != IsGrounded)
        {
            IsGrounded = grounded;
            A_OnGroundedChanged?.Invoke(grounded);
        } */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Durante la supresión ignoramos también colisiones para no
        // "re-engancharnos" al suelo mientras despegamos.
        if (Time.time < m_SuppressUntil) return;
        if ((m_GroundLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        var planet = collision.gameObject.GetComponent<C_PlanetGrav>();
        if (planet != null && planet.IsWalkable && !IsGrounded)
        {
            IsGrounded = true;
            A_OnGroundedChanged.Invoke(true);
        }
    }

    /// <summary>
    /// Ignora la detección de suelo durante <paramref name="duration"/> segundos
    /// y fuerza IsGrounded = false inmediatamente. Útil justo después de un
    /// lanzamiento para que el movimiento en suelo no frene el despegue.
    /// </summary>
    public void SuppressGroundDetection(float duration)
    {
        m_SuppressUntil = Time.time + duration;
        SetGrounded(false);
    }

    private void SetGrounded(bool value)
    {
        if (value == IsGrounded) return;
        IsGrounded = value;
        A_OnGroundedChanged?.Invoke(value);
    }
}
