using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class gameSetUpScript : MonoBehaviour {


    public GameObject spawnPoint;
    private MapGenerator mapGen;
    // Use this for initialization
    void Start() {

        mapGen = GameObject.FindGameObjectWithTag("mapGen").GetComponent<MapGenerator>();
        spawnPoints(mapGen);
    }

    // Update is called once per frame
    void Update() {
    }


    public void spawnPoints(MapGenerator mg) {
        if (GameObject.FindGameObjectsWithTag("spawner") != null) {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("spawner")) {
                Destroy(g);
            }
        }
        int rad = (mg.width / 2) - 20;
        float deg = 90;
        deg *= Mathf.Deg2Rad;
        if (LobbyManager.numPlayers != 0) {
            deg = 360 / LobbyManager.numPlayers;
            deg *= Mathf.Deg2Rad;
        }

        //Loop through the players and spawn a spawn point for each player along the circle
        for (int i = 0; i < LobbyManager.numPlayers; i++) {
            bool spawnable = false;
            GameObject spawn = Instantiate(spawnPoint, transform.position, Quaternion.identity) as GameObject;
            int x = (int)(rad * Mathf.Cos(deg * i));
            int y = (int)(rad * Mathf.Sin(deg * i));
            int qWidth = (x > 0 ? mg.quadWidth : -mg.quadWidth);
            int qHeight = (y > 0 ? -mg.quadHeight : mg.quadHeight);
            //Checks to see if a good spot to spawn the spawnPoints
            while (spawnable) {
                bool resetLoop = false;

                for (int j = x - qWidth / 2; j < x + qWidth / 2; j++) {
                    for (int k = y - qHeight / 2; k < y + qHeight / 2; k++) {
                        if (k < 0)
                        {
                            k = Mathf.Abs(k) + mg.height/2; 
                        }
                        if (j < 0)
                        {
                            j = Mathf.Abs(j) + mg.width/2;
                        }
                        if (mg.map[j, k] != (int)TileType.WATER) {
                            x  = x - qWidth;
                            y -= y - qHeight;
                            resetLoop = true;
                            break;
                        }


                        if (k > mg.height/2)
                        {
                            k = -(k - mg.height / 2);
                        }
                        if (j > mg.width)
                        {
                            j = -(j - mg.width / 2);
                        }
                    }
                    if (resetLoop)
                    {
                        break;
                    }
                }
                if (!resetLoop) {
                    spawnable = true;
                }
            }
            spawn.transform.position = new Vector2(x, y);
            Vector3 dir = -spawn.transform.position;
            dir = dir.normalized;
            spawn.transform.up = dir;
            NetworkServer.Spawn(spawn);
        }


    }
}
