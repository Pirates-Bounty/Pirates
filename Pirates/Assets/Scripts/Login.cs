using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

	private GameObject playButton;
    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
	private GameObject skipButton;
    private Color color = Color.black;
    private int fontSize = 20;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find("Canvas").transform;

		playButton = UI.CreateButton("Skip", "Skip", font, color, fontSize, canvas, 
		Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/PlayButtonUnClicked"), Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/PlayButtonClicked"),
        Vector3.zero, new Vector2(0.05f, 0.05f), new Vector2(0.325f, 0.2f),
        delegate {
            Application.LoadLevel("Menu");
        } );
	}

	// Update is called once per frame
	void Update () {
	
	}
}
