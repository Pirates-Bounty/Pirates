using UnityEngine;
using System.Collections;

//order relates to Awake SetBGM
public enum TrackID
{
    BGM_FIELD,
    BGM_BATTLE
}

public class SoundManager : MonoBehaviour {

    // singleton
    public static SoundManager Instance;

    // volume control & current BGM ID
    public float volumeBGM;
    public float volumeSFX;
    public int trackOnPlay;
    // audio list, values are set from inspector
    public AudioClip highlightAudio;
    public AudioClip battleBGM;
    public AudioClip fieldBGM;

    // private members
    private AudioSource[] bgm;
    private int trackFadeIn;
    private int trackFadeOut;
    private float fadeTime;
    private bool isFade;

    // initializations
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

        SetBGM(fieldBGM, battleBGM); //order relates to enum TrackID
        transform.position = GameObject.Find("Main Camera").transform.position;
    }



    //=== PRIVATE FUNCTIONS ===
    //- Update : fade mechanic
    //- SetBGM : AudioSource bgm initialization

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

    private void SetBGM(params AudioClip[] audio) //input: BGMList
    {
        bgm = new AudioSource[audio.Length];
        for(int i=0; i<bgm.Length; i++)
        {
            bgm[i] = GameObject.Find("SoundManager").AddComponent<AudioSource>();
            bgm[i].loop = true;
            bgm[i].clip = audio[i];
        }
    }



    //=== PUBLIC FUNCTIONS ===
    //- PlayBGM    : play a BGM, stop other BGMs
    //- SwitchBGM  : gradually switch BGMs
    //- FadeIn     : gradually play BGM
    //- FadeOut    : gradually stop BGM
    //- StopAllBGM : stop all playing BGM
    //- PlaySFX    : play one shot audio clip

    public void PlayBGM(int track) //input: TrackID
    {
        StopAllBGM();
        bgm[track].volume = volumeBGM;
        bgm[track].Play();
        trackOnPlay = track;
    }

    public void SwitchBGM(int trackSelect, float t) //input: TrackID, fadeTime
    {
        
        if (trackOnPlay == -1) PlayBGM(trackSelect);
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

    public void FadeIn(int track, float t) //input: TrackID, fadeTime
    {
        if (t <= 0) t = 1;
        if (track < 0 || track >= bgm.Length) track = 0;
        bgm[track].UnPause();
        trackOnPlay = track;
        trackFadeIn = track;
        fadeTime = t;
        isFade = true;
    }

    public void FadeOut(int track, float t) //input: TrackID, fadeTime
    {
        if (t <= 0) t = 1;
        if (track < 0 || track >= bgm.Length) track = 0;
        trackFadeOut = track;
        fadeTime = t;
        isFade = true;
    }

    public void StopAllBGM()
    {
        for(int i=0; i<bgm.Length; i++)
            bgm[i].Stop();
        trackOnPlay = -1;
    }

    public void PlaySFX(AudioClip audio) //input: SFXClip
    {
        AudioSource.PlayClipAtPoint(audio, transform.position, 0.05f);
    }
}
