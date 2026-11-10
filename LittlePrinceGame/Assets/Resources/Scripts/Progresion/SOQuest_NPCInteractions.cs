using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TalkToNpcQuest", menuName = "Game/Quest/TalkToNpc")]
public class SOQuest_NPCInteractions : SCOB_QuestData
{
    [Header("Condiciones de esta mision")]
    [Tooltip("Cuántos NPCs DISTINTOS hay que tocar para completar.")]
    public int RequiredCount = 3;

    [Tooltip("Lista de NpcIds válidos. Si está vacía, CUALQUIER NPC cuenta.")]
    public List<string> ValidNpcIds = new List<string>();

    // - Estado en runtime (solo vive en Play Mode) -
    [System.NonSerialized] public int CurrentCount = 0;
    [System.NonSerialized] private HashSet<string> _seenNpcIds = new HashSet<string>();

    public override void Begin()
    {
        // Nos suscribimos al evento global de NPCs
        C_NPC_Motor.OnNpcInteracted += HandleNpcInteracted;
    }

    public override void End()
    {
        // Nos desuscribimos para no dejar basura
        C_NPC_Motor.OnNpcInteracted -= HandleNpcInteracted;
    }

    private void HandleNpcInteracted(C_NPC_Motor npc)
    {
        // 1. Si hay lista de validos y este NPC no esta, ignoramos
        if (ValidNpcIds.Count > 0 && !ValidNpcIds.Contains(npc.NpcId))
            return;

        // 2. Si ya contamos a este NPC antes, ignoramos (evita doble conteo)
        //    Aunque TAG_NPC ya tiene su propio bool, esta comprobacion es una
        //    segunda capa de seguridad por si hay varios NPCs con el mismo NpcId.
        if (!_seenNpcIds.Add(npc.NpcId))
            return;

        // 3. Contamos
        CurrentCount++;
        Debug.Log($"[{Title}] NPC '{npc.NpcId}' → {CurrentCount}/{RequiredCount}");

        // 4. Ya llegamos a la meta?
        if (CurrentCount >= RequiredCount)
        {
            MarkComplete(); // dispara OnCompleted -> QuestManager avanza
        }
    }

    public override void ResetQuest()
    {
        base.ResetQuest();
        CurrentCount = 0;
        _seenNpcIds.Clear();
    }
}
