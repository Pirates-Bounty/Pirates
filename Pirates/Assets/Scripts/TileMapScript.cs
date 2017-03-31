using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapScript : MonoBehaviour{

    private Tilemap tMap;
    private tilebase tile;
    private ITilemap t;
    private int[] pos;
	// Use this for initialization
	void Awake () {
        tMap = GetComponent<Tilemap>();
        for(int i = 0; i < 2; i++)
        {
            
            
            tMap.SetTile(new Vector3Int(i, i, 0), tile);
            tMap.RefreshTile(new Vector3Int(i, i, 0));

            Debug.Log(tile.transform);
            Debug.Log(tile.sprite);
           
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
