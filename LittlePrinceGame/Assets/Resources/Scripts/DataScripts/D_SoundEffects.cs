using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SoundEffectData
{
    public string m_Key;
    [TextArea] public string m_Description;
    public AudioClip m_Clip;
    public float m_Volume;
    public SFXTypes m_Type;
}
public enum SFXTypes
{
    ColectablePickup,
    LaunchSelf,
    Launch,
    Land,
}
[CreateAssetMenu(fileName = "SoundEffects", menuName = "Data/SoundEffects")]
public class D_SoundEffects : ScriptableObject
{
    public List<SoundEffectData> m_SoundEffects = new List<SoundEffectData>();
    // Diccionario interno para búsquedas rápidas (no visible en Inspector)
    private Dictionary<string, SoundEffectData> m_SoundsDictionary;

    // Se llama automáticamente cuando el ScriptableObject se carga en memoria
    private void OnEnable()
    {
        BuildDictionary();
    }
    // Convierte la lista en diccionario
    private void BuildDictionary()
    {
        m_SoundsDictionary = new Dictionary<string, SoundEffectData>();
        foreach (var sound in m_SoundEffects)
        {
            if (sound != null && !string.IsNullOrEmpty(sound.m_Key))
            {
                if (!m_SoundsDictionary.ContainsKey(sound.m_Key))
                {
                    m_SoundsDictionary.Add(sound.m_Key, sound);
                }
                else
                {
                    Debug.LogWarning($"Clave duplicada en la lista: {sound.m_Key}. Se ignorará la segunda entrada.");
                }
            }
        }
    }
    // Método público para obtener SoundData por su clave
    public SoundEffectData GetSoundData(string key)
    {
        if (m_SoundsDictionary == null) BuildDictionary(); // Por si no se llamó OnEnable
        if (m_SoundsDictionary.TryGetValue(key, out SoundEffectData data))
        {
            return data;
        }
        else
        {
            Debug.LogWarning($"No se encontró SoundData con clave: '{key}' en {name}");
            return null;
        }
    }

    public List<SoundEffectData> GetSoundsByState(SFXTypes type)
    {
        List<SoundEffectData> soundsInState = new List<SoundEffectData>();
        foreach (var sound in m_SoundEffects)
        {
            if (sound.m_Type == type)
            {
                soundsInState.Add(sound);
            }
        }
        return soundsInState;
    }

    // Opcional: devolver toda la lista si se necesita iterar
    public List<SoundEffectData> GetAllSounds()
    {
        return m_SoundEffects;
    }

    // Opcional: refrescar el diccionario si modificas la lista en tiempo de ejecución
    public void RefreshDictionary()
    {
        BuildDictionary();
    }
}
