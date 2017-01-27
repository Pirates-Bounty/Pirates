using UnityEngine;
using System.Collections;

public class PlayerInRangeScirpt : MonoBehaviour {

    bool enemyPresence;
	// Use this for initialization
	void Awake () {
        enemyPresence = false;
	}
	
    private void OnTriggerStay2D(Collider2D other)
    {
        enemyPresence = (other.tag == "Player");
    }
}
