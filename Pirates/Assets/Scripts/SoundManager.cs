using UnityEngine;
using System.Collections;

public enum TrackID
{
    BGM_FIELD,
    BGM_BATTLE
}

public class SoundManager : MonoBehaviour {

    // singleton
    public static SoundManager Instance;

    //volume control
    public float volumeBGM;
    public float volumeSFX;

    // SFX list, value is set from inspector
    public AudioClip highlightAudio;
    public AudioClip battleBGM;
    public AudioClip fieldBGM;
    private AudioSource[] bgm;

    public int trackOnPlay;

    private int trackFadeIn;
    private int trackFadeOut;
    private float fadeTime;
    private bool isFade;
    //private bool[] isPlayed;

    void Awake()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        volumeBGM = 0.3f;
        volumeSFX = 0.3f;
        trackOnPlay = -1;
        trackFadeIn = -1;
        trackFadeOut = -1;
        fadeTime = 1;
        isFade = false;

        SetBGM(fieldBGM, battleBGM); //orders connected to enum TrackNumber
    }

    //=== PRIVATE FUNCTIONS ===

    private void Start()
    {
        transform.position = GameObject.Find("Main Camera").transform.position;
    }

    private void Update()
    {
        if (isFade)
        {
            float deltaVolumeBGM = (Time.deltaTime / fadeTime) * volumeBGM;
            if (trackFadeIn >= 0)
            {
                bgm[trackFadeIn].volume += deltaVolumeBGM;
                if (bgm[trackFadeIn].volume >= volumeBGM)
                {
                    bgm[trackFadeIn].volume = volumeBGM;
                    trackFadeIn = -1;
                }
            }
            if (trackFadeOut >= 0)
            {
                bgm[trackFadeOut].volume -= deltaVolumeBGM;
                if (bgm[trackFadeOut].volume <= 0)
                {
                    bgm[trackFadeOut].Pause();
                    trackFadeOut = -1;
                }
            }
            if (trackFadeIn < 0 && trackFadeOut < 0) isFade = false;
        }
    }

    private void SetBGM(params AudioClip[] audio)
    {
        bgm = new AudioSource[audio.Length];
        //isPlayed = new bool[audio.Length];
        for(int i=0; i<bgm.Length; i++)
        {
            bgm[i] = GameObject.Find("SoundManager").AddComponent<AudioSource>();
            bgm[i].loop = true;
            bgm[i].clip = audio[i];
        }
    }

    

    //=== PUBLIC FUNCTIONS ===

    public void SwitchBGM(int trackSelect, float t)
    {
        
        if (trackOnPlay == -1)
        {
            if (!bgm[trackSelect].isPlaying)
            {
                bgm[trackSelect].volume = volumeBGM;
                bgm[trackSelect].Play();
            }
            trackOnPlay = trackSelect;
        }
        else if (trackOnPlay != trackSelect)
        {
            FadeOut(trackOnPlay, t);
            if (!bgm[trackSelect].isPlaying)
            {
                bgm[trackSelect].volume = 0;
                bgm[trackSelect].Play();
            }
                
            FadeIn(trackSelect, t);
        }
    }

    public void FadeIn(int track, float t)
    {
        if (t <= 0) t = 1;
        if (track < 0 || track >= bgm.Length) track = 0;
        bgm[track].UnPause();
        trackOnPlay = track;
        trackFadeIn = track;
        fadeTime = t;
        isFade = true;
    }

    public void FadeOut(int track, float t)
    {
        if (t <= 0) t = 1;
        if (track < 0 || track >= bgm.Length) track = 0;
        trackFadeOut = track;
        fadeTime = t;
        isFade = true;
    }

    public void PlaySFX(AudioClip audio)
    {
        AudioSource.PlayClipAtPoint(audio, transform.position, volumeSFX);
    }
}
