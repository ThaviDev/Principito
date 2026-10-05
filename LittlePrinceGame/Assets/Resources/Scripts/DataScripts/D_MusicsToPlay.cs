using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SoundMusicData
{
    public string m_Key;
    [TextArea] public string m_Description;
    public AudioClip m_Clip;
    // Posibles valores de inicio de la música, en segundos, para que no siempre empiece desde el mismo punto
    public float[] m_PossibleStarts;
    public float m_Volume;
    public MusicState m_State;
    /*
    public m_StateToPlayOn m_State;
    public enum m_StateToPlayOn
    {
        None,
        Safe,
        Danger,
        Horde,
        SafeRoom,
        GameOver,
        Pause,
        MainMenu,
    }
    */
}
public enum MusicState
{
    None,
    Exploration,
    Dialogue,
}
[CreateAssetMenu(fileName = "Musics", menuName = "Data/Musics")]
public class D_MusicsToPlay : ScriptableObject
{
    public List<SoundMusicData> m_MusicsToPlay = new List<SoundMusicData>();

    // Diccionario interno para búsquedas rápidas (no visible en Inspector)
    private Dictionary<string, SoundMusicData> m_SoundsDictionary;

    // Se llama automáticamente cuando el ScriptableObject se carga en memoria
    private void OnEnable()
    {
        BuildDictionary();
    }
    // Convierte la lista en diccionario
    private void BuildDictionary()
    {
        m_SoundsDictionary = new Dictionary<string, SoundMusicData>();
        foreach (var sound in m_MusicsToPlay)
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
    public SoundMusicData GetSoundData(string key)
    {
        if (m_SoundsDictionary == null) BuildDictionary(); // Por si no se llamó OnEnable
        if (m_SoundsDictionary.TryGetValue(key, out SoundMusicData data))
        {
            return data;
        }
        else
        {
            Debug.LogWarning($"No se encontró SoundData con clave: '{key}' en {name}");
            return null;
        }
    }

    public List<SoundMusicData> GetSoundsByState(MusicState state)
    {
        List<SoundMusicData> soundsInState = new List<SoundMusicData>();
        foreach (var sound in m_MusicsToPlay)
        {
            if (sound.m_State == state)
            {
                soundsInState.Add(sound);
            }
        }
        return soundsInState;
    }

    // Opcional: devolver toda la lista si se necesita iterar
    public List<SoundMusicData> GetAllSounds()
    {
        return m_MusicsToPlay;
    }

    // Opcional: refrescar el diccionario si modificas la lista en tiempo de ejecución
    public void RefreshDictionary()
    {
        BuildDictionary();
    }
}