using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
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
    private bool positive = true;
    private bool forward = true;

	private AudioClip menuM;
    private AudioClip highlightAudio;
    private AudioClip selectAudio;

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
        font = Resources.Load<Font>("Art/Fonts/Angel Tears");
        sprite = Resources.Load<Sprite>("Art/Sprites/UPDATED 12-19-16/UI 11-19-16/Golden Button Unpushed");
        highlightedSprite = Resources.Load<Sprite>("Art/Sprites/UPDATED 12-19-16/UI 11-19-16/Golden Button Pushed");
        //background = Resources.Load<Sprite>("Art/Backgrounds/Pirate's Bounty");
        //Background
        //backgroundPanel = UI.CreatePanel("Background", background, Color.white, canvas, Vector3.zero, Vector2.zero, Vector2.one);
        // Play Button
        playButton = UI.CreateButton("Play", "Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.2f, 0.35f), new Vector2(0.8f, 0.5f),
            delegate {
                Navigator.Instance.LoadLevel("Lobby");
                SoundManager.Instance.PlaySFXTransition(Resources.Load<AudioClip>("Sound/SFX/UI/DoorOpen"),0.2f);
                SoundManager.Instance.PlayBGM((int)TrackID.BGM_LOBBY);
            } );

        // Instructions Button
        instructionsButton = UI.CreateButton("How To Play", "How To Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.35f), delegate { Navigator.Instance.LoadLevel("Lobby Tutorial"); });

        // Quit Button
        quitButton = UI.CreateButton("Quit", "Quit", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.2f, 0.05f), new Vector2(0.8f, 0.2f), delegate { Navigator.Instance.LoadLevel("Quit"); });

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

	}
    
}
