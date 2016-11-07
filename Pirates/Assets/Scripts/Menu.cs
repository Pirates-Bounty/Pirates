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
        playButton = UI.CreateButton("How To Play", "How To Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.2f), new Vector2(0.9f, 0.3f), delegate { Navigator.Instance.LoadLevel("Instructions"); });

        // Quit Button
        playButton = UI.CreateButton("Quit", "Quit", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.05f), new Vector2(0.9f, 0.15f), delegate { Navigator.Instance.LoadLevel("Quit"); });
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
}
