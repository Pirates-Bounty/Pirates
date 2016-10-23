using UnityEngine;
using System.Collections;

public class landPlayer : MonoBehaviour {

	public Rigidbody2D rb;
	public float runSpeed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
			//display up sprite
		}
		else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			//display left sprite
		}
		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			//display down sprite
		}
		else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {			
			//display right sprite
		}
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * runSpeed;
		var y = Input.GetAxis("Vertical") * Time.deltaTime * runSpeed;
		transform.Translate(x, y, 0);
	}
}
