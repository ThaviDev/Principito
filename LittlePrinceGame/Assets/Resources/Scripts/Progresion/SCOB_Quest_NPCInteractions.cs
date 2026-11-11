using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TalkToNpcQuest", menuName = "Game/Quest/TalkToNpc")]
public class SOQuest_NPCInteractions : SCOB_QuestData
{
    [Header("Condiciones de esta mision")]
    [Tooltip("Cuántos NPCs DISTINTOS hay que tocar para completar.")]
    public IntSCOB m_RequiredCount;

    [Tooltip("Lista de NpcIds válidos. Si está vacía, CUALQUIER NPC cuenta.")]
    public List<string> ValidNpcIds = new List<string>();

    // - Estado en runtime (solo vive en Play Mode) -
    public IntSCOB m_CurrentCount;

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
        m_CurrentCount.SetValue(m_CurrentCount.Value + 1);
        Debug.Log($"[{Title}] NPC '{npc.NpcId}' → {m_CurrentCount.Value}/{m_RequiredCount.Value}");

        // 4. Ya llegamos a la meta?
        if (m_CurrentCount.Value >= m_RequiredCount.Value)
        {
            MarkComplete(); // dispara OnCompleted -> QuestManager avanza
        }
    }

    public override void ResetQuest()
    {
        base.ResetQuest();
        m_CurrentCount.SetValue(0);
        _seenNpcIds.Clear();
    }

    public override bool TryGetDisplayData(
    out IntSCOB value1, out IntSCOB value2,
    out string prefix, out string middle, out string postfix)
    {
        value1 = m_CurrentCount;
        value2 = m_RequiredCount;
        prefix = $"{Title}: ";   // "Invitados: "
        middle = "/";            // "Invitados: 3/"
        postfix = "";            // "Invitados: 3/8"
        return value1 != null && value2 != null;
    }
}
