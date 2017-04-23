using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
	private GameObject skipButton;
    private Color color = Color.black;
    private int fontSize = 20;

	// Use this for initialization
	void Start () {
		// playButton = UI.CreateButton("Skip", "Skip", font, color, fontSize, canvas, sprite, highlightedSprite,
  //       Vector3.zero, new Vector2(0.05f, 0.05f), new Vector2(0.325f, 0.2f),
  //       delegate {
  //           Navigator.Instance.LoadLevel("Menu");
  //       } );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
