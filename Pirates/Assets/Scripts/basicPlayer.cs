using UnityEngine;
using System.Collections;

public class basicPlayer : MonoBehaviour {

	public Rigidbody2D rb;

	public float rotationSpeed; //150.0f
	public float moveSpeed;		//	5.0f


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
		// rb.MoveRotation(rb.rotation * deltaRotation);
		// rb.AddForce(transform.forward * thrust);


		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
			rb.AddForce(Input.GetAxis("Vertical") * transform.up * moveSpeed);
		}

		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			rb.AddForce(Input.GetAxis("Vertical") * transform.up * moveSpeed/6);
		}

     	// rb.AddForce(Input.GetAxis("Horizontal") * transform.right * rotationSpeed);
		// rb.AddForce(transform.up * thrust);

   //   	if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
   //   		Debug.Log("left");
			// rb.MoveRotation(rb.rotation + rotationSpeed * Time.fixedDeltaTime);
   //   	} 	
   //   	else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
   //   		Debug.Log("right");
   //   		rb.MoveRotation(-(rb.rotation + rotationSpeed * Time.fixedDeltaTime));
   //   	}
 		//apply some sort of rotation

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		// var z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
		transform.Rotate(0, 0, -x);

		// var velocityDirection = transform.InverseTransformDirection(rb.velocity);
			// from -90 to 90
		// if (velocityDirection.z > 0) {
		// Debug.Log(rb.velocity.y);
		// Debug.Log(rb.rotation);
		// if (rb.rotation > -90 && rb.rotation < 90) {
		// 	if (rb.velocity.y >= 0) {
		// 		rb.velocity = rb.velocity.magnitude * transform.up;
		// 	} 
		// 	else if (rb.velocity.y < 0) {
		// 		rb.velocity = rb.velocity.magnitude * -transform.up;
		// 	}
		// } 
		// else if (rb.rotation <= -90 || rb.rotation >= 90) {
		// 	if (rb.velocity.y >= 0) {
		// 		rb.velocity = rb.velocity.magnitude * -transform.up;
		// 	} 
		// 	else if (rb.velocity.y < 0) {
		// 		rb.velocity = rb.velocity.magnitude * transform.up;
		// 	}
		// }
		rb.velocity = rb.velocity.magnitude * transform.up;

			// v = rb.velocity.magnitude;
 		// 	rb.velocity = transform.forward*v; //change this from forward if something else should be
		// }
		// else if (velocityDirection.z < 0) {
		// 	Debug.Log("2");

		// 	rb.velocity = rb.velocity.magnitude * -transform.up;
		// }
        // rb.velocity = Quaternion.AngleAxis(rb.rotation, Vector3.up) * rb.velocity;
		// transform.Translate(0, z, 0);


	}
}
