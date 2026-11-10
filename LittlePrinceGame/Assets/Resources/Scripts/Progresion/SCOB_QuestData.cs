using System;
using UnityEngine;

//[CreateAssetMenu(fileName = "Quest", menuName = "Game/Quest Data")] // Este no sirve para crearse
/// <summary>
/// Clase base para TODAS las misiones.
/// Cada misión concreta (TalkToNpcQuest, CollectStarsQuest, etc.)
/// hereda de aquí y define CÓMO se completa.
/// </summary>
public abstract class SCOB_QuestData : ScriptableObject
{
    [Header("Identidad")]
    public string QuestId = "quest_id";
    public string Title = "Nombre de la Mision";
    [TextArea] public string Description = "Descripcion de la mision";

    // - Estado en runtime -
    // [NonSerialized] significa que NO se guarda en el asset (solo vive en Play Mode).
    // Esto es IMPORTANTE porque si lo guardara, la siguiente vez que abras Unity
    // la misión aparecería completada aunque no lo estuviera.
    [System.NonSerialized] public bool IsCompleted = false;

    /// <summary>
    /// Evento que se dispara al completarse.
    /// El QuestManager se suscribe a este evento.
    /// </summary>
    public event Action OnCompleted;

    // - Métodos abstractos (obligatorios en cada hijo) -

    /// <summary>
    /// Se llama cuando la mision empieza. Aqui la mision se suscribe
    /// a los eventos que necesita escuchar (ej: OnNpcInteracted, OnStarsChange).
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// Se llama cuando la misión termina (se completó o se canceló).
    /// Aquí se desuscribe para no dejar basura.
    /// </summary>
    public abstract void End();

    // - Método protegido (solo accesible desde los hijos) -

    /// <summary>
    /// Las misiones concretas llaman a esto cuando su condición se cumple.
    /// Dispara el evento que el QuestManager escucha.
    /// </summary>
    protected void MarkComplete()
    {
        if (IsCompleted) return; // protección contra doble disparo
        IsCompleted = true;
        OnCompleted?.Invoke();
    }

    /// <summary>
    /// Se llama al iniciar el juego, para resetear la misión.
    /// Cada hija puede sobrescribirlo para limpiar sus propias variables.
    /// </summary>
    public virtual void ResetQuest()
    {
        IsCompleted = false;
    }
}
