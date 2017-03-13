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

	public float scoreReserve;
	public List<Player> targets;
	public bool hiding;

	private Collider2D myCollider;
	private SpriteRenderer mySprite;

	private GameObject MapGen;
	private GameObject hillRep;
	private RectTransform hillRepRect;
	private RectTransform minimapRect;
	private GameObject hillTimerTextDisplay;
	private Text hillTimerText;

	//private GameObject bountyManager;

	[SyncVar]
	public float hideTimer = 0f;

	// Use this for initialization
	void Start () {
		transform.localScale *= hillSize;
		hiding = true;

		targets = new List<Player>();
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
			HideHill ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (hiding) {
			if (hideTimer > 0) {
				hideTimer -= Time.deltaTime;
				hillTimerText.text = "New Bounty Fountain spawns in " + Mathf.CeilToInt (hideTimer) + " seconds";
			} else {
				RevealHill ();
            }
		} else {
			if (isServer) {
				for (int i = 0; i < targets.Count; i++) {
					targets [i].score += SCORE_INCREMENT * Time.deltaTime / targets.Count;
					scoreReserve -= SCORE_INCREMENT * Time.deltaTime / targets.Count;
				}
				if (scoreReserve <= 0f) {
                    RpcStopCaptureSFX();
                    scoreReserve = Random.Range (10f, 15f);
					targets.Clear ();
                    
					//BountyManager.Instance.CmdMoveHill ();
					RpcMoveHill ();
                }
			}
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.CompareTag ("Player")) {
			targets.Add(col.gameObject.GetComponent<Player>());
		}
	}

	void OnTriggerExit2D (Collider2D col) {
		if (col.gameObject.CompareTag ("Player")) {
			targets.Remove(col.gameObject.GetComponent<Player>());
		}
	}

	void HideHill () {
		hiding = true;
		myCollider.enabled = false;
		mySprite.enabled = false;
		hillRep.SetActive (false);
		hillTimerTextDisplay.SetActive (true);
		if (isServer) {
			transform.position = MapGen.GetComponent<MapGenerator>().GetRandHillLocation(hillCheck);
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

	[ClientRpc]
	void RpcMoveHill () {
		hideTimer = TIME_BETWEEN_SPAWNS;
		HideHill ();
	}

    [ClientRpc]
    void RpcStopCaptureSFX()
    {
        SoundManager.Instance.StopCaptureSFX();
    }
}
