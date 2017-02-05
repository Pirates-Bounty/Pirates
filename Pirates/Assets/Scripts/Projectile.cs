using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {
    public float lifetime;
    public float damage = 10.0f;
	public int assignedID = -1;
	public AudioClip hit;
	public AudioClip splash;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			//AudioSource.PlayClipAtPoint(splash, transform.position, 100.0f);
            SoundManager.Instance.PlaySFX(splash, 1.0f);
            Destroy(gameObject);
		}
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            //collision.gameObject.SendMessage("ApplyDamage", damage);
			if (isServer) {
				collision.gameObject.GetComponent<Player> ().ApplyDamage (damage, assignedID);
			}
			//AudioSource.PlayClipAtPoint(hit, transform.position, 100.0f);
            SoundManager.Instance.PlaySFX(hit, 1.0f);
        }
        Destroy(gameObject);
    }

    
}
