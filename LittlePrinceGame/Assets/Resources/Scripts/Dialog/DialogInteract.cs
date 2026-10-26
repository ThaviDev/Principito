using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogInteract : MonoBehaviour
{
    [Tooltip("Id inicial del dialogo en el archivo de texto")]
    [SerializeField] private int m_DialogoId = 1;
    [Tooltip("Tag del jugador.")]
    [SerializeField] private string m_PlayerTag = "Player";
    private void Reset()
    {
        // Se asegura que el componente collider sea trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) { collider.isTrigger = true; }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(m_PlayerTag)) return;
        Interactuar(other.GetComponent<C_PlayerMotor>());

    }
    public void Interactuar(C_PlayerMotor player)
    {
        //if (!player.IsGrounded) return;
        // ESTO DISPARA EL DIALOGO
        DialogLoader.Instance.PlayDialogue(m_DialogoId);
        //player.StopPlayerMovement();
    }
}
