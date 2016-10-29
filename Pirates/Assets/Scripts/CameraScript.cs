using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z) ;
	}
}
