using UnityEngine;
using System.Collections;

public class HighlightSound : MonoBehaviour {

    public AudioClip highlightAudio;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMouseEnter()
    {
        AudioSource.PlayClipAtPoint(highlightAudio, transform.position, 0.1f);
    } 
}
