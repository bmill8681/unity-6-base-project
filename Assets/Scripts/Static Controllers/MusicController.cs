using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * TODO: Invoking PlayMusic with a new selection mid fade will set the volume of the currenty
 * active music to max volume and then begin the fadeout. It should fade out from the current
 * volume. Need global vars to account for the current volumes of the active and inactive tracks
 * at the time that a fade was initiated.
 * 
 */
public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField] List<SoundTrack> soundTracks;
    [SerializeField] MUSIC_STATE state = MUSIC_STATE.OFF;
    [SerializeField] float defaultFadeDuration = 5f;
    
    public GameSettingsSO gameSettings;

    AudioSource monoSource1, monoSource2, activeSource, inactiveSource;

    float fadeDuration;
    float timer = 0f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        monoSource1 = this.gameObject.AddComponent<AudioSource>();
        monoSource2 = this.gameObject.AddComponent<AudioSource>();
        monoSource1.volume = 0f;
        monoSource2.volume = 0f;
        monoSource1.playOnAwake = false;
        monoSource2.playOnAwake = false;
        monoSource1.loop = true;
        monoSource2.loop = true;
        activeSource = monoSource1;
        inactiveSource = monoSource2;
        fadeDuration = defaultFadeDuration;
    }

    private void Start()
    {
        SoundTrack track = soundTracks.Find(cur => cur.trackName.Equals(SOUNDTRACK.TEST_TRACK_2));
        if (track.clip != null) monoSource1.clip = track.clip;
    }

    private void Update()
    {
        HandleState();
        ResolveNextState();
    }

    void HandleState()
    {
        switch (state)
        {
            case MUSIC_STATE.FADE_IN:
                FadeIn(activeSource);
                FadeOut(inactiveSource);
                break;
            case MUSIC_STATE.FADE_OUT:
                FadeOut(activeSource);
                FadeOut(inactiveSource);
                break;
            case MUSIC_STATE.CROSS_FADE:
                FadeIn(inactiveSource);
                FadeOut(activeSource);
                break;
            default:
                break;
        }
    }
    void FadeIn(AudioSource source)
    {
        float musicVolume = gameSettings.MasterVolume * gameSettings.MusicVolume;
        if (!source.isPlaying) source.Play();
        if (source.volume > musicVolume) source.volume = musicVolume;
        if (source.volume.Equals(musicVolume)) return;

        float newVolume = Mathf.Lerp(0f, musicVolume, timer / fadeDuration);
        timer += Time.deltaTime;

        source.volume = newVolume;
    }

    void FadeOut(AudioSource source)
    {
        if (!source.isPlaying) source.Play();
        if (source.volume <= 0.001f) source.volume = 0f;
        if (source.volume.Equals(0f))
        {
            if (source.isPlaying) source.Stop();
            return;
        }
        float musicVolume = gameSettings.MasterVolume * gameSettings.MusicVolume;
        float newVolume = Mathf.Lerp(musicVolume, 0f, timer / fadeDuration);
        timer += Time.deltaTime;

        source.volume = newVolume;
    }


    /*
     * ResolveNextState
     * 
     * Automates changing the music state from the current state to the next
     */
    void ResolveNextState()
    {
        float musicVolume = gameSettings.MasterVolume * gameSettings.MusicVolume;
        switch (state)
        {
            case MUSIC_STATE.FADE_IN:
                {
                    if (activeSource.volume.Equals(musicVolume))
                    {
                        ChangeState(MUSIC_STATE.ON);
                    }
                    break;
                }
            case MUSIC_STATE.FADE_OUT:
                {
                    if (activeSource.volume.Equals(0f)) ChangeState(MUSIC_STATE.OFF);
                    break;
                }
            case MUSIC_STATE.CROSS_FADE:
                {

                    if (activeSource.volume.Equals(0f) && inactiveSource.volume.Equals(musicVolume))
                    {
                        ChangeState(MUSIC_STATE.ON);
                        AudioSource temp = activeSource;
                        activeSource = inactiveSource;
                        inactiveSource = temp;
                    }
                    break;
                }
            case MUSIC_STATE.ON:
                {
                    if (activeSource.volume < musicVolume || inactiveSource.volume < 0f) ChangeState(MUSIC_STATE.FADE_IN);
                    break;
                }
            case MUSIC_STATE.OFF:
                {
                    if (activeSource.volume > 0f || inactiveSource.volume > 0f) ChangeState(MUSIC_STATE.FADE_OUT);
                    break;
                }
            default: break;
        }
    }

    void ChangeState(MUSIC_STATE newState)
    {
        state = newState;
        timer = 0f;
    }


    /*
     * PlayMusic
     * 
     * Takes in a SOUNDTRACK (trackName) and optional transition duration, 
     * crossfades the music playing to the new track over the specified duration.
     */
    public void PlayMusic(SOUNDTRACK trackName, float duration)
    {
        SoundTrack track = soundTracks.Find(cur => cur.trackName.Equals(trackName));
        if (track.clip == null) return;

        inactiveSource.clip = track.clip;
        inactiveSource.time = 0f;
        ChangeState(MUSIC_STATE.CROSS_FADE);
        fadeDuration = duration;
    }
    /* If no duration is specified, sets the fade duration to the default duration */
    public void PlayMusic(SOUNDTRACK trackName)
    {
        PlayMusic(trackName, defaultFadeDuration);
    }

    /**
     * PlayMusic
     * 
     * If no SOUNDTRACK (trackName) is provided, PlayMusic will fade the existing sound track in
     * assuming one has been set.
     */
    public void PlayMusic()
    {
        if (activeSource.clip == null) return;
        ChangeState(MUSIC_STATE.FADE_IN);
        fadeDuration = defaultFadeDuration * 0.25f;
    }

    /*
     * StopMusic
     * 
     * Fades the current music track out
     */
    public void StopMusic()
    {
        ChangeState(MUSIC_STATE.FADE_OUT);
        fadeDuration = defaultFadeDuration * 0.25f;
    }

    /**
     * Specifically set the active source music volume
     */
    public void SetMusicVolume(float vol)
    {
        if (state.Equals(MUSIC_STATE.CROSS_FADE))
        {
            inactiveSource.volume = vol;
            activeSource.volume = 0f;
        }
        else
        {
            activeSource.volume = vol;
            inactiveSource.volume = 0f;
        }
        state = MUSIC_STATE.ON;

    }
}



public enum ACTIVE_SOURCE
{
    SOURCE1,
    SOURCE2
}

public enum SOUNDTRACK
{
    TEST_TRACK_1,
    TEST_TRACK_2
}

public enum MUSIC_STATE
{
    OFF,
    ON,
    FADE_IN,
    FADE_OUT,
    CROSS_FADE
}

[Serializable]
public struct SoundTrack
{
    public SOUNDTRACK trackName;
    public AudioClip clip;
}