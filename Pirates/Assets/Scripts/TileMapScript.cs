using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapScript : MonoBehaviour {

    public TileMapScript Instance;
    private Tilemap tMap;
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
        tMap = GetComponentInChildren<Tilemap>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
