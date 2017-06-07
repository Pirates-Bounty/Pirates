using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {
    private Transform canvas;
    private Font font;
    //private Sprite background;
    private Color color = Color.black;

    private GameObject playButton;
    private GameObject instructionsButton;
    private GameObject quitButton;
    private GameObject boat;
    private GameObject leftArm;
    private GameObject rightArm;
    private GameObject waves1;
    private GameObject waves2;
    private GameObject waves3;
    private GameObject waves4;
    
    private float maxRotation = 2f;
    private float currentRotation = 0.0f;
    private int fontSize = 20;
    private bool positive = true;
    private bool forward = true;
    

	private AudioClip menuM;
    private AudioClip highlightAudio;
    private AudioClip selectAudio;

	public Image tutorial;
	public Text tutorialText;
	private int slideIndex;
	private string[][] slide;

    // Use this for initialization
    void Start() {
        canvas = GameObject.Find("Canvas").transform;
        boat = GameObject.Find("Boat");
        waves1 = GameObject.Find("Waves 1");
        waves2 = GameObject.Find("Waves 2");
        waves3 = GameObject.Find("Waves 3");
        waves4 = GameObject.Find("Waves 4");
        leftArm = GameObject.Find("leftarm");
        rightArm = GameObject.Find("rightarm");
        font = Resources.Load<Font>("Art/Fonts/SHOWG");
        //background = Resources.Load<Sprite>("Art/Backgrounds/Pirate's Bounty");
        //Background
        //backgroundPanel = UI.CreatePanel("Background", background, Color.white, canvas, Vector3.zero, Vector2.zero, Vector2.one);
        // Play Button
        playButton = UI.CreateButton("Play", "", font, color, fontSize, canvas,
            Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/PlayButtonUnClicked"), Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/PlayButtonClicked"),
            Vector3.zero, new Vector2(0.05f, 0.05f), new Vector2(0.325f, 0.2f),
            delegate {
                Navigator.Instance.LoadLevel("Lobby");
                SoundManager.Instance.PlaySFXTransition(Resources.Load<AudioClip>("Sound/SFX/UI/DoorOpen"),0.2f);
                SoundManager.Instance.PlayBGM((int)TrackID.BGM_LOBBY);
            } );
		playButton.layer = 5;

        // Instructions Button
        instructionsButton = UI.CreateButton("How To Play", "", font, color, fontSize, canvas,
            Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/HowToPlayButtonUnClicked"), Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/HowToPlayButtonClicked"),
			Vector3.zero, new Vector2(0.375f, 0.05f), new Vector2(0.625f, 0.2f), delegate { StartTutorial(); }); //Navigator.Instance.LoadLevel("Lobby"); Navigator.Tutorial = true; });
		instructionsButton.layer = 5;

        // Quit Button
        quitButton = UI.CreateButton("Quit", "", font, color, fontSize, canvas,
            Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/QuitButtonUnClicked"), Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/QuitButtonClicked"),
            Vector3.zero, new Vector2(0.675f, 0.05f), new Vector2(0.95f, 0.2f), delegate { Navigator.Instance.LoadLevel("Quit"); });
		quitButton.layer = 5;

        //=== SOUND SECTION - BEGIN ===
        //preparing highlight entries
        UnityEngine.EventSystems.EventTrigger.Entry entry_highlight = new UnityEngine.EventSystems.EventTrigger.Entry(); //entry object creation
        entry_highlight.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter; //setting the trigger type; how is it triggered
        entry_highlight.callback.AddListener((data) => SoundManager.Instance.PlaySFX(GameObject.Find("SoundManager").GetComponent<SoundManager>().highlightAudio, 0.3f)); //call function=> playAudio(...)
        //adding highlight entries
        playButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);
        instructionsButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);
        quitButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);

        SoundManager.Instance.PlayBGM((int)TrackID.BGM_MENU);
        //adding onclick audio
        //selectAudio = Resources.Load<AudioClip>("Sound/SFX/ButtonSelect");
        //playButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => playAudio(selectAudio));
        //instructionsButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => playAudio(selectAudio));
        //quitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => playAudio(selectAudio));
        //=== SOUND SECTION - END ===
    }
	
	// Update is called once per frame
	void Update () {
        // make the background sway back and forth
        if(currentRotation < maxRotation && positive) {
            currentRotation += 2f*Time.deltaTime;
        } else if(currentRotation > -maxRotation && !positive){
            currentRotation -= 2f*Time.deltaTime;
        } else {
            positive = !positive;
        }
        boat.transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentRotation);
        leftArm.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -2f * currentRotation);
        rightArm.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -2f * currentRotation);
        if (boat.transform.position.x > 5 || boat.transform.position.x < -5) {
            forward = !forward;
        }
        if (forward) {
            boat.transform.Translate(new Vector3(1f * Time.deltaTime, 0f, 0f));
        } else {
            boat.transform.Translate(new Vector3(-1f * Time.deltaTime, 0f, 0f));
        }
        if (positive) {
            waves1.transform.Translate(new Vector3(0.5f * Time.deltaTime, 0.1f * Time.deltaTime, 0f));
            waves2.transform.Translate(new Vector3(-0.5f * Time.deltaTime, -0.1f * Time.deltaTime, 0f));
            waves3.transform.Translate(new Vector3(0.5f * Time.deltaTime, 0.1f * Time.deltaTime, 0f));
            waves4.transform.Translate(new Vector3(-0.5f * Time.deltaTime, -0.1f * Time.deltaTime, 0f));
        } else {
            waves1.transform.Translate(new Vector3(-0.5f * Time.deltaTime, -0.1f * Time.deltaTime, 0f));
            waves2.transform.Translate(new Vector3(0.5f * Time.deltaTime, 0.1f * Time.deltaTime, 0f));
            waves3.transform.Translate(new Vector3(-0.5f * Time.deltaTime, -0.1f * Time.deltaTime, 0f));
            waves4.transform.Translate(new Vector3(0.5f * Time.deltaTime, 0.1f * Time.deltaTime, 0f));
        }

		if (tutorial.IsActive ())
			TutorialProcess ();
	}

	void StartTutorial () {
		Debug.Log ("Welcome to the tutorial, doofus!~~You are trapped with me forever! Mwahaha!");
		slide = new string[13][]{
			new string[2]{ "1 - Welcome", "Yarr.. Welcome to the tutorial!\nUse Left Click to continue"},
			new string[2]{ "2 - Forward", "Press the 'W' key to move forward and the 'S' key to move back!"},
			new string[2]{ "3 - Turn"   , "Turn left or right using the 'A' and 'D' keys!\nPress 'L.Shift' to get a quick boost of speed!"},
			new string[2]{ "4 - Fire"   , "Time to learn combat! Left Click to fire from your Left Cannons and Right Click to fire from your Right Cannons!"},
			new string[2]{ "5 - Gold"   , "Throughout your journeys, you'll find gold!\nMove your ship over the gold to collect it."},
			new string[2]{ "6 - Collect", "Your collected gold shows up on the bottom of your screen!\nDefeating other players also rewards you with gold."},
			new string[2]{ "7 - Upgrade", "Click 'X' to open your upgrade menu and use your hard earned gold to upgrade your vessel!"},
			new string[2]{ "8 - Wow"    , "Continually upgrade and your ship will become stronger and stronger!"},
			new string[2]{ "9 - Compass", "On the bottom of the screen you also have a compass!"},
			new string[2]{"10 - Hill"   , "Follow the compass to reach the hill!\nThe hill also shows up on your minimap."},
			new string[2]{"11 - Points" , "Hold 'Tab' to view the current distribution of points.\nMake sure you're in the lead!"},
			new string[2]{"12 - Capture", "Set your vessel upon the hill to capture it so you can acquire points!"},
			new string[2]{"13 - Win"    , "In the end, the best pirate will win, so go out there and kick some pirates' bounty!"}};
		tutorial.transform.SetAsLastSibling();
		tutorial.gameObject.SetActive(true);
		slideIndex = 0;
		tutorial.GetComponent<Image> ().sprite = Resources.Load<Sprite>("Art/Screenshots/" + slide [slideIndex] [0]);
		tutorialText.GetComponent<Text> ().text = slide [slideIndex] [1];
	}

	void TutorialProcess () {
		if(Input.GetKeyDown (KeyCode.Mouse0)) {
			++slideIndex;
			if (slideIndex >= 13) {
				tutorial.gameObject.SetActive (false);
			} else {
				tutorial.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Art/Screenshots/" + slide [slideIndex] [0]);
				tutorialText.GetComponent<Text> ().text = slide [slideIndex] [1];
			}
		}
	}
    
}
