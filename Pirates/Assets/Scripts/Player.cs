using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public const float maxHealth = 100.0f;
    public const float projectileSpeed = 50.0f;

    public const float firingDelay = 0.3f;
    public const float rotationSpeed = 25.0f;
    public const float moveSpeed = 10.0f;
    [SyncVar(hook = "OnChangePlayer")]
    public float currentHealth = maxHealth;
    public int resources = 1000;


    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode fireLeft;
    public KeyCode fireRight;
    public KeyCode menu;
    public Transform leftSpawn;
    public Transform rightSpawn;
    public GameObject projectile;

    private float firingTimer;
    private Camera playerCamera;
    private Canvas canvas;
    private GameObject healthBar;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private GameObject inGameMenu;
    private GameObject resourcesText;
    private Sprite menuBackground;
    private Rigidbody2D rb;
    private bool menuActive = false;
    private bool anchorDown = false;


    // Use this for initialization
    void Start () {
		firingTimer = firingDelay;
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        menuBackground = Resources.Load<Sprite>("Art/Textures/Menu Background");
        font = Resources.Load<Font>("Art/Fonts/riesling");
        sprite = Resources.Load<Sprite>("Art/Textures/Button");
        highlightedSprite = Resources.Load<Sprite>("Art/Textures/HighlightedButton");
        if (!isLocalPlayer) {
            return;
        }
        RenderInterface();
        CreateInGameMenu();
    }

	void Update () {
        if (!isLocalPlayer) {
            return;
        }
        // update the camera's position
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        // get player's movement
        GetMovement();

        firingTimer -= Time.deltaTime;
        if (firingTimer < 0) {
            // fire cannons
            if (Input.GetKeyDown(fireLeft)) {
                CmdFireLeft();
                firingTimer = firingDelay;
            }
            if (Input.GetKeyDown(fireRight)) {
                CmdFireRight();
                firingTimer = firingDelay;
            }
        }

        UpdateInterface();
    }
    [Command]   
	void CmdFireLeft () {
		GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, leftSpawn.position, Quaternion.identity);
        instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = leftSpawn.up * projectileSpeed;
        NetworkServer.Spawn(instantiatedProjectile);
    }
    [Command]
    void CmdFireRight() {
        GameObject instantiatedProjectile = (GameObject)Instantiate(projectile, rightSpawn.position, Quaternion.identity);
        instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = rightSpawn.up * projectileSpeed;
        NetworkServer.Spawn(instantiatedProjectile);
    }

    private void UpdateInterface() {
        if (Input.GetKeyDown(menu)) {
            if (!inGameMenu) {
                CreateInGameMenu();
            }
            menuActive = !menuActive;
            inGameMenu.SetActive(menuActive);
        }
        resourcesText.GetComponent<Text>().text = "Resources " + resources;
    }
    void OnChangePlayer(float health) {
        if (!isLocalPlayer) {
            return;
        }
        healthBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f - 0.3f * (maxHealth - health) / 100.0f, 0.95f);
    }

    private void GetMovement() {
        if (Input.GetKey(left)) {
            transform.Rotate(new Vector3(0.0f, 0.0f, rotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(right)) {
            transform.Rotate(new Vector3(0.0f, 0.0f, -rotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(up)) {
            transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f);
            //rb.AddForce(transform.up * moveSpeed);
        }
        if (Input.GetKey(down)) {
            //rb.AddForce(-transform.up * moveSpeed / 4);
        }

    }
    public void ApplyDamage(float damage) {
        if (!isServer) {
            return;
        }
        currentHealth -= damage;
        // destroy the player if they are dead
        if (currentHealth <= 0.0f) {
            Destroy(gameObject);
        }
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

    public override void OnStartLocalPlayer() {
        //GetComponent<SpriteRenderer>().color = Color.red;
    }
}
