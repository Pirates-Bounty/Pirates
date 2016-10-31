using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Rigidbody2D rb;

	public float rotationSpeed; //150.0f
	public float moveSpeed;		//	5.0f
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode fire;

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
		if (Input.GetKey(up)) {
			rb.AddForce(transform.up * moveSpeed);
		}
		else if (Input.GetKey(down)) {
			rb.AddForce(-transform.up * moveSpeed/4);
		}

		if (Input.GetKey(left)) {
			transform.Rotate (0, 0, Time.deltaTime * rotationSpeed);
			rb.velocity = rb.velocity.magnitude * transform.up;
		}
		else if (Input.GetKey(right)) {
			transform.Rotate (0, 0, -Time.deltaTime * rotationSpeed);
			rb.velocity = rb.velocity.magnitude * transform.up;
		}
	}

	void Update () {
		// check if you want to fire the cannons!
		// you can hold the fire button down to fire at regular intervals, or tap it and fire automatically at the next interval
		if (firingTimer > 0) {
			firingTimer -= Time.deltaTime;
			if (!loadedBullet && Input.GetKey(fire)) {
				loadedBullet = true;
			}
            else if (Input.GetKey(fire))
            {
                FireCannons();
                firingTimer = firingDelay;
                loadedBullet = false;
            }
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
