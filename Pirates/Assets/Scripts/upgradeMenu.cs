using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class upgradeMenu : MonoBehaviour {

	public bool playerOne;
	private Player myPlayer;
	public GameObject[] buttons;
	public GameObject myCanvas;
	public int selected;
	public bool active;

	// Use this for initialization
	void Start () {
		active = false;
		selected = 0;
		//buttons [selected].GetComponent<Button> ().Select ();
		//myCanvas = transform.GetChild (0).gameObject;
		myPlayer = GetComponent<Player> ();
		playerOne = myPlayer.playerOne;
	}
	
	// Update is called once per frame
	void Update () {
		if (!active || myCanvas == null) {
			return;
		}
		buttons [selected].GetComponent<Button> ().Select ();

		if ((playerOne && Input.GetKeyDown (KeyCode.W)) || (!playerOne && Input.GetKeyDown (KeyCode.UpArrow))) {
			selected -= 2;
			if (selected < 0) {
				selected += 6;
			}
			//buttons [selected].GetComponent<Button> ().Select ();
		}
		if ((playerOne && Input.GetKeyDown (KeyCode.A)) || (!playerOne && Input.GetKeyDown (KeyCode.LeftArrow))) {
			selected -= 1;
			if (selected % 2 != 0) {
				selected += 2;
			}
			//buttons [selected].GetComponent<Button> ().Select ();
		}
		if ((playerOne && Input.GetKeyDown (KeyCode.S)) || (!playerOne && Input.GetKeyDown (KeyCode.DownArrow))) {
			selected += 2;
			if (selected > 5) {
				selected -= 6;
			}
			//buttons [selected].GetComponent<Button> ().Select ();
		}
		if ((playerOne && Input.GetKeyDown (KeyCode.D)) || (!playerOne && Input.GetKeyDown (KeyCode.RightArrow))) {
			selected += 1;
			if (selected % 2 == 0) {
				selected -= 2;
			}
			//buttons [selected].GetComponent<Button> ().Select ();
		}
	}

	void LateUpdate () {
		if (!active || myCanvas == null) {
			return;
		}
		if ((playerOne && Input.GetKeyUp (KeyCode.E)) || (!playerOne && Input.GetKeyUp (KeyCode.Return))) {
			buttons [selected].GetComponent<Button> ().onClick.Invoke ();
			StartCoroutine(SimulatePress(0.1f));
		}
	}


	public IEnumerator SimulatePress(float num)
	{
		Button thisButton = buttons [selected].GetComponent<Button> ();
		SpriteState sstate = thisButton.spriteState;
		Sprite highlight = sstate.highlightedSprite;
		sstate.highlightedSprite = sstate.pressedSprite;
		thisButton.spriteState = sstate;
		yield return new WaitForSeconds (num);
		sstate.highlightedSprite = highlight;
		thisButton.spriteState = sstate;
	}


	public void CloseMenu () {
		active = false;
		myCanvas.SetActive (false);
		myPlayer.activeMenu = false;
	}

	public void OpenMenu() {
		active = true;
		myCanvas.SetActive (true);
		selected = 0;
	}
}
