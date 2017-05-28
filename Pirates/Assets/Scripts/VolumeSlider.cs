using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour {
    UnityEngine.UI.Slider sliderBGM;
    UnityEngine.UI.Slider sliderSFX;

    void Awake()
    {
        sliderBGM = GameObject.Find("SliderBGM").GetComponent<UnityEngine.UI.Slider>();
        sliderBGM.value = SoundManager.Instance.volumeBGM;
        sliderBGM.onValueChanged.AddListener(delegate { SoundManager.Instance.UpdateBGMVolume(); });
        sliderSFX = GameObject.Find("SliderBGM").GetComponent<UnityEngine.UI.Slider>();
        sliderSFX.value = SoundManager.Instance.volumeSFX;
        sliderBGM.onValueChanged.AddListener(delegate { SoundManager.Instance.UpdateSFXVolume(); });
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
