using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GameObject.FindGameObjectWithTag(gameObject.tag) != null)
        {

            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
