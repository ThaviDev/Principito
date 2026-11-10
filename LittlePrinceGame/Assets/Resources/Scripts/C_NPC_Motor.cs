using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class C_NPC_Motor : MonoBehaviour
{
    [Tooltip("Identificador único de este NPC (ej: 'Vanidoso', 'Zorro', 'Rey').")]
    public string NpcId = "npc_sin_nombre";

    [Tooltip("Si es true, este NPC ya fue contado por una misión. Evita doble conteo.")]
    [HideInInspector] public bool AlreadyInteracted = false;

    public static event Action<C_NPC_Motor> OnNpcInteracted;

    private void Reset()
    {
        // Al añadir el componente desde el editor, aseguramos que sea trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo el jugador cuenta
        if (!other.CompareTag("Player")) return;

        // Si ya fue contado por una misión anterior, no volvemos a disparar
        if (AlreadyInteracted) return;

        AlreadyInteracted = true;
        OnNpcInteracted?.Invoke(this);
    }

    /// <summary>
    /// Llamado por el QuestManager cuando empieza una nueva misión,
    /// para "resetear" los NPCs que ya habían sido tocados.
    /// </summary>
    public void ResetInteraction()
    {
        AlreadyInteracted = false;
    }
}
