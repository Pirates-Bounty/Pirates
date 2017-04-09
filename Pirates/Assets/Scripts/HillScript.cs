using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HillScript : NetworkBehaviour {

	public const float SCORE_INCREMENT = 1.0f;
	public const float TIME_BETWEEN_SPAWNS = 5f;
	public int hillCheck = 20;
	public int hillSize = 4;
    public int timeToCapture = 3;
    public Color nullColor = Color.grey;
    private Player hillController = null;

	public float scoreReserve;
	public Dictionary<Player,float> targets;
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
    private int totalPlayersInHill = 0;

    private List<Player> keys;

	//private GameObject bountyManager;

	[SyncVar]
	public float hideTimer = 0f;



	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().color = nullColor;
        keys = new List<Player>();
		transform.localScale *= hillSize;
		hiding = true;

		targets = new Dictionary<Player,float>();
		scoreReserve = Random.Range (10f, 15f);
		myCollider = GetComponent<Collider2D> ();
		mySprite = GetComponent<SpriteRenderer> ();
		MapGen = GameObject.FindGameObjectWithTag("mapGen");
		hillRep = GameObject.Find("Canvas/Minimap/Hill");
		hillRepRect = hillRep.GetComponent<RectTransform>();
		minimapRect = GameObject.Find("Canvas/Minimap").GetComponent<RectTransform>();
		hillTimerTextDisplay = GameObject.Find("Canvas/UI/HillTimerText");
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

		if (hiding) {
			if (hideTimer > 0) {
				if (isServer) {
					hideTimer -= Time.deltaTime;
				}
				hillTimerText.text = "New Bounty Fountain spawns in " + Mathf.CeilToInt (hideTimer) + " seconds";
			} else {
				RevealHill ();
            }
		} else {
			if (isServer) {

                if(hillController != null)
                {
                    hillController.score += SCORE_INCREMENT * Time.deltaTime / targets.Count;
                    scoreReserve -= SCORE_INCREMENT * Time.deltaTime / targets.Count;
                }
				if (scoreReserve <= 0f) {
                    RpcStopCaptureSFX();
                    scoreReserve = Random.Range (10f, 15f);

                    ClearTargetsValues();
                    GetComponent<SpriteRenderer>().color = nullColor;
                    hillController = null;
					totalPlayersInHill = 0;
					//BountyManager.Instance.CmdMoveHill ();
					RpcMoveHill ();
					hideTimer = TIME_BETWEEN_SPAWNS;
                }

                
                foreach (Player p in keys)
                {
                    if (p != hillController && targets[p] > 0 && !(p.inHill))
                    {
                        targets[p] = targets[p] -= Time.deltaTime;
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
                        targets[p] = 0;
                    }
                    keys = new List<Player>(targets.Keys);
                }
                else
                {
                    foundAllPlayers = true;
                }
            }
            totalPlayersInHill += 1;
		}
	}


    void OnTriggerStay2D(Collider2D other)
    {
		if (isServer && other.gameObject.CompareTag("Player"))
        {
			if (totalPlayersInHill < 2) {
				Player p = other.gameObject.GetComponent<Player> ();
				if (hillController != p) {
					p.inHill = true;
					targets [p] = targets [p] + Time.deltaTime;
					if (targets [p] >= timeToCapture) {
						targets [p] = timeToCapture;
						hillController = p;
						GetComponent<SpriteRenderer> ().color = p.playerColor;
						RpcColorChange (p.playerColor);
						ClearTargetsValues ();
					}
				}
			}
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
		if (isServer && other.gameObject.CompareTag("Player"))
        {
            totalPlayersInHill -= 1;
            Player p = other.gameObject.GetComponent<Player>();
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
	}


	void RevealHill () {
		hillRepRect.anchoredPosition = new Vector3(minimapRect.rect.width* transform.position.x, minimapRect.rect.height * transform.position.y, 1) / MapGen.GetComponent<MapGenerator>().width;
		hiding = false;
		myCollider.enabled = true;
		mySprite.enabled = true;
		hillRep.SetActive (true);
		hillTimerTextDisplay.SetActive (false);
        SoundManager.Instance.PlaySFX_HillSpawn();
    }


    void ClearTargetsValues()
    {
        foreach (Player p in keys)
        {
            targets[p] = 0;
        }
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
		GetComponent<SpriteRenderer> ().color = pColor;
	}
}
