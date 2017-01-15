using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private Sprite background;
    private Color color = Color.black;

    private GameObject playButton;
    private GameObject instructionsButton;
    private GameObject quitButton;
    private GameObject backgroundPanel;
    private float maxRotation = 0.5f;
    private float currentRotation = 0.0f;
    private bool positive = true;

	private AudioClip menuM;
    private AudioClip highlightAudio;
    private AudioClip selectAudio;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas").transform;
        font = Resources.Load<Font>("Art/Fonts/Angel Tears");
        sprite = Resources.Load<Sprite>("Art/Textures/Button");
        highlightedSprite = Resources.Load<Sprite>("Art/Textures/HighlightedButton");
        background = Resources.Load<Sprite>("Art/Backgrounds/Pirate's Bounty");
        //Background
        backgroundPanel = UI.CreatePanel("Background", background, Color.white, canvas, Vector3.zero, Vector2.zero, Vector2.one);
        // Play Button
        playButton = UI.CreateButton("Play", "Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.35f), new Vector2(0.9f, 0.45f), delegate { Navigator.Instance.LoadLevel("Lobby"); });

        // Instructions Button
        instructionsButton = UI.CreateButton("How To Play", "How To Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.2f), new Vector2(0.9f, 0.3f), delegate { Navigator.Instance.LoadLevel("Instructions"); });

        // Quit Button
        quitButton = UI.CreateButton("Quit", "Quit", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.05f), new Vector2(0.9f, 0.15f), delegate { Navigator.Instance.LoadLevel("Quit"); });

        //=== SOUND SECTION - BEGIN ===
        //variable initialization
        highlightAudio = Resources.Load<AudioClip>("Sound/SFX/ButtonHighlight");
        selectAudio = Resources.Load<AudioClip>("Sound/SFX/ButtonSelect");
        //preparing highlight entries
        UnityEngine.EventSystems.EventTrigger.Entry entry_highlight = new UnityEngine.EventSystems.EventTrigger.Entry(); //entry object creation
        entry_highlight.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter; //setting the trigger type; how is it triggered
        entry_highlight.callback.AddListener((data) => playAudio(highlightAudio)); //call function=> playAudio(...)
        //adding highlight entries
        playButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);
        instructionsButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);
        quitButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);
        //adding onclick audio
        playButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => playAudio(selectAudio));
        instructionsButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => playAudio(selectAudio));
        quitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => playAudio(selectAudio));
        //=== SOUND SECTION - END ===
    }
	
	// Update is called once per frame
	void Update () {
        // make the background sway back and forth
        if(currentRotation < maxRotation && positive) {
            currentRotation += Time.deltaTime;
        } else if(currentRotation > -maxRotation && !positive){
            currentRotation -= Time.deltaTime;
        } else {
            positive = !positive;
        }
        backgroundPanel.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, currentRotation);

	}
    
    //function to play audio
    public void playAudio(AudioClip audio)
    {
        AudioSource.PlayClipAtPoint(audio, transform.position, 100.0f);
    }
}
