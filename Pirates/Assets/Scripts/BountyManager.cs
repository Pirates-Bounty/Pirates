using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BountyManager : NetworkBehaviour {

	public const int BASE_BOUNTY = 100;

	public SyncListInt playerBounties = new SyncListInt();
	public SyncListInt killStreak = new SyncListInt();

	private GameObject bountyPanel;
	private List<GameObject> bountyTexts = new List<GameObject>();
	private Canvas canvas;
	private Font font;

	// Use this for initialization
	void Start () {
		font = Resources.Load<Font>("Art/Fonts/riesling");

		if (!isLocalPlayer) {
			return;
		}
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		if (canvas != null) {
			CreateBountyPanel ();
			//print ("makin' a bounty board");
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

				if (playerBounties [playerList [i].playerID] >= 1000) {
					DeclareVictory (playerList [i].playerID);
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


	private IEnumerator DeclareVictory(int playerID) {
		// delcare the winning player to be the pirate king
	}
}
