using System;
using UnityEngine;

public class C_PlayerLaunchController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_BaseLaunchForce = 200f;
    [SerializeField] private float m_SelfLiftForce = 300f;
    [SerializeField] bool m_TimeTypeForceIsPositive = true;
    [Tooltip("Impacto de la distancia en la fuerza del lanzamiento.")]
    [SerializeField, Range(0f, 1f)] private float m_DistanceImpact = 1f;
    [Tooltip("Impacto del tiempo en la fuerza del lanzamiento.")]
    [SerializeField, Range(0f, 1f)] private float m_TimeImpact = 1f;
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
    [Tooltip("Tiempo durante el cual se ignora la detección de suelo tras despegar. " +
             "Debe ser suficiente para alejarse de m_RayDistance, pero corto para no " +
             "perder un aterrizaje real.")]
    [SerializeField] private float m_GroundSuppressionTime = 0.15f;
    private bool m_WasGroundedAtBegin;

    [Header("References")]
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private C_PlayerAnimation m_Anim;
    [SerializeField] private C_PlayerInputAdapter m_InputAdapter;
    [SerializeField] private C_PlayerGroundDetector m_GroundDetector;

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
        if (m_InputAdapter == null)
            m_InputAdapter = GetComponent<C_PlayerInputAdapter>();
        if (m_GroundDetector == null)
            m_GroundDetector = GetComponent<C_PlayerGroundDetector>();
    }

    private void Update()
    {
        // Si estamos preparando el lanzamiento, calculamos la fuerza en tiempo real (solo para verla en pantalla)
        if (IsPreparing && C_TouchInputManager.Instance.IsTouchPressing)
        {
            var gesture = C_TouchInputManager.Instance.LastGesture;
            // Ojo: mientras presionas, el gesto real no ha terminado. 
            // Podrías calcular la distancia actual así:
            float currentDist = Vector2.Distance(gesture.StartPos, C_TouchInputManager.Instance.TouchFollower.transform.position);
            float currentDuration = Time.unscaledTime - gesture.Duration; // Necesitarías guardar el tiempo inicial
            if (C_LaunchDebugger.Instance != null)
                C_LaunchDebugger.Instance.UpdateDebugInfo(0f, 0f, 0f, 0f, currentDist, currentDuration);
        }
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
            m_Target = gameObject;
            target = gameObject;
            //rb.AddRelativeForce(Vector2.up * m_SelfLiftForce); // Impulso de salto: Ya no porque velocidad siempre esta en 0 cuando es seleccionado
            if (!isGrounded)
            {
                m_Anim.SetAnimationState?.Invoke(PlayerAnimationStates.spinning);
            }
        }

        rb.linearVelocity = Vector2.zero;
        m_WasGroundedAtBegin = isGrounded;
        IsPreparing = true;
        A_OnLaunchStart?.Invoke();
    }

    public void Preparing()
    {
        if (!IsPreparing || m_Target == null) return;
        var launchRb = m_Target.GetComponent<Rigidbody2D>();
        if (launchRb != null) launchRb.linearVelocity = Vector2.zero;
        print("Objeto seleccionado esta en velocidad 0");
    }
    public void TryRelease(Vector2 fromWorld, Vector2 toWorld)
    {
        if (!IsPreparing || m_Target == null) { Reset(); return; }
        //Vector2 dir = fromWorld - toWorld;
        Vector2 dir = C_TouchInputManager.Instance.InvertDrag ?
            (toWorld - fromWorld) :
            (fromWorld - toWorld);
        //Vector2 dragVector = (fromWorld - toWorld);
        if (dir.sqrMagnitude < 0.2f) { Reset(); return; }

        // si el gesto fue horizontal y empezó en suelo, NO lanzamos.
        // Dejamos que C_PlayerMotor use WalkInput (ya clasificado) y el jugador camine.
        if (m_WasGroundedAtBegin && m_InputAdapter.IsWalkGesture)
        {
            Reset();
            return;
        }

        var launchRb = m_Target.GetComponent<Rigidbody2D>();
        if (launchRb == null) { Reset(); return; }

        var gesture = C_TouchInputManager.Instance.LastGesture;
        float force = CalculateLaunchForce(gesture);

        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
        dir.Normalize();

        Debug.Log("Final Force: " + (m_WasGroundedAtBegin ? 500 : force));

        bool isSelf = (m_Target == this.gameObject);
        bool wasGrounded = m_GroundDetector.IsGrounded;

        if (m_WasGroundedAtBegin)
            launchRb.AddForce(dir * 500, ForceMode2D.Force);
        else 
            launchRb.AddForce(dir * force, ForceMode2D.Force);

        if (isSelf && wasGrounded)
            m_GroundDetector.SuppressGroundDetection(m_GroundSuppressionTime);
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
        if (m_InputAdapter.MyGroundControlMode == C_PlayerInputAdapter.GroundControlMode.Swipe)
            return this.gameObject;

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
        float rawDistFactor = Mathf.Lerp(m_MinDistForceMult, 1f, distNorm);

        // -- Duración: 0 -> 1, max -> min (mas tiempo = menos fuerza) (o alrevez segun el bool)
        float timeNorm = Mathf.Clamp01(gesture.Duration / m_MaxHoldDuration);
        float rawTimeFactor = 
            !m_TimeTypeForceIsPositive ? 
            Mathf.Lerp(1f, m_MinTimeForceMult, timeNorm) : 
            Mathf.Lerp(m_MinTimeForceMult, 1f, timeNorm);

        float finalDistFactor = Mathf.Lerp(1f, rawDistFactor, m_DistanceImpact);
        float finalTimeFactor = Mathf.Lerp(1f, rawTimeFactor, m_TimeImpact);

        var finalScoreLimitless = m_BaseLaunchForce * finalDistFactor * finalTimeFactor;

        var finalScore = finalScoreLimitless;
        if (finalScore >= m_MaxLaunchPower)
            finalScore = m_MaxLaunchPower;

        if (C_LaunchDebugger.Instance != null)
            C_LaunchDebugger.Instance.UpdateDebugInfo(finalScore, finalScoreLimitless, finalDistFactor, finalTimeFactor, gesture.Distance, gesture.Duration);
        // -- Combinación multiplicativa: peor de los dos domina
        return finalScore;
    }
}
