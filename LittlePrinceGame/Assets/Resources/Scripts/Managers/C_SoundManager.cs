using System;
using System.Collections.Generic;
using UnityEngine;

public class C_SoundManager : MonoBehaviour
{
    private static C_SoundManager _instance;
    public static C_SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Buscar una instancia existente en la escena.
                _instance = FindAnyObjectByType<C_SoundManager>();
                if (_instance == null)
                {
                    // Crear un nuevo GameObject con el script adjunto si no se encuentra ninguna instancia.
                    GameObject singletonObject = new GameObject("Sound Manager");
                    _instance = singletonObject.AddComponent<C_SoundManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    // float es duracion del estado, si es 0 es infinito
    public static Action<MusicState, float> A_ChangeMusicState;
    public static Action<SFXTypes,float> A_PlaySFX;
    [SerializeField] private D_MusicsToPlay m_MusicData;
    [SerializeField] private D_SoundEffects m_SFXData;
    [SerializeField] AudioSource m_AudioS_Music;
    [SerializeField] AudioSource m_AudioS_SFX;
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
        A_ChangeMusicState += MusicChangeStateEvent;
        A_PlaySFX += PlaySFXEvent;
    }
    private void MusicChangeStateEvent(MusicState ms, float duration)
    {
        List<SoundMusicData> musicToPlay = m_MusicData.GetSoundsByState(ms);
        if (musicToPlay.Count > 0)
        {
            SoundMusicData selectedMusic = musicToPlay[UnityEngine.Random.Range(0, musicToPlay.Count)];
            //C_AudioManager.A_PlayMusic(selectedMusic.m_Clip, selectedMusic.m_Volume);
            PlayMusic(selectedMusic.m_Clip, selectedMusic.m_Volume);
        }
    }
    public void PlaySFXEvent(SFXTypes sfxType, float volume)
    {
        List<SoundEffectData> sfxToPlay = m_SFXData.GetSoundsByState(sfxType);
        if (sfxToPlay.Count > 0)
        {
            SoundEffectData selectedSFX = sfxToPlay[UnityEngine.Random.Range(0, sfxToPlay.Count)];
            //C_AudioManager.A_PlaySound(selectedSFX.m_Clip, selectedSFX.m_Volume);
            PlaySound(selectedSFX.m_Clip, selectedSFX.m_Volume);
        }
    }
    private void OnDestroy()
    {
        A_ChangeMusicState -= MusicChangeStateEvent;
        A_PlaySFX -= PlaySFXEvent;
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
    private void PlayMusic(AudioClip clip, float volume)
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
    private void PlaySound(AudioClip clip, float volume)
    {
        Debug.Log($"Playing SFX: {clip.name} at volume: {volume}");
        // Implementar la lógica para reproducir efectos de sonido aquí.
        // Por ejemplo, podrías usar un AudioSource para reproducir el clip con el volumen especificado.
        if (m_AudioS_SFX != null && clip != null)
        {
            m_AudioS_SFX.PlayOneShot(clip, volume);
        }
    }
}