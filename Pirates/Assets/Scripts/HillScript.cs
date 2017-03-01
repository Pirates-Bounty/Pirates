using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class HillScript : NetworkBehaviour {

	public const float SCORE_INCREMENT = 5.0f;
	public List<Player> targets;

	// Use this for initialization
	void Start () {
		targets = new List<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < targets.Count; i++) {
			targets[i].score += SCORE_INCREMENT * Time.deltaTime / targets.Count;
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.CompareTag ("Player") && isServer) {
			targets.Add(col.gameObject.GetComponent<Player>());
		}
	}

	void OnTriggerExit2D (Collider2D col) {
		if (col.gameObject.CompareTag ("Player") && isServer) {
			targets.Remove(col.gameObject.GetComponent<Player>());
		}
	}
}
