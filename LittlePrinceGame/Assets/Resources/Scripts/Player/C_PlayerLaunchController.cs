using System;
using UnityEngine;

public class C_PlayerLaunchController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_LaunchForce = 20f;
    [SerializeField] private float m_SelfLiftForce = 300f;

    [SerializeField] GameObject m_Target;
    [SerializeField] private C_PlayerAnimation m_Anim;
    public bool IsPreparing { get; private set; }
    public event Action A_OnLaunchStart;
    public event Action A_OnLaunchRelease;

    private void Awake()
    {
        if (m_Anim == null)
            m_Anim = GetComponentInChildren<C_PlayerAnimation>();
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
    }
    public void TryRelease(Vector2 fromWorld, Vector2 toWorld)
    {
        Debug.Log("Accedo a TryRelease");
        Debug.Log("Target es: " + m_Target + " ¿Se esta preparando para lanzarse? " + IsPreparing);
        if (!IsPreparing && m_Target == null) return;
        Debug.Log("Continuo?");
        var launchRb = m_Target.GetComponent<Rigidbody2D>();
        Debug.Log("Tengo RigidBody2D?");
        Vector2 dir = fromWorld - toWorld;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
        dir.Normalize();
        launchRb.linearVelocity = Vector2.zero;
        launchRb.AddForce(dir * m_LaunchForce, ForceMode2D.Force);

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
}
