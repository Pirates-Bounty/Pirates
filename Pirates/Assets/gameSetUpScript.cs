using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class gameSetUpScript : MonoBehaviour {


    public GameObject spawnPoint;
    public MapGenerator mapGen;
	// Use this for initialization
	void Start () {
        spawnPoints(mapGen);
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void spawnPoints(MapGenerator mg)
    {
        if (GameObject.FindGameObjectsWithTag("spawner") != null)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("spawner"))
            {
                Destroy(g);
            }
        }
        int rad = (mg.width / 2) - 5;
        float deg = 90;
        if (LobbyManager.numPlayers != 0)
        {
            deg = 360 / LobbyManager.numPlayers;
        }


        //Loop through the players and spawn a spawn point for each player along the circle
        for (int i = 0; i < LobbyManager.numPlayers; i++)
        {
            bool spawnable = false;
            GameObject spawn = Instantiate(spawnPoint, transform.position, Quaternion.identity) as GameObject;
            int x = (int)(rad * Mathf.Cos(deg * i));
            int y = (int)(rad * Mathf.Sin(deg * i));
            //Checks to see if a good spot to spawn the spawnPoints
            while (spawnable)
            {
                bool resetLoop = false;
                for (int j = x - mg.quadWidth / 2; j < x + mg.quadWidth / 2; j++)
                {
                    for (int k = y - mg.quadHeight / 2; k < y + mg.quadHeight / 2; k++)
                    {

                        if (mg.map[j, k] != (int)TileType.WATER)
                        {
                            x -= x / Mathf.Abs(x);
                            y -= y / Mathf.Abs(x);
                            resetLoop = true;
                            break;
                        }
                        if (resetLoop)
                        {
                            break;
                        }
                    }
                }
                if (!resetLoop)
                {
                    spawnable = true;
                }
            }
            spawn.transform.position = new Vector2(x, y);
            Vector3 dir = -spawn.transform.position;
            dir = dir.normalized;
            spawn.transform.up = dir;
            spawn.GetComponent<SpawnScript>().spawned = true;
            NetworkServer.Spawn(spawn);
        }


    }
}
