using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {
    public float lifetime;
    public float damage = 10.0f;
	public AudioClip shotS;
	public AudioClip hitS;
	// Use this for initialization
	void Start () {
		AudioSource.PlayClipAtPoint (shotS, transform.position, 75.0f);
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, lifetime);
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.SendMessage("ApplyDamage", damage);
			AudioSource.PlayClipAtPoint (hitS, transform.position, 100.0f);
        }
        Destroy(gameObject);
    }
}
