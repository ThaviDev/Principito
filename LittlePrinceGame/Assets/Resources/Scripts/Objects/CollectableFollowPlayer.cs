using UnityEngine;

public class CollectableFollowPlayer : MonoBehaviour
{
    [SerializeField] private float m_speed = 5f;
    private bool m_isFollowing = false;
    private Transform m_Player;
    private void Update()
    {
        if (m_isFollowing)
        {
            FollowPlayer();
        }
    }
    private void FollowPlayer()
    {
        Vector3 direction = (m_Player.position - transform.position).normalized;
        transform.parent.transform.position += direction * m_speed * Time.deltaTime;
        m_speed += Random.Range(-0.01f, 2f) * Time.deltaTime; // Incrementa la velocidad con el tiempo
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_isFollowing = true;
            m_Player = collision.transform;
        }
    }
}
