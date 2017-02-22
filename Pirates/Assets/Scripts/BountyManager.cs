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
    public const int MAX_BOUNTY = 1000;
    public const int MAX_PLAYERS = 20;

    public SyncListInt playerBounties = new SyncListInt();
    public SyncListInt killStreak = new SyncListInt();
    public SyncListInt scoreOrder = new SyncListInt();

    //public int localID;
    public Sprite bountyBoardSprite;
    private GameObject bountyPanel;
    //public List<GameObject> bountyTexts = new List<GameObject>();
    private Canvas canvas;
    private Font font;
    private int fontSize = 10;
    public GameObject spawnPoint;
    private int maxResources = 40;
    public GameObject resourcePrefab;
    private GameObject MapGen;
    public Player[] playerList;
    private GameObject bountyBoard;
    private RectTransform bountyBoardRect;
    public float iconHeight = 0.1f;
    public float iconPadding = 15f;
    public float iconStartY = 0.82f;

    public bool victoryUndeclared;
    private bool createdPlayerIcons;
    private Sprite iconSprite;
    public GameObject[] playerIconGOs = new GameObject[LobbyManager.numPlayers];

    // Use this for initialization
    void Start() {
        playerList = FindObjectsOfType<Player>();
        victoryUndeclared = true;
        MapGen = GameObject.FindGameObjectWithTag("mapGen");
        maxResources = (int)(MapGen.GetComponent<MapGenerator>().maxResources);
        //maxResources = Mathf.RoundToInt((width + height) / 50);

        font = Resources.Load<Font>("Art/Fonts/Amarillo");
        iconSprite = Resources.Load<Sprite>("Art/Lobby/In Game UI/PlayerIndicator");
        bountyBoard = GameObject.Find("Canvas/Bounty Board");
        bountyBoard.SetActive(false);
        //bountyBoardRect = bountyBoard.GetComponent<RectTransform>();

        Random.InitState(System.DateTime.Now.Millisecond);
        for (int i = 0; i < maxResources; i++) {
            if (isServer) {
                CmdSpawnResource();
            }
        }

        if (!isLocalPlayer) {
            return;
        }

        //canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //if (canvas != null) {
        //    CreateBountyPanel();
        //}
    }


    [Command]
    void CmdSpawnResource() {
        if (!isServer) {
            return;
        }
        ClientScene.RegisterPrefab(resourcePrefab);
        GameObject instantiatedResource = Instantiate(resourcePrefab, MapGen.GetComponent<MapGenerator>().GetRandWaterTile(), Quaternion.identity) as GameObject;
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

    // Update is called once per frame
    void Update() {
        displayLocal = isLocalPlayer;

        if (Input.GetKey(KeyCode.Tab)) {
            bountyBoard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab)) {
            bountyBoard.SetActive(false);
        }

        if (playerList.Length < LobbyManager.numPlayers) {
            playerList = FindObjectsOfType<Player>();
        }

        if (playerBounties.Count > 0) {
            for (int i = 0; i < playerList.Length; i++) {
                int upgradeBounty = 5 * playerList[i].lowUpgrades
                                   + 25 * playerList[i].midUpgrades
                                   + 100 * playerList[i].highUpgrades;
                int killStreakBounty = 15 * killStreak[playerList[i].playerID];
                float bonusMod = 1f;
                playerBounties[playerList[i].playerID] = (int)((BASE_BOUNTY + upgradeBounty + killStreakBounty) * bonusMod);

                if (!createdPlayerIcons) {
                    GameObject playerIcon = new GameObject("Player Icon " + (i + 1));
                    playerIcon.transform.parent = bountyBoard.transform;
                    Image image = playerIcon.AddComponent<Image>();
                    image.sprite = iconSprite;
                    RectTransform rect = playerIcon.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2((playerBounties[i] - iconPadding) / (float)MAX_BOUNTY, iconStartY - (i + 1) * iconHeight);
                    rect.anchorMax = new Vector2((playerBounties[i] + iconPadding) / (float)MAX_BOUNTY, iconStartY - i * iconHeight);
                    rect.offsetMin = Vector3.zero;
                    rect.offsetMax = Vector3.zero;
                    playerIconGOs[i] = playerIcon;
                } else {
                    RectTransform rect = playerIconGOs[i].GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2((playerBounties[i] - iconPadding) / (float)MAX_BOUNTY, iconStartY - (i + 1) * iconHeight);
                    rect.anchorMax = new Vector2((playerBounties[i] + iconPadding) / (float)MAX_BOUNTY, iconStartY - i * iconHeight);
                    rect.offsetMin = Vector3.zero;
                    rect.offsetMax = Vector3.zero;
                }

                if (victoryUndeclared && playerBounties[playerList[i].playerID] >= MAX_BOUNTY) {
                    StartCoroutine(DeclareVictory(playerList[i].playerID));
                    victoryUndeclared = false;
                }
            }
        }
        createdPlayerIcons = true;
    }

    public int AddID() {

        int newID = playerBounties.Count;
        playerBounties.Add(100);
        killStreak.Add(0);
        scoreOrder.Add(newID);
        return newID;
    }

    public void ReportHit(int loser, int winner) {
        if (playerBounties[winner] * 0.5f >= playerBounties[loser]) {
            killStreak[winner] += 1;
        } else if (playerBounties[winner] * 0.8f >= playerBounties[loser]) {
            killStreak[winner] += 3;
        } else if (playerBounties[winner] * 1.2f >= playerBounties[loser]) {
            killStreak[winner] += 5;
        } else if (playerBounties[winner] * 2f >= playerBounties[loser]) {
            killStreak[winner] += 7;
        } else {
            killStreak[winner] += 10;
        }
        killStreak[winner] += killStreak[loser] / 2;
        killStreak[loser] = 0;
        //playerBounties [loser] = 100;

        for (int i = 0; i < playerList.Length; i++) {
            if (playerList[i].playerID == winner) {
                float bonusMod = 1.0f;
                if (GetHighestBounty() == loser) {
                    bonusMod = 1.2f;
                }
                playerList[i].AddGold((int)(playerBounties[loser] * bonusMod));
            }
        }
    }


    public int GetHighestBounty() {
        int highestID = 0;
        for (int i = 1; i < playerBounties.Count; i++) {
            if (playerBounties[i] > playerBounties[highestID]) {
                highestID = i;
            }
        }
        return highestID;
        //return scoreOrder [0];
    }


    private IEnumerator DeclareVictory(int playerID) {
        // declare the winning player to be the pirate king
        //print("Victory has been declared!");
        GameObject lastText = (UI.CreateText("Victory Text", "Player " + (playerID + 1) + " is the Pirate King!", font, Color.black, 100, canvas.transform,
            Vector3.zero, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, true));
        //lastText.GetComponent<Text> ().resizeTextForBestFit = true;

        if (isServer) {
            for (int i = 0; i < playerList.Length; i++) {
                playerList[i].dead = true;
            }
        }

        yield return new WaitForSeconds(5.0f);
        Navigator.Instance.LoadLevel("Menu");
    }
}
