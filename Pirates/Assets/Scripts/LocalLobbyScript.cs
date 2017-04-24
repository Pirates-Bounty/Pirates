using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalLobbyScript : MonoBehaviour
{

    public bool mapsCount = false;
    public bool tileMapsCount = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!mapsCount)
        {
            GameObject[] maps = GameObject.FindGameObjectsWithTag("MapGenTopLevel");
            if (maps.Length > 1)
            {
                for (int i = 1; i < maps.Length; i++)
                {
                    Destroy(maps[i]);
                }
                MapGenerator m = GameObject.FindGameObjectWithTag("MapGenTopLevel").GetComponentInChildren<MapGenerator>();
                if(m != null)
                {
                    m.ClearMap();
                }
                
            }

            else if (maps.Length == 1)
            {
                mapsCount = true;
            }
        }

        if (!tileMapsCount)
        {
            GameObject[] tileMaps = GameObject.FindGameObjectsWithTag("TileMap");
            if (tileMaps.Length > 1)
            {
                for (int i = 1; i < tileMaps.Length; i++)
                {
                    Destroy(tileMaps[i]);
                }
            }

            else if (tileMaps.Length == 1)
            {
                tileMapsCount = true;
            }
        }
    }
}
