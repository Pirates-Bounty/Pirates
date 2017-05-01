using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class gameSetUpScript : MonoBehaviour {


    public GameObject spawnPoint;
    private MapGenerator mapGen;
    private LobbyTopPanel inGameMenuPanel;
    private int numPlayers;
    // Use this for initialization
    void Start() {
        mapGen = GameObject.FindGameObjectWithTag("mapGen").GetComponent<MapGenerator>();
        spawnPoints(mapGen);
        inGameMenuPanel = GameObject.Find("InGameMenu").GetComponent<LobbyTopPanel>();
        numPlayers = inGameMenuPanel.numberPlayers;
    }

    // Update is called once per frame
    void Update() {
    }


    public void spawnPoints(MapGenerator mg) {
        inGameMenuPanel = GameObject.Find("InGameMenu").GetComponent<LobbyTopPanel>();
        numPlayers = inGameMenuPanel.numberPlayers;
        if (GameObject.FindGameObjectsWithTag("spawner") != null) {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("spawner")) {
                Destroy(g);
            }
        }
        //int rad = (mg.width / 2) - 20;
        //float deg = 90;
        //deg *= Mathf.Deg2Rad;
        //if (LobbyManager.singleton.numPlayers != 0) {
        //    deg = 360 / LobbyManager.singleton.numPlayers;
        //    deg *= Mathf.Deg2Rad;
        //}
        Debug.Log("SpawnPoints");
        Debug.Log(numPlayers);
        //Loop through the players and spawn a spawn point for each player along the circle
        for (int i = 0; i < numPlayers; i++) {
            //bool spawnable = false;

            GameObject spawn = Instantiate(spawnPoint, mapGen.GetRandLocAwayFromLand(5), Quaternion.identity) as GameObject;
            //int qWidth = (x > 0 ? mg.quadWidth : -mg.quadWidth);
            //int qHeight = (y > 0 ? -mg.quadHeight : mg.quadHeight);
            //Checks to see if a good spot to spawn the spawnPoints
            //while (spawnable) {
            //    bool resetLoop = false;

            //    for (int j = x - qWidth / 2; j < x + qWidth / 2; j++) {
            //        for (int k = y - qHeight / 2; k < y + qHeight / 2; k++) {
            //            if (k < 0)
            //            {
            //                k = Mathf.Abs(k) + mg.height/2; 
            //            }
            //            if (j < 0)
            //            {
            //                j = Mathf.Abs(j) + mg.width/2;
            //            }
            //            if (mg.map[j, k] != (int)TileType.WATER) {
            //                x  = x - qWidth;
            //                y -= y - qHeight;
            //                resetLoop = true;
            //                break;
            //            }


            //            if (k > mg.height/2)
            //            {
            //                k = -(k - mg.height / 2);
            //            }
            //            if (j > mg.width)
            //            {
            //                j = -(j - mg.width / 2);
            //            }
            //        }
            //        if (resetLoop)
            //        {
            //            break;
            //        }
            //    }
            //    if (!resetLoop) {
            //        spawnable = true;
            //    }
            //}
            Vector3 dir = -spawn.transform.position;
            dir = dir.normalized;
            spawn.transform.up = dir;
            NetworkServer.Spawn(spawn);
        }


    }
}
