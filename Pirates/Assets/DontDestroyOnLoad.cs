using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    // Use this for initialization

    DontDestroyOnLoad Instance;
	void Start () {
        if (Instance != null)
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
