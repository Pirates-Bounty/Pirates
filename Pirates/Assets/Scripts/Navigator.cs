using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour {
    // singleton
    public static Navigator Instance;
    private Sprite menuBackground;
    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private GameObject inGameMenu;

    private bool menuActive = false;

    void Awake() {
        if (!Instance) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            menuBackground = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Menu Background.jpg", typeof(Sprite));
            font = (Font)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Fonts/Angel Tears.otf", typeof(Font));
            sprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Button.png", typeof(Sprite));
            highlightedSprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/HighlightedButton.png", typeof(Sprite));
            createInGameMenu();
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Main"))) {
            if (!inGameMenu) {
                createInGameMenu();
            }
            menuActive = !menuActive;
            inGameMenu.SetActive(menuActive);
        }
    }

    public void LoadLevel(string action) {
        // Loads the scene based on the given string, or exits the game if the string is Quit
        if (action == "Quit") {
            Application.Quit();
        } else {
            SceneManager.LoadScene(action);
        }
    }

    private void createInGameMenu() {
        canvas = GameObject.Find("Canvas").transform;
        inGameMenu = UI.CreatePanel("In-Game Menu", menuBackground, Color.white, canvas,
            Vector3.zero, new Vector2(0.25f, 0.25f), new Vector3(0.75f, 0.75f));
        GameObject mainMenuButton = UI.CreateButton("Main Menu Button", "Main Menu", font, Color.black, 24, inGameMenu.transform,
            sprite, highlightedSprite, Vector3.zero, new Vector2(0.25f, 0.7f), new Vector2(0.75f, 0.9f),
            delegate { UI.CreateYesNoDialog("Confirmation", "Are you sure?",
                font, Color.black, 24, menuBackground, sprite, highlightedSprite,
                Color.white, inGameMenu.transform, Vector3.zero, new Vector2(0.25f, 0.1f),
                new Vector2(0.75f, 0.9f), delegate { LoadLevel("Menu"); }); });
        GameObject optionButton = UI.CreateButton("Options Button", "Options", font, Color.black, 24, inGameMenu.transform,
            sprite, highlightedSprite, Vector3.zero, new Vector2(0.25f, 0.4f), new Vector2(0.75f, 0.6f), delegate { ; });
        GameObject returnButton = UI.CreateButton("Return to Game Button", "Return to Game", font, Color.black, 24, inGameMenu.transform,
            sprite, highlightedSprite, Vector3.zero, new Vector2(0.25f, 0.1f), new Vector2(0.75f, 0.3f),
            delegate { menuActive = !menuActive; inGameMenu.SetActive(menuActive); });
        inGameMenu.SetActive(menuActive);
    }
}
