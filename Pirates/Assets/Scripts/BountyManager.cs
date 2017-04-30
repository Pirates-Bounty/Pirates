using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class BountyManager : NetworkBehaviour {
    public const int BASE_BOUNTY = 200;
    public const int MAX_BOUNTY = 2;
    public const int MAX_PLAYERS = 20;
    private GameObject bountyPanel;
    private Canvas canvas;
    private Font font;
    private int fontSize = 10;
    public GameObject spawnPoint;
    private int maxResources = 40;
    public GameObject resourcePrefab;
    private GameObject MapGen;
    public Player[] playerList = new Player[LobbyManager.numberPlayers];
    private GameObject bountyBoard;
    private RectTransform bountyBoardRect;
    private GameObject scoreBar;
    public float iconHeight = 0.1f;
    public float iconPadding = 15f / 1000f;
    public float iconStartY = 0.82f;
    private GameObject broadcastText;
    public bool victoryUndeclared;
    private bool createdPlayerIcons;
    private Sprite iconSprite;
    private GameObject[] playerIconGOs = new GameObject[LobbyManager.singleton.numPlayers];
    private GameObject localPlayerIcon = null;
    public static BountyManager Instance;
    private int currentIndex = 0;
    public UpgradePanel upgradePanel;
    public GameObject Hill;
    //public int hillCheck;
    //public int hillSize;
    public Vector2 moveHillRange;
	private GameObject currHill;
    private GameObject hillRep;
    private RectTransform hillRepRect;
    private RectTransform minimapRect;
    private GameObject hillTimerText;

    public static bool kingOfTheHill = true;


    // Use this for initialization
    void Start() {
        Instance = this;
        playerList = FindObjectsOfType<Player>();
		playerIconGOs = new GameObject[LobbyManager.numberPlayers];
        //upgradePanel = FindObjectOfType<UpgradePanel>();
        victoryUndeclared = true;
        MapGen = GameObject.FindGameObjectWithTag("mapGen");
        maxResources = (int)(MapGen.GetComponent<MapGenerator>().maxResources);
        //maxResources = Mathf.RoundToInt((width + height) / 50);

        font = Resources.Load<Font>("Art/Fonts/Amarillo");
        iconSprite = Resources.Load<Sprite>("Art/Lobby/In Game UI/PlayerIndicator");
        hillRep = GameObject.Find("MainCanvas/Minimap/Hill");
        hillRepRect = hillRep.GetComponent<RectTransform>();
        minimapRect = GameObject.Find("MainCanvas/Minimap").GetComponent<RectTransform>();
        bountyBoard = GameObject.Find("MainCanvas/Bounty Board");
        bountyBoard.SetActive(false);
        scoreBar = GameObject.Find("MainCanvas/UI/ScoreBar");
        hillTimerText = GameObject.Find("MainCanvas/UI/HillTimerText");
        canvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();


        //StartCoroutine(MoveHill((int)moveHillRange.x,(int)moveHillRange.y));
        if (isServer) {
            CmdSpawnHill();
        }
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
        GameObject instantiatedResource = Instantiate(resourcePrefab, MapGen.GetComponent<MapGenerator>().GetRandLocAwayFromLand(5), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(instantiatedResource);
    }

    [Command]
    void CmdSpawnHill()
    {
        //ClientScene.RegisterPrefab(Hill);
        currHill = Instantiate(Hill, transform.position, Quaternion.identity) as GameObject;
        //currHill.transform.localScale *= hillSize;
        NetworkServer.Spawn(currHill);
        //UpdateMinimapHill();
    }

    /*[Command]
    public void CmdMoveHill()
    {
        if (currHill == null)
		{
			CmdSpawnHill();
		}
        currHill.transform.position = MapGen.GetComponent<MapGenerator>().GetRandHillLocation(hillCheck);
		RpcHideHill ();
        UpdateMinimapHill();
    }

	[ClientRpc]
	void RpcHideHill()
	{
		if (currHill == null) {
			currHill = GameObject.FindGameObjectWithTag ("Hill");
		}
		StartCoroutine (HideHill (25));
	}

    void UpdateMinimapHill() {
		hillRepRect.anchoredPosition = new Vector3(minimapRect.rect.width* currHill.transform.position.x, minimapRect.rect.height * currHill.transform.position.y, 1) / MapGen.GetComponent<MapGenerator>().width;
    }*/

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Tab)) {
            bountyBoard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab)) {
            bountyBoard.SetActive(false);
        }

        if (playerList.Length != LobbyManager.numberPlayers) {
            playerList = FindObjectsOfType<Player>();
        }
		if (playerIconGOs.Length < playerList.Length) {
			playerIconGOs = new GameObject[playerList.Length];
		}
        for (int i = 0; i < playerList.Length; i++) {
			
			if (playerIconGOs [i] == null) {
				GameObject playerIcon = new GameObject ("Player Icon " + (i + 1));
				playerIcon.transform.parent = bountyBoard.transform;
				Image image = playerIcon.AddComponent<Image> ();
				image.sprite = iconSprite;
				RectTransform rect = playerIcon.GetComponent<RectTransform> ();
				rect.anchorMin = new Vector2 (Mathf.Min (1.0f, CalculateWorth (playerList [i]) / (float)MAX_BOUNTY) - iconPadding, iconStartY - (i + 1) * iconHeight);
				rect.anchorMax = new Vector2 (Mathf.Min (1.0f, CalculateWorth (playerList [i]) / (float)MAX_BOUNTY) + iconPadding, iconStartY - i * iconHeight);
				rect.offsetMin = Vector3.zero;
				rect.offsetMax = Vector3.zero;
				playerIconGOs [i] = playerIcon;
			} else {
				RectTransform rect = playerIconGOs [i].GetComponent<RectTransform> ();
				rect.anchorMin = new Vector2 (Mathf.Min (1.0f, CalculateWorth (playerList [i]) / (float)MAX_BOUNTY) - iconPadding, iconStartY - (i + 1) * iconHeight);
				rect.anchorMax = new Vector2 (Mathf.Min (1.0f, CalculateWorth (playerList [i]) / (float)MAX_BOUNTY) + iconPadding, iconStartY - i * iconHeight);
				rect.offsetMin = Vector3.zero;
				rect.offsetMax = Vector3.zero;
			}

            if (playerList[i].isLocalPlayer) {
                if (localPlayerIcon == null) {
                    localPlayerIcon = new GameObject("Player Icon " + (i + 1));
                    localPlayerIcon.transform.parent = scoreBar.transform;
                    Image image = localPlayerIcon.AddComponent<Image>();
                    image.sprite = iconSprite;
                    RectTransform rect = localPlayerIcon.GetComponent<RectTransform>();
                    float iconPos = scoreBar.transform.position.y;
                    rect.anchorMin = new Vector2(Mathf.Min(1.0f, CalculateWorth(playerList[i]) / (float)MAX_BOUNTY) - iconPadding, 0f);
                    rect.anchorMax = new Vector2(Mathf.Min(1.0f, CalculateWorth(playerList[i]) / (float)MAX_BOUNTY) + iconPadding, 1f);
                    rect.offsetMin = Vector3.zero;
                    rect.offsetMax = Vector3.zero;
                } else {
                    RectTransform rect = localPlayerIcon.GetComponent<RectTransform>();
                    float iconPos = scoreBar.transform.position.y;
                    rect.anchorMin = new Vector2(Mathf.Min(1.0f, CalculateWorth(playerList[i]) / (float)MAX_BOUNTY) - iconPadding, 0f);
                    rect.anchorMax = new Vector2(Mathf.Min(1.0f, CalculateWorth(playerList[i]) / (float)MAX_BOUNTY) + iconPadding, 1f);
                    rect.offsetMin = Vector3.zero;
                    rect.offsetMax = Vector3.zero;
                }
            }

            if (victoryUndeclared && CalculateWorth(playerList[i]) >= MAX_BOUNTY) {
                victoryUndeclared = false;
                RpcStopCaptureSFX();
                StartCoroutine(DeclareVictory(playerList[i].ID));
                
            }
        }
        for (int i = 0; i < playerIconGOs.Length; ++i) {
            if (!playerIconGOs[i]) {
                createdPlayerIcons = false;
                break;
            } else {
                createdPlayerIcons = true;
            }
        }
    }

    public int RegisterPlayer(Player player) {
        /*int prevIndex = currentIndex;
        playerList[currentIndex++] = player;
        return prevIndex;*/
		for (int i = 0; i < playerList.Length; i++) {
			if (playerList [i] == player) {
				return i;
			}
		}
		return -1;
    }

    [Command]
    public void CmdReportKill(int victimID, int killerID) {
		Player killer = null;
		int victimLoc = -1;
		for (int i = 0; i < playerList.Length; i++) {
			if (playerList [i].ID == killerID) {
				killer = playerList [i];
			}
			if (playerList [i].ID == victimID) {
				victimLoc = i;
			}
		}
		if (killer == null || victimLoc == -1) {
			return;
		}
        killer.AddGold((int)CalculateBounty(playerList[victimLoc]));
        killer.kills++;
        if (killer.streak < 0) {
            killer.streak = 1;
        } else {
            killer.streak++;
        }
        broadcastText = UI.CreateText("Broadcast", "Player " + (killerID + 1) + " has slain Player " + (victimID + 1),
            font, Color.black, 72, canvas.transform, Vector3.zero, new Vector3(0.1f, 0.5f), new Vector3(0.9f, 0.7f), TextAnchor.MiddleCenter, true);
        Destroy(broadcastText, 5f);
    }

    public static float CalculateWorth(Player p) {
        if (kingOfTheHill) {
            return p.score;
        } else {
            return CalculateBounty(p);
        }
    }

    public static float CalculateBounty(Player p) {
        return BASE_BOUNTY + GetShipValue(p) + CalculateStreakValue(p) + CalculateKDRValue(p);
    }
    static int GetShipValue(Player p) {
        // for now, returns 1/10th of the player's gold investment into their ship
        return p.lowUpgrades + p.midUpgrades * 5 + p.highUpgrades * 20;
    }
    static int CalculateKDRValue(Player p) {
        if (p.deaths != 0) {
            float kdr = p.kills / (float)p.deaths;
            if (kdr < 1) {
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
        if (p.streak >= 5) {
            return 100;
        } else if (p.streak >= 3) {
            return 50;
        } else {
            return 10 * p.streak;
        }
    }
    public GameObject GetLeader() {
        if (kingOfTheHill) {
            return currHill;
        } else {
            if (playerList.Length > 0) {
                Player p = playerList[0];
                float bounty = CalculateWorth(p);
                for (int i = 1; i < playerList.Length; i++) {
                    float newWorth = CalculateWorth(playerList[i]);
                    if (CalculateWorth(playerList[i]) > bounty) {
                        bounty = newWorth;
                        p = playerList[i];
                    }
                }
                return p.gameObject;
            }
        }
        return null;
    }

	public void SetHill (GameObject theHill) {
		currHill = theHill;
	}


    public void UpgradeMenuButton() {
        upgradePanel.gameObject.SetActive(!upgradePanel.gameObject.activeSelf);
    }


    private IEnumerator DeclareVictory(int playerID) {
        // declare the winning player to be the pirate king
        //print("Victory has been declared!");
        if (isServer)
        {
            RpcStopCaptureSFX();
            for (int i = 0; i < playerList.Length; i++)
            {
                playerList[i].dead = true;
            }
        }

        //if (playerList[playerID].isLocalPlayer)
        SoundManager.Instance.PlaySFX_Victory();
        //else SoundManager.Instance.PlaySFX_Defeat();

        GameObject lastText = (UI.CreateText("Victory Text", "Player " + (playerID + 1) + " is the Pirate King!", font, Color.black, 100, canvas.transform,
            Vector3.zero, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.9f), TextAnchor.MiddleCenter, true));
        
        //lastText.GetComponent<Text> ().resizeTextForBestFit = true;

        yield return new WaitForSeconds(5.0f);
        Debug.Log("Reset");
        GameObject.Find("LobbyManager").GetComponent<LobbyManager>().ResetGame();
    }

    [ClientRpc]
    void RpcStopCaptureSFX()
    {
        SoundManager.Instance.StopCaptureSFX();
    }

    /*private IEnumerator MoveHill(int rangeBegin, int rangeEnd)
    {
        if (currHill == null)
        {
            CmdSpawnHill();
            yield return new WaitForSeconds(Random.Range(rangeBegin,rangeEnd));
        }
        CmdMoveHill();
        yield return new WaitForSeconds(Random.Range(rangeBegin, rangeEnd));
        StartCoroutine(MoveHill(rangeBegin, rangeEnd));
    }

	private IEnumerator HideHill(int hillTime)
	{
		currHill.SetActive (false);
		hillRep.SetActive (false);
		hillTimerText.SetActive (true);
		Text timerText = hillTimerText.GetComponent<Text> ();
		for (int i = hillTime; i > 0; i--) {
			timerText.text = "New Bounty Fountain spawns in " + i + " seconds";
			yield return new WaitForSeconds (1.0f);
		}
		hillTimerText.SetActive (false);
		currHill.SetActive (true);
		hillRep.SetActive (true);
	}*/
}
