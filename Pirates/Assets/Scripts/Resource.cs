using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Resource : NetworkBehaviour {
	public float lifetime = 30f;
	public int gold = 10;
	public AudioClip coinS;

	//void OnCollisionEnter2D(Collision2D collision) {
	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.CompareTag("Player")) {
			col.gameObject.SendMessage("AddGold", gold);
			
            GameObject.FindGameObjectWithTag("bountyManager").SendMessage("CmdSpawnResource");
			Destroy(gameObject);
		}
	}
}
