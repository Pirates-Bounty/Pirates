using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class BountyManager : NetworkBehaviour {
    public const int BASE_BOUNTY = 200;
    public const int MAX_BOUNTY = 1000;
    public const int MAX_PLAYERS = 20;
    private GameObject bountyPanel;
    private Canvas canvas;
    private Font font;
    private int fontSize = 10;
    public GameObject spawnPoint;
    private int maxResources = 40;
    public GameObject resourcePrefab;
    private GameObject MapGen;
    public Player[] playerList = new Player[LobbyManager.numPlayers];
    private GameObject bountyBoard;
    private RectTransform bountyBoardRect;
    public float iconHeight = 0.1f;
    public float iconPadding = 15f;
    public float iconStartY = 0.82f;
    private GameObject broadcastText;
    public bool victoryUndeclared;
    private bool createdPlayerIcons;
    private Sprite iconSprite;
    private GameObject[] playerIconGOs = new GameObject[LobbyManager.numPlayers];
    private int currentIndex = 0;
    public static BountyManager Instance;

    // Use this for initialization
    void Start() {
        if (!Instance) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        playerList = FindObjectsOfType<Player>();
        victoryUndeclared = true;
        MapGen = GameObject.FindGameObjectWithTag("mapGen");
        maxResources = (int)(MapGen.GetComponent<MapGenerator>().maxResources);
        //maxResources = Mathf.RoundToInt((width + height) / 50);

        font = Resources.Load<Font>("Art/Fonts/Amarillo");
        iconSprite = Resources.Load<Sprite>("Art/Lobby/In Game UI/PlayerIndicator");
        bountyBoard = GameObject.Find("Canvas/Bounty Board");
        bountyBoard.SetActive(false);

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        Random.InitState(System.DateTime.Now.Millisecond);
        for (int i = 0; i < maxResources; i++) {
            if (isServer) {
                CmdSpawnResource();
            }
        }
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

    // Update is called once per frame
    void Update() {

        if (Input.GetKey(KeyCode.Tab)) {
            bountyBoard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab)) {
            bountyBoard.SetActive(false);
        }

        if (playerList.Length < LobbyManager.numPlayers) {
            playerList = FindObjectsOfType<Player>();
        }
            for (int i = 0; i < playerList.Length; i++) {

                if (!createdPlayerIcons) {
                    if(playerIconGOs[i] == null) {
                        GameObject playerIcon = new GameObject("Player Icon " + (i + 1));
                        playerIcon.transform.parent = bountyBoard.transform;
                        Image image = playerIcon.AddComponent<Image>();
                        image.sprite = iconSprite;
                        RectTransform rect = playerIcon.GetComponent<RectTransform>();
                        rect.anchorMin = new Vector2((CalculateWorth(playerList[i]) - iconPadding) / (float)MAX_BOUNTY, iconStartY - (i + 1) * iconHeight);
                        rect.anchorMax = new Vector2((CalculateWorth(playerList[i]) + iconPadding) / (float)MAX_BOUNTY, iconStartY - i * iconHeight);
                        rect.offsetMin = Vector3.zero;
                        rect.offsetMax = Vector3.zero;
                        playerIconGOs[i] = playerIcon;
                    }
                } else {
                    RectTransform rect = playerIconGOs[i].GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2((CalculateWorth(playerList[i]) - iconPadding) / (float)MAX_BOUNTY, iconStartY - (i + 1) * iconHeight);
                    rect.anchorMax = new Vector2((CalculateWorth(playerList[i]) + iconPadding) / (float)MAX_BOUNTY, iconStartY - i * iconHeight);
                    rect.offsetMin = Vector3.zero;
                    rect.offsetMax = Vector3.zero;
                }

                if (victoryUndeclared && CalculateWorth(playerList[i]) >= MAX_BOUNTY) {
                    StartCoroutine(DeclareVictory(playerList[i].ID));
                    victoryUndeclared = false;
                }
        }
        createdPlayerIcons = true;
    }

    public int RegisterPlayer(Player player) {
        int prevIndex = currentIndex;
        playerList[currentIndex++] = player;
        return prevIndex;
    }

    [Command]
    public void CmdReportKill(int victimID, int killerID) {
        Player killer = playerList[killerID];
        killer.AddGold(CalculateWorth(playerList[victimID]));
        killer.kills++;
        if(killer.streak < 0) {
            killer.streak = 1;
        } else {
            killer.streak++;
        }
        broadcastText = UI.CreateText("Broadcast", "Player " + (killerID + 1) + " has slain Player " + (victimID + 1),
            font, Color.black, 72, canvas.transform, Vector3.zero, new Vector3(0.1f, 0.5f), new Vector3(0.9f, 0.7f), TextAnchor.MiddleCenter, true);
        Destroy(broadcastText, 5f);
    }

    public static int CalculateWorth(Player p) {
        return BASE_BOUNTY + GetShipValue(p) + CalculateStreakValue(p) + CalculateKDRValue(p);
    }
    static int GetShipValue(Player p) {
        // for now, returns 1/10th of the player's gold investment into their ship
        return p.lowUpgrades + p.midUpgrades * 5 + p.highUpgrades * 20;
    }
    static int CalculateKDRValue(Player p) {
        if(p.deaths != 0) {
            float kdr = p.kills / (float)p.deaths;
            if(kdr < 1) {
                return 0;
            }
            int value = (int)kdr * 10;
            value = Mathf.Clamp(value, 0, 100);
            return value;
        } else {
            int value = p.kills * 20;
            value = Mathf.Clamp(value, 0, 100);
            return value;
        }
    }
    static int CalculateStreakValue(Player p) {
        if(p.streak >= 5) {
            return 100;
        } else if(p.streak >= 3) {
            return 50;
        } else if(p.streak >= 0) {
            return 0;
        } else {
            return 10 * p.streak;
        }
    }
    public Player GetLeader() {
        if(playerList.Length > 0) {
            Player p = playerList[0];
            int bounty = CalculateWorth(p);
            for (int i = 1; i < playerList.Length; i++) {
                int newWorth = CalculateWorth(playerList[i]);
                if (CalculateWorth(playerList[i]) > bounty) {
                    bounty = newWorth;
                    p = playerList[i];
                }
            }
            return p;
        }
        return null;
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
