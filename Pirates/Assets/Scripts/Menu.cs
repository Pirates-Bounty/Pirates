using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private Color color = Color.black;

    private GameObject playButton;
    private GameObject instructionsButton;
    private GameObject quitButton;
    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas").transform;
        font = (Font)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Fonts/Angel Tears.otf", typeof(Font));
        sprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Button.png", typeof(Sprite));
        highlightedSprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/HighlightedButton.png", typeof(Sprite));

        // Play Button
        playButton = UI.CreateButton("Play", "Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.35f), new Vector2(0.9f, 0.45f), delegate { Navigator.Instance.LoadLevel("Main"); });

        // Instructions Button
        playButton = UI.CreateButton("How To Play", "How To Play", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.2f), new Vector2(0.9f, 0.3f), delegate { Navigator.Instance.LoadLevel("Instructions"); });

        // Quit Button
        playButton = UI.CreateButton("Quit", "Quit", font, color, 64, canvas, sprite, highlightedSprite,
            Vector3.zero, new Vector2(0.6f, 0.05f), new Vector2(0.9f, 0.15f), delegate { Navigator.Instance.LoadLevel("Quit"); });
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
