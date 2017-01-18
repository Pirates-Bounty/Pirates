using UnityEngine;
using System.Collections;

public class PlayerInRangeScirpt : MonoBehaviour {


    int PlayerCount = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerInRange")
        {
            PlayerCount++;
            //Auido change
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "PlayerInRange")
        {
            
        }
    }
}
