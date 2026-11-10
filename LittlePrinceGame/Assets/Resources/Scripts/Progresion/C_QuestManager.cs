using System.Collections.Generic;
using UnityEngine;

public class C_QuestManager : MonoBehaviour
{
    public static C_QuestManager Instance { get; private set; }

    [Header("Misiones Globales (siempre activas)")]
    [Tooltip("Misiones que se checan siempre, sin orden. Ej: 'Consigue 10 estrellas'.")]
    public List<SCOB_QuestData> GlobalMissions = new List<SCOB_QuestData>();

    [Header("Misiones Secuenciales (una tras otra)")]
    [Tooltip("Misiones en orden estricto. Solo la actual está activa.")]
    public List<SCOB_QuestData> SequentialMissions = new List<SCOB_QuestData>();

    [Header("Estado (solo lectura)")]
    public int CurrentMissionOfSequence = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        // 1. Resetear TODAS las misiones (por si acaso venimos de una sesión anterior)
        foreach (var q in GlobalMissions) q.ResetQuest();
        foreach (var q in SequentialMissions) q.ResetQuest();
        CurrentMissionOfSequence = 0;

        // 2. Encender TODAS las globales
        foreach (var q in GlobalMissions)
        {
            q.OnCompleted += HandleGlobalCompleted;
            q.Begin();
        }

        // 3. Encender SOLO la primera secuencial
        if (SequentialMissions.Count > 0)
            StartSequentialMission(0);
    }

    private void OnDisable()
    {
        // Desuscribirse para no dejar basura
        foreach (var q in GlobalMissions)
        {
            q.OnCompleted -= HandleGlobalCompleted;
            q.End();
        }
        foreach (var q in SequentialMissions)
            q.End();
    }

    // ─── Secuenciales ───

    private void StartSequentialMission(int index)
    {
        if (index >= SequentialMissions.Count) return;

        CurrentMissionOfSequence = index;
        var quest = SequentialMissions[index];

        // Suscribir al evento de la misión
        quest.OnCompleted += HandleSequentialCompleted;
        quest.Begin();

        Debug.Log($"[QuestManager] Iniciando misión secuencial: {quest.Title}");
    }

    private void HandleSequentialCompleted()
    {
        var completed = SequentialMissions[CurrentMissionOfSequence];
        completed.OnCompleted -= HandleSequentialCompleted;
        completed.End();

        Debug.Log($"[QuestManager] ¡Misión completada!: {completed.Title}");

        // Avanzar a la siguiente
        int nextIndex = CurrentMissionOfSequence + 1;
        if (nextIndex < SequentialMissions.Count)
            StartSequentialMission(nextIndex);
        else
            Debug.Log("[QuestManager] Secuencia de misiones principal completada.");
    }

    // --- Globales ---

    private void HandleGlobalCompleted()
    {
        // Las globales no avanzan nada, solo se completan.
        // Aquí podrías disparar recompensas o eventos especiales.
    }

}
