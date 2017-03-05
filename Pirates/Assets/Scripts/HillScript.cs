using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class HillScript : NetworkBehaviour {

	public const float SCORE_INCREMENT = 1.0f;
	public float scoreReserve;
	public List<Player> targets;
	//private GameObject bountyManager;

	// Use this for initialization
	void Start () {
		targets = new List<Player>();
		scoreReserve = Random.Range (10f, 15f);
		//bountyManager = GameObject.Find ("BountyManager");
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < targets.Count; i++) {
			targets[i].score += SCORE_INCREMENT * Time.deltaTime / targets.Count;
			scoreReserve -= SCORE_INCREMENT * Time.deltaTime / targets.Count;
		}
		if (scoreReserve <= 0f) {
			scoreReserve = Random.Range (10f, 15f);
			BountyManager.Instance.CmdMoveHill ();
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
