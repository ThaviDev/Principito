using System;
using System.Collections.Generic;
using UnityEngine;

public class C_MusicManager : MonoBehaviour
{
    private static C_MusicManager _instance;
    public static C_MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Buscar una instancia existente en la escena.
                _instance = FindAnyObjectByType<C_MusicManager>();
                if (_instance == null)
                {
                    // Crear un nuevo GameObject con el script adjunto si no se encuentra ninguna instancia.
                    GameObject singletonObject = new GameObject("Music Manager");
                    _instance = singletonObject.AddComponent<C_MusicManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    // float es duracion del estado, si es 0 es infinito
    public static Action<MusicState, float> ChangeState;
    [SerializeField] private D_MusicsToPlay m_MusicData;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Evitar que el objeto sea destruido al cambiar de escena.
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Destruir instancias adicionales si ya existe una instancia.
        }
    }
    private void Start()
    {
        ChangeState += MusicChangeStateEvent;
    }
    private void MusicChangeStateEvent(MusicState ms, float duration)
    {
        List<SoundData> musicToPlay = m_MusicData.GetSoundsByState(ms);
        if (musicToPlay.Count > 0)
        {
            SoundData selectedMusic = musicToPlay[UnityEngine.Random.Range(0, musicToPlay.Count)];
            C_AudioManager.PlayMusic(selectedMusic.m_Clip, selectedMusic.m_Volume);
        }
    }
    private void OnDestroy()
    {
        ChangeState -= MusicChangeStateEvent;
    }

    public void ChangeStateByNameEvent(string musicName)
    {
        if (Enum.TryParse(musicName, out MusicState parsedMusic))
        {
            MusicChangeStateEvent(parsedMusic, 0f);
        }
        else
        {
            Debug.LogWarning($"Music state '{musicName}' not found.");
        }
    }
}