using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    // singleton
    public static SoundManager Instance;

    //volume control
    public float volumeBGM;
    public float volumeSFX;

    // SFX list, value is set from inspector
    public AudioClip highlightAudio; 

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
        volumeBGM = 1.0f;
        volumeSFX = 1.0f;

        
        highlightAudio = SoundManager.Instance.highlightAudio;//Resources.Load<AudioClip>("Sound/SFX/ButtonHighlight");//
    }

    private void Start()
    {
        transform.position = GameObject.Find("Main Camera").transform.position;
    }

    public void PlayBGM(AudioClip audio)
    {
        AudioSource.PlayClipAtPoint(audio, transform.position, volumeBGM);
    }
    public void PlaySFX(AudioClip audio)
    {
        AudioSource.PlayClipAtPoint(audio, transform.position, volumeSFX);
    }
}
