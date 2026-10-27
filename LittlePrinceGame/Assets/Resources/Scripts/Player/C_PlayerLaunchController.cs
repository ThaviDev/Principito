using System;
using UnityEngine;

public class C_PlayerLaunchController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_BaseLaunchForce = 200f;
    [SerializeField] private float m_SelfLiftForce = 300f;
    [Tooltip("Multiplicador mínimo cuando el gesto de distancia es muy débil o muy largo.")]
    [SerializeField, Range(0f, 1f)] private float m_MinDistForceMult = 0.3f;
    [Tooltip("Multiplicador mínimo cuando el gesto de tiempo es muy débil o muy largo.")]
    [SerializeField, Range(0f, 1f)] private float m_MinTimeForceMult = 0.6f;
    [Tooltip("Distancia que se considera 'swipe completo'. Debe estar en la MISMA unidad que TouchDistance.")]
    [SerializeField] private float m_MaxSwipeDistance = 300f;
    [Tooltip("Duración máxima considerada. A partir de aquí la fuerza es la mínima posible.")]
    [SerializeField] private float m_MaxHoldDuration = 1.5f;
    [Tooltip("Maxima cantidad de potencia que puede tener un lanzamiento")]
    [SerializeField] private float m_MaxLaunchPower = 500f;

    [Header("References")]
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private C_PlayerAnimation m_Anim;

    [Header("Testing")]
    [SerializeField] GameObject m_Target;
    public bool IsPreparing { get; private set; }
    public event Action A_OnLaunchStart;
    public event Action A_OnLaunchRelease;

    private void Awake()
    {
        if (m_Anim == null)
            m_Anim = GetComponentInChildren<C_PlayerAnimation>();
        if (m_rb == null)
            m_rb = GetComponent<Rigidbody2D>();
    }

    public void TryBegin(Vector2 touchWorldPos, bool isGrounded)
    {
        if (IsPreparing) return;

        GameObject target = isGrounded ? FindTouchTarget(touchWorldPos) : this.gameObject;
        if (target == null) return;

        var rb = target.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        m_Target = target;

        if (target == this.gameObject)
        {
            /* Checar si esta en el suelo, si si entonces hay que hacer un detonador que cheque durante el lanzamiento
             * si el angulo es mejor para otra cosa, o sera mejor detectar ese angulo de antemano, quiza sea necesario
             * checar los mismos "gestures", pero en vez de con el punto inicio y final con el punto inicio y de update
             * para simular un tipo de Joystick
             * */
            m_Anim.SetAnimationState?.Invoke(PlayerAnimationStates.spinning);
            if (isGrounded) rb.AddRelativeForce(Vector2.up * m_SelfLiftForce);
        }

        rb.linearVelocity = Vector2.zero;
        IsPreparing = true;
        A_OnLaunchStart?.Invoke();
    }
    public void Preparing()
    {
        if (!IsPreparing || m_Target != null) return;
        var launchRb = m_Target.GetComponent<Rigidbody2D>();
        if (launchRb != null) launchRb.linearVelocity = Vector2.zero;
        print("Objeto seleccionado esta en velocidad 0");
    }
    public void TryRelease(Vector2 fromWorld, Vector2 toWorld)
    {
        if (!IsPreparing && m_Target == null) { Reset(); return; }

        var launchRb = m_Target.GetComponent<Rigidbody2D>();
        if (launchRb == null) { Reset(); return; }

        var gesture = C_TouchInputManager.Instance.LastGesture;
        float force = CalculateLaunchForce(gesture);

        Vector2 dir = fromWorld - toWorld;

        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
        dir.Normalize();

        launchRb.AddForce(dir * force, ForceMode2D.Force);
        Debug.Log("Final Force: " + force);

        //if (m_Target == this.gameObject)
            //m_Anim.SetAnimationState?.Invoke(PlayerAnimationStates.fly);

        Reset();
        A_OnLaunchRelease?.Invoke();
    }

    private void Reset()
    {
        m_Target = null;
        IsPreparing = false;
    }
    private GameObject FindTouchTarget(Vector2 worldPos)
    {
        var hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
        foreach(var hit in hits)
        {
            if (hit.collider.GetComponent<Rigidbody2D>() != null)
            {
                Debug.Log("Objeto " + hit + " tiene RigidBody2D");
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    private float CalculateLaunchForce(C_TouchInputManager.TouchGesture gesture)
    {
        // -- Distancia: 0 -> min, max -> 1
        float distNorm = Mathf.Clamp01(gesture.Distance / m_MaxSwipeDistance);
        float distFactor = Mathf.Lerp(m_MinDistForceMult, 1f, distNorm);

        // -- Duración: 0 -> 1, max -> min (mas tiempo = menos fuerza)
        float timeNorm = Mathf.Clamp01(gesture.Duration / m_MaxHoldDuration);
        float timeFactor = Mathf.Lerp(1f, m_MinTimeForceMult, timeNorm);

        var finalScore = m_BaseLaunchForce * distFactor * timeFactor;
        if (finalScore >= m_MaxLaunchPower)
            finalScore = m_MaxLaunchPower;
        // -- Combinación multiplicativa: peor de los dos domina
        return finalScore;
    }
}
