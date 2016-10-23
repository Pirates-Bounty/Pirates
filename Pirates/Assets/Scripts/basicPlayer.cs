using UnityEngine;
using System.Collections;

public class basicPlayer : MonoBehaviour {

	public Rigidbody2D rb;

	public float rotationSpeed; //150.0f
	public float moveSpeed;		//	5.0f

	public float bulletForce = 10.0f;
	public GameObject projectile;
	public float firingDelay = 0.3f;
	private float firingTimer;
	private bool loadedBullet;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		firingTimer = firingDelay;
		loadedBullet = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
			rb.AddForce(Input.GetAxis("Vertical") * transform.up * moveSpeed);
		}

		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			rb.AddForce(Input.GetAxis("Vertical") * transform.up * moveSpeed/4);
		}

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		transform.Rotate(0, 0, -x);
		rb.velocity = rb.velocity.magnitude * transform.up;
	}

	void Update () {
		// check if you want to fire the cannons!
		// you can hold the fire button down to fire at regular intervals, or tap it and fire automatically at the next interval
		if (firingTimer > 0) {
			firingTimer -= Time.deltaTime;
			if (!loadedBullet && (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.F))) {
				loadedBullet = true;
			}
		} else if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.F) || loadedBullet) {
			FireCannons ();
			firingTimer = firingDelay;
			loadedBullet = false;
		}
	}

	void FireCannons () {
		Quaternion q = Quaternion.FromToRotation (Vector3.up, transform.right);
		GameObject go = (GameObject)Instantiate (projectile, transform.position, q);
		Rigidbody2D bulletRb = go.GetComponent<Rigidbody2D> ();
		bulletRb.AddForce (go.transform.up * bulletForce);

		q = Quaternion.FromToRotation (Vector3.up, -transform.right);
		go = (GameObject)Instantiate (projectile, transform.position, q);
		bulletRb = go.GetComponent<Rigidbody2D> ();
		bulletRb.AddForce (go.transform.up * bulletForce);
	}
}
