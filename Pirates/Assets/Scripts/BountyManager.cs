using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class BountyManager : NetworkBehaviour {

	public bool displayLocal;

	public const int BASE_BOUNTY = 100;

	public SyncListInt playerBounties = new SyncListInt();
	public SyncListInt killStreak = new SyncListInt();
	public SyncListInt scoreOrder = new SyncListInt();

	//public int localID;
	private GameObject bountyPanel;
	private List<GameObject> bountyTexts = new List<GameObject>();
	private Canvas canvas;
	private Font font;
    public GameObject spawnPoint;
    private int maxResources = 40;
    public GameObject resourcePrefab;
    private GameObject MapGen;
	private Player[] playerList;
    private GameObject bountyBoard;
    private RectTransform bountyBoardRect;

	public bool victoryUndeclared;

	// Use this for initialization
	void Start () {
		playerList = FindObjectsOfType<Player> ();
		victoryUndeclared = true;
		MapGen = GameObject.FindGameObjectWithTag("mapGen");
        maxResources = (int)(MapGen.GetComponent<MapGenerator>().maxResources);
        //maxResources = Mathf.RoundToInt((width + height) / 50);
        
		font = Resources.Load<Font>("Art/Fonts/riesling");
        bountyBoard = GameObject.Find("Canvas/UI/Bounty Board");
        bountyBoardRect = bountyBoard.GetComponent<RectTransform>();

        Random.InitState(System.DateTime.Now.Millisecond);
		for (int i = 0; i < maxResources; i++) {
			if (isServer) {
				CmdSpawnResource ();
			}
		}

        if (!isLocalPlayer) {
			return;
		}

		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		if (canvas != null) {
			CreateBountyPanel ();
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
        GameObject instantiatedResource = Instantiate(resourcePrefab, MapGen.GetComponent<MapGenerator>().getRandWaterTile(), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(instantiatedResource);
    }


	/*[Command]
	void CmdCreateID ()
	{
		int newID = playerBounties.Count;
		playerBounties.Add (100);
		killStreak.Add (0);
		if (bountyPanel != null) {
			int playerCount = playerBounties.Count;
			bountyTexts.Add (UI.CreateText ("Bounty Text " + newID, "Player " + newID + " | " + playerBounties [newID] + "g", font, Color.black, 24, bountyPanel.transform,
				Vector3.zero, new Vector2 (0.1f, 1f/playerCount * (playerCount-(newID+1))), new Vector2 (0.9f, 1f/playerCount * (playerCount-newID)), TextAnchor.UpperLeft, true));
		}
	}*/


	void OrderBounties () {
		bool finishedSort = false;
		int[] orders = new int[scoreOrder.Count];
		for (int i = 0; i < scoreOrder.Count; i++) {
			orders [scoreOrder [i]] = i;
		}
		for (int i = scoreOrder.Count-1; !finishedSort && (i > 0); i--) {
			finishedSort = true;
			for (int j = 0; j < i; j++) {
				if (playerBounties [orders [j]] < playerBounties [orders [j + 1]]) {
					int temp = orders [j];
					orders [j] = orders [j + 1];
					orders [j + 1] = temp;
					finishedSort = false;
				}
			}
		}
		for (int i = 0; i < scoreOrder.Count; i++) {
			scoreOrder[orders[i]] = i;
			//print ("Player " + orders [i] + " is ranked " + i + " with " + playerBounties [orders [i]] + "g");
		}
	}


    
	// Update is called once per frame
    void Update () {
		displayLocal = isLocalPlayer;

		if (canvas == null) {
			canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			if (canvas != null) {
				CreateBountyPanel ();
				//print ("makin' a bounty board (late)");
			}
		}

		if (playerList.Length < LobbyManager.numPlayers) {
			playerList = FindObjectsOfType<Player> ();
		}

		if (playerBounties.Count > 0) {

			if (isServer) {
				OrderBounties ();
			}
			for (int i = 0; i < playerList.Length; i++) {
				if (isServer) {
					int upgradeBounty = 10 * (int)Mathf.Floor (playerList [i].lowUpgrades / 2)
					                   + 25 * playerList [i].midUpgrades
					                   + 100 * playerList [i].highUpgrades;
					int killStreakBounty = 15 * killStreak [playerList [i].playerID];
					float bonusMod = 1f;
					/*if (playerList [i].playerID == GetHighestBounty ()) {
						bonusMod = 1.2f;
					}*/
					playerBounties [playerList [i].playerID] = (int)((BASE_BOUNTY + upgradeBounty + killStreakBounty)*bonusMod);
				}

				if (victoryUndeclared && playerBounties [playerList [i].playerID] >= 1000) {
					StartCoroutine(DeclareVictory (playerList [i].playerID));
					victoryUndeclared = false;
				}

				if (bountyTexts.Count <= i) {
					if (bountyPanel != null) {
						int playerCount = playerList.Length;
						for (int j=0; j < bountyTexts.Count; j++)
						{
							/*GameObject oldText = bountyTexts [j];
							Text tx = oldText.GetComponent<Text> ();
							GameObject newText = UI.CreateText(oldText.name, tx.text, tx.font, tx.color, tx.fontSize, oldText.transform.parent, 
								Vector3.zero, new Vector2 (0.1f, 1f/playerCount * (playerCount-(scoreOrder[j]+1))), new Vector2 (0.9f, 1f/playerCount * (playerCount-scoreOrder[j])), TextAnchor.UpperLeft, true);
							bountyTexts [j] = newText;
							GameObject.Destroy (oldText);*/
							RectTransform rectMod = bountyTexts [j].GetComponent<RectTransform> ();
							rectMod.anchorMin = new Vector2 (0.1f, 1f / playerCount * (scoreOrder [j] + 0));
							rectMod.anchorMax = new Vector2 (0.9f, 1f / playerCount * (scoreOrder [j] + 1));
						}

						bountyTexts.Add (UI.CreateText ("Bounty Text " + i, "Player " + (i+1) + " | " + playerBounties [playerList[i].playerID] + "g", font, Color.black, 24, bountyPanel.transform,
							Vector3.zero, new Vector2 (0.1f, 1f/playerCount * (playerCount-(scoreOrder[i]+1))), new Vector2 (0.9f, 1f/playerCount * (playerCount-scoreOrder[i])), TextAnchor.UpperLeft, true));
					}
				} else {
					int playerCount = playerList.Length;
					bountyTexts [playerList[i].playerID].GetComponent<Text> ().text = "Player " + (i+1) + "  |  " + playerBounties [playerList[i].playerID] + "g";
					RectTransform rectMod = bountyTexts [playerList [i].playerID].GetComponent<RectTransform> ();
					rectMod.anchorMin = new Vector2 (0.1f, 1f / playerCount * (scoreOrder [i] + 0));
					rectMod.anchorMax = new Vector2 (0.9f, 1f / playerCount * (scoreOrder [i] + 1));
					if (playerList[i].isLocalPlayer) {
						bountyTexts [playerList[i].playerID].GetComponent<Text> ().color = Color.red;
					}
				}
				if (GetHighestBounty () == playerList [i].playerID) {
					bountyTexts [playerList [i].playerID].GetComponent<Text> ().text += " +" + 0.2f * playerBounties [playerList [i].playerID] + "g";
				}
			}
		}

		/*if (Input.GetKeyDown (KeyCode.Q)) {
			StartCoroutine(DeclareVictory (0));
		}*/
	}

	public int AddID () {
		//localID = CmdCreateID ();
		//return localID;

		int newID = playerBounties.Count;
		playerBounties.Add (100);
		killStreak.Add (0);
		scoreOrder.Add (newID);
		if (bountyPanel != null) {
			int playerCount = playerList.Length;

			for (int j=0; j < bountyTexts.Count; j++)
			{
				GameObject oldText = bountyTexts [j];
				Text tx = oldText.GetComponent<Text> ();
				GameObject newText = UI.CreateText(oldText.name, tx.text, tx.font, tx.color, tx.fontSize, oldText.transform.parent, 
					Vector3.zero, new Vector2 (0.1f, 1f/playerCount * (playerCount-(j+1))), new Vector2 (0.9f, 1f/playerCount * (playerCount-j)), TextAnchor.UpperLeft, true);
				bountyTexts [j] = newText;
				GameObject.Destroy (oldText);
			}

			bountyTexts.Add (UI.CreateText ("Bounty Text " + newID, "Player " + (newID+1) + " | " + playerBounties [newID] + "g", font, Color.black, 24, bountyPanel.transform,
				Vector3.zero, new Vector2 (0.1f, 1f / playerCount * (playerCount - (newID + 1))), new Vector2 (0.9f, 1f / playerCount * (playerCount - newID)), TextAnchor.UpperLeft, true));
			/*bountyTexts.Add (UI.CreateText ("Bounty Text " + newID, "Player " + newID + " | " + playerBounties [newID] + "g", font, Color.black, 24, bountyPanel.transform,
				Vector3.zero, new Vector2 (0.1f, 1.0f / 5f * newID), new Vector2 (0.9f, 1.0f / 5f * (newID+1)), TextAnchor.MiddleCenter, true));*/
		}
		return newID;
	}

	public void ReportHit (int loser, int winner) {
		if (playerBounties [winner] * 0.5f >= playerBounties [loser]) {
			killStreak [winner] += 1;
		} else if (playerBounties [winner] * 0.8f >= playerBounties [loser]) {
			killStreak [winner] += 3;
		} else if (playerBounties [winner] * 1.2f >= playerBounties [loser]) {
			killStreak [winner] += 5;
		} else if (playerBounties [winner] * 2f >= playerBounties [loser]) {
			killStreak [winner] += 7;
		} else {
			killStreak [winner] += 10;
		}
		killStreak [winner] += killStreak [loser] / 2;
		killStreak [loser] = 0;
		//playerBounties [loser] = 100;

		for (int i = 0; i < playerList.Length; i++) {
			if (playerList [i].playerID == winner) {
				float bonusMod = 1.0f;
				if (GetHighestBounty () == loser) {
					bonusMod = 1.2f;
				}
				playerList [i].AddGold ((int)(playerBounties [loser] * bonusMod));
			}
		}
	}


	private void CreateBountyPanel() {
		int playerCount = playerList.Length;
		bountyPanel = UI.CreatePanel("Bounty Panel", null, new Color(0.0f, 0.0f, 0.0f, 0f), canvas.transform,
			Vector3.zero, new Vector2(0.75f, 0.1f), new Vector3(1f, 0.2f + 0.1f * playerCount));
        bountyBoardRect.anchorMax = new Vector2(bountyBoardRect.anchorMax.x, 0.2f + 0.1f * playerCount);
	}

	public int GetHighestBounty() {
        int highestID = 0;
        for (int i = 1; i < playerBounties.Count; i++) {
            if (playerBounties [i] > playerBounties [highestID]) {
                highestID = i;
			}
        }
        return highestID;
		//return scoreOrder [0];
	}


	private IEnumerator DeclareVictory(int playerID) {
		// delcare the winning player to be the pirate king
		//print("Victory has been declared!");
		GameObject lastText = (UI.CreateText ("Victory Text", "Player " + (playerID+1) + " is the Pirate King!", font, Color.black, 100, canvas.transform,
			Vector3.zero, new Vector2 (0.1f, 0.1f), new Vector2 (0.9f, 0.9f), TextAnchor.MiddleCenter, true));
		//lastText.GetComponent<Text> ().resizeTextForBestFit = true;

		if (isServer) {
			for (int i = 0; i < playerList.Length; i++) {	
				playerList [i].dead = true;
			}
		}

		yield return new WaitForSeconds(5.0f);
		Navigator.Instance.LoadLevel("Menu");
	}
}
