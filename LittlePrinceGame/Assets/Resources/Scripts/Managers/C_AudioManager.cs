using System;
using UnityEngine;

public class C_AudioManager : MonoBehaviour
{
    private static C_AudioManager _instance;
    public static C_AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Buscar una instancia existente en la escena.
                _instance = FindAnyObjectByType<C_AudioManager>();
                if (_instance == null)
                {
                    // Crear un nuevo GameObject con el script adjunto si no se encuentra ninguna instancia.
                    GameObject singletonObject = new GameObject("Audio Manager");
                    _instance = singletonObject.AddComponent<C_AudioManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    // Clip de audio, volumen
    public static Action<AudioClip, float> PlayMusic;
    [SerializeField] AudioSource m_AudioS_Music;
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
        PlayMusic += PlayMusicEvent;
    }
    private void PlayMusicEvent(AudioClip clip, float volume)
    {
        // Implementar la lógica para reproducir música aquí.
        // Por ejemplo, podrías usar un AudioSource para reproducir el clip con el volumen especificado.
        if (m_AudioS_Music != null && clip != null)
        {
            m_AudioS_Music.clip = clip;
            m_AudioS_Music.volume = volume;
            m_AudioS_Music.Play();
        }
    }
    private void OnDestroy()
    {
        PlayMusic -= PlayMusicEvent;
    }
}
