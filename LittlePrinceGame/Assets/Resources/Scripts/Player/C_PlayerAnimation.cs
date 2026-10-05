using System;
using UnityEngine;
public enum PlayerAnimationStates
{
    idle,
    walk,
    fly,
    landing,
    jump,
    launch,
}
public class C_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private SpriteRenderer m_Sprite;
    public static Action<PlayerAnimationStates> SetAnimationState;
    private void Awake()
    {

    }
    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Sprite = GetComponent<SpriteRenderer>();
        SetAnimationState += CheckStateEvent;
    }
    private void CheckStateEvent(PlayerAnimationStates state)
    {
        if (m_Animator == null)
        {
            Debug.LogError("Animator is not assigned in C_PlayerAnimation.");
            return;
        }
        if (m_Animator.GetInteger("State") != (int)state)
        {
            ChangeAnimationState(state);
        }
    }
    private void ChangeAnimationState(PlayerAnimationStates state)
    {
        m_Animator.SetInteger("State", (int)state);
    }
    public void FlipSprite(bool flip)
    {
        m_Sprite.flipX = flip;
    }
    private void OnDestroy()
    {
        SetAnimationState -= ChangeAnimationState;
    }
}
