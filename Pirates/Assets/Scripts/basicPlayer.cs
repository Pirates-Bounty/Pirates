using UnityEngine;
using System.Collections;

public class basicPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;

		transform.Rotate(0, 0, -x);
		transform.Translate(0, z, 0);
	}
}
