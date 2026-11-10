using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogInteract : MonoBehaviour
{
    [Tooltip("Id inicial del dialogo en el archivo de texto")]
    [SerializeField] private int[] m_DialogoId = new int[] { 1 };
    [Tooltip("Tag del jugador.")]
    [SerializeField] private string m_PlayerTag = "Player";

    [Tooltip("Opcional. Si se asigna, el ID se pregunta a este proveedor.")]
    [SerializeField] private MonoBehaviour m_Provider;

    private int m_CurrentDialogId = 0;
    private bool m_PlayerInRange = false;
    private bool m_HasTriggered = false;
    private C_PlayerGroundDetector m_GroundDetector;

    private void Reset()
    {
        // Se asegura que el componente collider sea trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) { collider.isTrigger = true; }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(m_PlayerTag)) return;

        m_PlayerInRange = true;
        m_GroundDetector = other.GetComponent<C_PlayerGroundDetector>();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(m_PlayerTag)) return;
        m_PlayerInRange = false;
        m_HasTriggered = false;
        m_GroundDetector = null;
    }

    private void Update()
    {
        if (!m_PlayerInRange || m_HasTriggered || m_GroundDetector == null) return;
        if (!m_GroundDetector.IsGrounded) return;
        if (DialogLoader.Instance == null || DialogLoader.Instance.IsDialogueActive) return;
        m_HasTriggered = true;
        Interactuar(m_GroundDetector.GetComponent<C_PlayerMotor>());
    }
    public void Interactuar(C_PlayerMotor player)
    {
        int dialogId = m_DialogoId[m_CurrentDialogId];

        // ESTO DISPARA EL DIALOGO
        DialogLoader.Instance.PlayDialogue(dialogId);

        // Si hay proveedor y lo implementa, le preguntamos
        if (m_Provider is IDialogProvider provider)
            dialogId = provider.GetDialogId();

        //m_CurrentDialogId = (m_CurrentDialogId + 1) % m_DialogoId.Length;
        m_CurrentDialogId = m_DialogoId.Length == m_CurrentDialogId + 1 ? m_CurrentDialogId : m_CurrentDialogId + 1;
    }

}
