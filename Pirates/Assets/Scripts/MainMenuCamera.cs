using UnityEngine;
using System.Collections;

public class MainMenuCamera : MonoBehaviour {

	public AudioClip menuM;

	// Use this for initialization
	void Start () {

		// Play Sound
		AudioSource.PlayClipAtPoint(menuM, transform.position, 75.0f);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
