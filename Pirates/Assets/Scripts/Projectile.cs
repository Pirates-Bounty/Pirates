using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public float lifetime;
    public float damage = 10.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, lifetime);
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.SendMessage("ApplyDamage", damage);
        }
        Destroy(gameObject);
    }
}
