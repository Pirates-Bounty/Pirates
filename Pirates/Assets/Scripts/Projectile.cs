using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {
    public float lifetime;
    public float damage = 10.0f;
    public int assignedID = -1;
    public AudioClip hitS1;
	public AudioClip hitS2;
	public AudioClip hitS3;
	private AudioClip currentHitS;
    public AudioClip splashS1;
	public AudioClip splashS2;
	public AudioClip splashS3;
	private AudioClip currentSplashS;
	public Vector2 playerVel;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            //AudioSource.PlayClipAtPoint(splash, transform.position, 100.0f);
            //SoundManager.Instance.PlaySFX(splash, 1.0f);
			int splashNum = Random.Range (1, 4);
			switch(splashNum) {
			case 1:
				currentSplashS = splashS1;
				break;
			case 2:
				currentSplashS = splashS2;
				break;
			case 3:
				currentSplashS = splashS3;
				break;
			}
            AudioSource.PlayClipAtPoint(currentSplashS, transform.position, SoundManager.Instance.volumeSFX);
            Destroy(gameObject);
        }
		transform.Translate (Time.deltaTime * playerVel);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            //collision.gameObject.SendMessage("ApplyDamage", damage);
            if (isServer) {
                collision.gameObject.GetComponent<Player>().ApplyDamage(damage, assignedID);
            }
            //AudioSource.PlayClipAtPoint(hit, transform.position, 100.0f);
			int hitNum = Random.Range (1, 4);
			switch(hitNum) {
			case 1:
				currentHitS = hitS1;
				break;
			case 2:
				currentHitS = hitS2;
				break;
			case 3:
				currentHitS = hitS3;
				break;
			}
            SoundManager.Instance.PlaySFX(currentHitS, 1.0f);
        }
        Destroy(gameObject);
    }


}
