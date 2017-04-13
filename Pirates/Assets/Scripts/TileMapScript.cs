using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapScript : MonoBehaviour {

    public TileMapScript Instance;
	// Use this for initialization
	void Start () {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
