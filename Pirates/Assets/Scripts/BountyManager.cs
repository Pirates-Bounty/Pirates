﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class BountyManager : NetworkBehaviour {

	public const int BASE_BOUNTY = 100;

	public SyncListInt playerBounties = new SyncListInt();
	public SyncListInt killStreak = new SyncListInt();

	private GameObject bountyPanel;
	private List<GameObject> bountyTexts = new List<GameObject>();
	private Canvas canvas;
	private Font font;
    public GameObject spawnPoint;
    public int maxResources = 40;
    public GameObject resourcePrefab;
    private GameObject MapGen;

	// Use this for initialization
	void Start () {

        MapGen = GameObject.FindGameObjectWithTag("mapGen");
        //maxResources = Mathf.RoundToInt((width + height) / 50);
        spawnPoints(MapGen.GetComponent<MapGenerator>());
		font = Resources.Load<Font>("Art/Fonts/riesling");

        Random.InitState(System.DateTime.Now.Millisecond);
        for (int i = 0; i < maxResources; i++)
        {
            CmdSpawnResource();
        }

        if (!isLocalPlayer) {
			return;
		}




        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		if (canvas != null) {
			CreateBountyPanel ();
			//print ("makin' a bounty board");
		}
	}


    [Command]
    void CmdSpawnResource()
    {
        if (!isServer)
        {
            return;
        }
        ClientScene.RegisterPrefab(resourcePrefab);
        GameObject instantiatedResource = Instantiate(resourcePrefab, new Vector2(Random.Range(-MapGen.GetComponent<MapGenerator>().width / 2, MapGen.GetComponent<MapGenerator>().width / 2), Random.Range(-MapGen.GetComponent<MapGenerator>().height / 2, MapGen.GetComponent<MapGenerator>().height / 2)), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(instantiatedResource);
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




    // Update is called once per frame
    void Update () {
		if (canvas == null) {
			canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			if (canvas != null) {
				CreateBountyPanel ();
				//print ("makin' a bounty board (late)");
			}
		}

		if (playerBounties.Count > 0) {
			Player[] playerList = FindObjectsOfType<Player> ();

			for (int i = 0; i < playerList.Length; i++) {
				if (isServer) {
					int upgradeBounty = 10 * (int)Mathf.Floor (playerList [i].lowUpgrades / 2)
					                   + 25 * playerList [i].midUpgrades
					                   + 100 * playerList [i].highUpgrades;
					int killStreakBounty = 100 * killStreak [playerList [i].playerID];
					float bonusMod = 1f;
					if (playerList [i].playerID == GetHighestBounty ()) {
						bonusMod = 1.2f;
					}
					playerBounties [playerList [i].playerID] = (int)((BASE_BOUNTY + upgradeBounty + killStreakBounty)*bonusMod);
				}

				if (bountyTexts.Count <= i) {
					if (bountyPanel != null) {
						//print ("postin' a bounty (late)");
						bountyTexts.Add (UI.CreateText ("Bounty Text " + i, "Player " + i + " | " + playerBounties [i] + "g", font, Color.black, 24, bountyPanel.transform,
							Vector3.zero, new Vector2 (0.1f, 1.0f / 4f * (3-i)), new Vector2 (0.9f, 1.0f / 4f * (4-i)), TextAnchor.MiddleCenter, true));
					}
				} else {
					bountyTexts [i].GetComponent<Text> ().text = "Player " + (i+1) + "  |  " + playerBounties [i];
				}
			}
		}
	}

	public int AddID () {
		int newID = playerBounties.Count;
		playerBounties.Add (100);
		killStreak.Add (0);
		if (bountyPanel != null) {
			bountyTexts.Add (UI.CreateText ("Bounty Text " + newID, "Player " + newID + " | " + playerBounties [newID] + "g", font, Color.black, 24, bountyPanel.transform,
				Vector3.zero, new Vector2 (0.1f, 1.0f / 5f * newID), new Vector2 (0.9f, 1.0f / 5f * (newID+1)), TextAnchor.MiddleCenter, true));
		}
		/* need to adjust this later to deal with a more variable number of players */
		return newID;
	}

	public void ReportHit (int loser, int winner) {
		if (killStreak [loser] >= 5) {
			killStreak [winner] += 2;
		} else {
			killStreak [winner] += 1;
		}
		killStreak [loser] = 0;
		//playerBounties [loser] = 100;

		Player[] playerList = FindObjectsOfType<Player> ();
		for (int i = 0; i < playerList.Length; i++) {
			if (playerList [i].playerID == winner) {
				playerList[i].AddGold(playerBounties[loser]);
			}
		}
	}


	private void CreateBountyPanel() {
		bountyPanel = UI.CreatePanel("Bounty Panel", null, new Color(1.0f, 1.0f, 1.0f, 0.65f), canvas.transform,
			Vector3.zero, new Vector2(0.02f, 0.75f), new Vector3(0.18f, 0.95f));
	}

	private int GetHighestBounty() {
		int highestID = 0;
		for (int i = 1; i < playerBounties.Count; i++) {
			if (playerBounties [i] > playerBounties [highestID]) {
				highestID = i;
			}
		}
		return highestID;
	}
}