using UnityEngine;
using System.Collections;

public class basicPlayer : MonoBehaviour {

	public bool playerOne;

	public Rigidbody2D rb;

	public float rotationSpeed; //150.0f
	public float moveSpeed;		//	5.0f

	public float bulletForce = 10.0f;
	public GameObject projectile;
	public float firingDelay = 0.3f;
	private float firingTimer;
	private bool loadedBullet;

	public int health;
	public int maxHealth = 100;
	public int resources;
	public int prestige;

	private upgradeMenu upMenu;
	public bool activeMenu;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		firingTimer = firingDelay;
		loadedBullet = false;
		health = maxHealth;
		prestige = 0;

		upMenu = GetComponent<upgradeMenu> ();
		activeMenu = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// use up-arrow/W and down-arrow/S to accelerate and decelerate
		if (!activeMenu && ((playerOne && Input.GetKey(KeyCode.W)) || (!playerOne && Input.GetKey(KeyCode.UpArrow)))) {
			rb.AddForce(transform.up * moveSpeed);
		}
		else if (!activeMenu && ((playerOne && Input.GetKey(KeyCode.S)) || (!playerOne && Input.GetKey(KeyCode.DownArrow)))) {
			rb.AddForce(-transform.up * moveSpeed/4);
		}

		// use left-arrow/A and right-arrow/D to turn
		if (!activeMenu && ((playerOne && Input.GetKey (KeyCode.A)) || (!playerOne && Input.GetKey (KeyCode.LeftArrow)))) {
			transform.Rotate (0, 0, Time.deltaTime * rotationSpeed);
			rb.velocity = rb.velocity.magnitude * transform.up;
		}
		else if (!activeMenu && ((playerOne && Input.GetKey (KeyCode.D)) || (!playerOne && Input.GetKey (KeyCode.RightArrow)))) {
			transform.Rotate (0, 0, -Time.deltaTime * rotationSpeed);
			rb.velocity = rb.velocity.magnitude * transform.up;
		}

		// if you run out of health, you die!
		if (health <= 0) {
			/* figure out what code to put here! */
		}
	}

	void Update () {
		// check if you want to fire the cannons!
		// you can hold the fire button down to fire at regular intervals, or tap it and fire automatically at the next interval
		if (firingTimer > 0) {
			firingTimer -= Time.deltaTime;
			if (!activeMenu && !loadedBullet && ((!playerOne && Input.GetKeyDown (KeyCode.Space)) || (playerOne && Input.GetKeyDown (KeyCode.F)))) {
				loadedBullet = true;
			}
		} else if (!activeMenu && (!playerOne && Input.GetKeyDown (KeyCode.Space)) || (playerOne && Input.GetKeyDown (KeyCode.F)) || loadedBullet) {
			FireCannons ();
			firingTimer = firingDelay;
			loadedBullet = false;
		}

		// open the menu when the player presses the menu button!
		if (!activeMenu && ((!playerOne && Input.GetKeyUp (KeyCode.Return)) || (playerOne && Input.GetKeyUp (KeyCode.E)))) {
			upMenu.OpenMenu ();
			activeMenu = true;
		}
	}

	// fire the cannons!
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
