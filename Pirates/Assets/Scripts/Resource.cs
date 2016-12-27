using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Resource : NetworkBehaviour {
	public float lifetime = 30f;
	public int gold = 10;
	//public AudioClip shot;
	//public AudioClip hit;

	// Use this for initialization
	void Start () {
		//AudioSource.PlayClipAtPoint(shot, transform.position, 65.0f);
	}

	// Update is called once per frame
	void Update () {
		//Destroy(gameObject, lifetime);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			collision.gameObject.SendMessage("AddGold", gold);
            GameObject.Find("MapGenerator").SendMessage("CmdSpawnResource");
            Destroy(gameObject);
            //AudioSource.PlayClipAtPoint(hit, transform.position, 100.0f);
        }
		
	}
}
