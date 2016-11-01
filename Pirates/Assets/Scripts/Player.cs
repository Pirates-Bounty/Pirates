using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public Rigidbody2D rb;
    public float maxHealth = 100.0f;
    public int resources = 1000;
    private float currentHealth;
	public float rotationSpeed; //150.0f
	public float moveSpeed;		//	5.0f
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode fire;
    public KeyCode menu;
    public Transform projectileSpawn;

    public float projectileSpeed = 100.0f;
	public GameObject projectile;
	public float firingDelay = 0.3f;
	private float firingTimer;
    //private Camera playerCamera;
    private Canvas canvas;
    private GameObject healthBar;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private GameObject inGameMenu;
    private GameObject resourcesText;
    private Sprite menuBackground;
    private bool menuActive = false;

    // Use this for initialization
    void Start () {
		firingTimer = firingDelay;
        //playerCamera = GetComponentInChildren<Camera>();
        canvas = GetComponentInChildren<Canvas>();
        currentHealth = maxHealth;
        menuBackground = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Menu Background.jpg", typeof(Sprite));
        font = (Font)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Fonts/riesling.ttf", typeof(Font));
        sprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/Button.png", typeof(Sprite));
        highlightedSprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/HighlightedButton.png", typeof(Sprite));
        RenderInterface();
        CreateInGameMenu();
    }

	void Update () {
        // update the camera's position
        //playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        // get player's movement
        GetMovement();
        firingTimer -= Time.deltaTime;
        if (firingTimer < 0) {
            // fire cannons
            if (Input.GetKeyDown(fire))
            {
                FireCannons();
                firingTimer = firingDelay;
            }
        }
        // destroy the player if they are dead
        if(currentHealth <= 0.0f) {
            Destroy(gameObject);
        }
        healthBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f - 0.3f*(maxHealth - currentHealth)/100.0f, 0.95f);
        if (Input.GetKeyDown(menu) && SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Main"))) {
            if (!inGameMenu) {
                CreateInGameMenu();
            }
            menuActive = !menuActive;
            inGameMenu.SetActive(menuActive);
        }
        resourcesText.GetComponent<Text>().text = "Resources " + resources;
    }

	void FireCannons () {
		GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, projectileSpawn.position, Quaternion.identity);
        instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(new Vector2(0, projectileSpeed));
    }

    private void GetMovement() {
        if (Input.GetKey(up)) {
            rb.AddForce(transform.up * moveSpeed);
        }
        if (Input.GetKey(down)) {
            rb.AddForce(-transform.up * moveSpeed / 4);
        }
        if (Input.GetKey(left)) {
            transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
            rb.velocity = rb.velocity.magnitude * transform.up;
        }
        if (Input.GetKey(right)) {
            transform.Rotate(0, 0, -Time.deltaTime * rotationSpeed);
            rb.velocity = rb.velocity.magnitude * transform.up;
        }
    }
    public void ApplyDamage(float damage) {
        currentHealth -= damage;
    }

    private void RenderInterface() {
        UI.CreatePanel("Profile", sprite, Color.white, canvas.transform, Vector3.zero, new Vector2(0.05f, 0.8f), new Vector2(0.2f, 0.95f));
        healthBar = UI.CreatePanel("Health Bar", null, Color.green, canvas.transform, Vector3.zero, new Vector2(0.2f, 0.9f), new Vector2(0.5f, 0.95f));
        GameObject bar = UI.CreatePanel("Bar", null, new Color(0.8f, 0.8f, 0.1f), canvas.transform, Vector3.zero, new Vector2(0.2f, 0.8f), new Vector2(0.5f, 0.9f));
        resourcesText = UI.CreateText("Resources Text", "Resources ", font, Color.black, 20, bar.transform, Vector3.zero, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, true);
    }

    private void CreateInGameMenu() {
        inGameMenu = UI.CreatePanel("In-Game Menu", menuBackground, Color.white, canvas.transform,
            Vector3.zero, new Vector2(0.25f, 0.25f), new Vector3(0.75f, 0.75f));
        GameObject mainMenuButton = UI.CreateButton("Main Menu Button", "Main Menu", font, Color.black, 24, inGameMenu.transform,
            sprite, highlightedSprite, Vector3.zero, new Vector2(0.25f, 0.7f), new Vector2(0.75f, 0.9f),
            delegate {
                UI.CreateYesNoDialog("Confirmation", "Are you sure?",
         font, Color.black, 24, menuBackground, sprite, highlightedSprite,
         Color.white, inGameMenu.transform, Vector3.zero, new Vector2(0.25f, 0.1f),
         new Vector2(0.75f, 0.9f), delegate { Navigator.Instance.LoadLevel("Menu"); });
            });
        GameObject optionButton = UI.CreateButton("Options Button", "Options", font, Color.black, 24, inGameMenu.transform,
            sprite, highlightedSprite, Vector3.zero, new Vector2(0.25f, 0.4f), new Vector2(0.75f, 0.6f), delegate {; });
        GameObject returnButton = UI.CreateButton("Return to Game Button", "Return to Game", font, Color.black, 24, inGameMenu.transform,
            sprite, highlightedSprite, Vector3.zero, new Vector2(0.25f, 0.1f), new Vector2(0.75f, 0.3f),
            delegate { menuActive = !menuActive; inGameMenu.SetActive(menuActive); });
        inGameMenu.SetActive(menuActive);
    }
}
