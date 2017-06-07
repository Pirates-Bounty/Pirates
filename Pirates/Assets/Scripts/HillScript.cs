using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HillScript : NetworkBehaviour {

	public const float SCORE_INCREMENT = 1.0f;
	public const float TIME_BETWEEN_SPAWNS = 2f;
    public int totalPlayersInHill = 0;
    public int hillCheck = 2;
	public int hillSize = 4;
    public int timeToCapture = 3;
    public Color nullColor = Color.grey;
    private Player hillController = null;

	public float scoreReserve;
	public Dictionary<Player,Capture> targets;
    private bool foundAllPlayers = false;
	public bool hiding; 

	private Collider2D myCollider;
	private SpriteRenderer mySprite;

	private GameObject MapGen;
	private GameObject hillRep;
	private RectTransform hillRepRect;
	private RectTransform minimapRect;
	private GameObject hillTimerTextDisplay;
	private Text hillTimerText;


    private List<Player> keys;

	//private GameObject bountyManager;

	[SyncVar]
	public float hideTimer = 0f;


    public struct Capture
    {
        public float time;
        public bool capturing;
        public Capture(float t,bool cap)
        {
            time = t;
            capturing = cap;
        }
    }

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().color = nullColor;
        keys = new List<Player>();
		transform.localScale *= hillSize;
		hiding = true;

		targets = new Dictionary<Player,Capture>();
		scoreReserve = Random.Range (10f, 15f);
		myCollider = GetComponent<Collider2D> ();
		mySprite = GetComponent<SpriteRenderer> ();
		MapGen = GameObject.FindGameObjectWithTag("mapGen");
		hillRep = GameObject.Find("MainCanvas/UI/Minimap/Hill");
		hillRepRect = hillRep.GetComponent<RectTransform>();
		minimapRect = GameObject.Find("MainCanvas/UI/Minimap").GetComponent<RectTransform>();
		hillTimerTextDisplay = GameObject.Find("MainCanvas/UI/HillTimerText");
		hillTimerText = hillTimerTextDisplay.GetComponent<Text> ();
		//bountyManager = GameObject.Find ("BountyManager");

		if (isServer) {
			RpcMoveHill ();
		} else {
			BountyManager.Instance.SetHill(gameObject);
			HideHill ();
		}
	}
	
	// Update is called once per frame
	void Update () {

        totalPlayersInHill = 0;
        foreach(Player p in keys)
        {
            if (hillController == p && p.dead)
            {
                totalPlayersInHill = 0;
                if (!BountyManager.gameOver)
                {
                    ClearTargetsValues();
                }

                break;
            }
            if (targets[p].capturing)
            {
                totalPlayersInHill++;
            }

        }

		if (hiding) {
			if (hideTimer > 0) {
				hideTimer -= Time.deltaTime;
				hillTimerText.text = "New Bounty Fountain spawns in " + Mathf.CeilToInt (hideTimer) + " seconds";
			} else {
				RevealHill ();
            }
		} else {
			if (isServer) {

                if(hillController != null)
                {
                    //Debug.Log(hillController.playerName);
                    hillController.score += SCORE_INCREMENT * Time.deltaTime / targets.Count;
                    scoreReserve -= SCORE_INCREMENT * Time.deltaTime / targets.Count;
                }
				if (scoreReserve <= 0f) {
                    RpcStopCaptureSFX();
					hillController.StopPointSFX ();
                    scoreReserve = Random.Range (10f, 15f);

                    ClearTargetsValues();
                    hillController = null;
					totalPlayersInHill = 0;
					//BountyManager.Instance.CmdMoveHill ();
					RpcMoveHill ();
                }

                
                foreach (Player p in keys)
                {
                    if (p != hillController && targets[p].time > 0 && !(p.inHill))
                    {
                        targets[p] = new Capture(targets[p].time - Time.deltaTime,false);
                    }
                }
            }
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (isServer && col.gameObject.CompareTag ("Player")) {
            if (!foundAllPlayers)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                if (targets.Count != players.Length)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        Player p = players[i].GetComponent<Player>();
                        targets[p] = new Capture(0,false);
                    }
                    keys = new List<Player>(targets.Keys);
                }
                else
                {
                    foundAllPlayers = true;
                }
            }
		}
	}


    void OnTriggerStay2D(Collider2D other)
    {
		if (isServer && other.gameObject.CompareTag("Player"))
        {
			if (totalPlayersInHill < 2) {
				Player p = other.gameObject.GetComponent<Player> ();
				if (hillController != p && !p.dead) {
					p.inHill = true;
					targets[p] = new Capture(targets[p].time + Time.deltaTime,true);
					if (targets[p].time >= timeToCapture) {
						targets[p] = new Capture(timeToCapture,true);
						if(hillController != null)
							hillController.StopPointSFX ();
						hillController = p;
                        //GetComponent<SpriteRenderer> ().color = p.playerColor;
                        if (!BountyManager.gameOver)
                        {
                            RpcColorChange(p.playerColor);
                            ClearTargetsValuesCapture();
							if(p.isLocalPlayer)
								p.PlayPointSFX ();
                        }

                        
					}
				}
                else if(hillController == p && !p.dead)
                {
                    targets[p] = new Capture(0, true);
                }
                else
                {
                    targets[p] = new Capture(0, false);
                }
			}
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
		if (isServer && other.gameObject.CompareTag("Player"))
        {
            Player p = other.gameObject.GetComponent<Player>();
            targets[p] = new Capture(targets[p].time, false);
            p.inHill = false;
        }
    }

	void HideHill () {
		hiding = true;
		myCollider.enabled = false;
		mySprite.enabled = false;
		hillRep.SetActive (false);
		hillTimerTextDisplay.SetActive (true);
		if (isServer) {
			transform.position = MapGen.GetComponent<MapGenerator>().GetRandLocAwayFromLand(hillCheck);
		}
		hideTimer = TIME_BETWEEN_SPAWNS;
	}


	void RevealHill () {
		hillRepRect.anchoredPosition = new Vector3(minimapRect.rect.width* transform.position.x, minimapRect.rect.height * transform.position.y, 1) / (MapGen.GetComponent<MapGenerator>().width * MapGen.GetComponent<MapGenerator>().tileSize);
		hiding = false;
		myCollider.enabled = true;
		mySprite.enabled = true;
		hillRep.SetActive (true);
		hillTimerTextDisplay.SetActive (false);
        SoundManager.Instance.PlaySFX_HillSpawn();
    }


    void ClearTargetsValuesCapture()
    {
        foreach (Player p in keys)
        {
            targets[p] = new Capture(0,targets[p].capturing);
        }
    }
    void ClearTargetsValues()
    {
        foreach (Player p in keys)
        {
            targets[p] = new Capture(0, false);
        }
        RpcColorChange(nullColor);
    }

    [ClientRpc]
	void RpcMoveHill () {
		//hideTimer = TIME_BETWEEN_SPAWNS;
		HideHill ();
	}

    [ClientRpc]
    void RpcStopCaptureSFX()
    {
        SoundManager.Instance.StopCaptureSFX();
    }

    [ClientRpc]
    void RpcColorChange(Color pColor)
    {
        GetComponent<SpriteRenderer>().color = pColor;
    }
}
