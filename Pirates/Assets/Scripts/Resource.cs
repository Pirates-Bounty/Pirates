using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Resource : NetworkBehaviour {
	public float lifetime = 30f;
	public int gold = 10;
	public AudioClip coinS;

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			collision.gameObject.SendMessage("AddGold", gold);
            SoundManager.Instance.PlaySFX(coinS, 1.0f);
            GameObject.FindGameObjectWithTag("bountyManager").SendMessage("CmdSpawnResource");
			Destroy(gameObject);
		}
	}
}
