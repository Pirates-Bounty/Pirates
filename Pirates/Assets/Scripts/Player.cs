using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
public enum Upgrade {
    MANEUVERABILITY, //rotation speed
    SPEED, // move speed
    HULL_STRENGTH, // health
    CANNON_SPEED, // cannon firing time
    CANNON_STRENGTH, // cannon damage
    COUNT //num items in the enum
}
public class Player : NetworkBehaviour {
    // const vars
    public const float BASE_MAX_HEALTH = 100.0f;
    public const float BASE_PROJECTILE_SPEED = 50.0f;
	public const float BASE_PROJECTILE_STRENGTH = 10.0f;
    public const float BASE_FIRING_DELAY = 1.0f;
    public const float BASE_ROTATION_SPEED = 25.0f;
    public const float BASE_MOVE_SPEED = 10.0f;
    public const int MAX_UPGRADES = 5;
    public const int UPGRADE_COST = 100;

    [SyncVar(hook = "OnChangePlayer")]
	public float currentHealth = BASE_MAX_HEALTH;
    [SyncVar(hook = "OnChangeResources")]
    public int resources = 1000;

    // keybinds and prefabs
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode fireLeft;
    public KeyCode fireRight;
    public KeyCode menu;
    public KeyCode upgrade;
    public Transform leftSpawn;
    public Transform rightSpawn;
    public GameObject projectile;
	public GameObject resourceObj;


    private Camera playerCamera;
    private Canvas canvas;
    private GameObject healthBar;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    // GameObject references
    private GameObject inGameMenu;
    private GameObject upgradeMenu;
    private GameObject[] upgradeTexts = new GameObject[(int)Upgrade.COUNT];
    private GameObject resourcesText;
    private Sprite menuBackground;
    private Rigidbody2D rb;
    // upgrade menu ranks
	private int maneuverabiltyRank = 0;
	private int speedRank = 0;
    private int hullStrengthRank = 0;
    private int cannonSpeedRank = 0;
    private int cannonStrengthRank = 0;
	private int[] upgradeRanks = new int[(int)Upgrade.COUNT];
    // base stats
	public float currMoveSpeed = BASE_MOVE_SPEED;
	public float currRotationSpeed = BASE_ROTATION_SPEED;
	public float currFiringDelay = BASE_FIRING_DELAY;
	public float currProjectileSpeed = BASE_PROJECTILE_SPEED;
	public float currProjectileStrength = BASE_PROJECTILE_STRENGTH;
	public float firingTimer = BASE_FIRING_DELAY;
	public float currMaxHealth = BASE_MAX_HEALTH;
    // menu checks
    private bool inGameMenuActive = false;
    private bool upgradeMenuActive = false;
    private bool anchorDown = false;
	// ayy it's dem seagulls
	public AudioClip seagullS;
	private float seagullTimer = 0;

	private Vector3 originalSpawnPos;


    // Use this for initialization
    void Start () {
        upgradeRanks[0] = maneuverabiltyRank;
        upgradeRanks[1] = speedRank;
        upgradeRanks[2] = hullStrengthRank;
        upgradeRanks[3] = cannonSpeedRank;
        upgradeRanks[4] = cannonStrengthRank;

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
        CreateUpgradeMenu();

		originalSpawnPos = transform.position;
    }

	void Update () {
        // networking check
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
                // left cannon
                CmdFireLeft();
                // reset timer
                firingTimer = currFiringDelay;
            }
            if (Input.GetKeyDown(fireRight)) {
                // right cannon
                CmdFireRight();
                // reset timer
                firingTimer = currFiringDelay;
            }
        }
        UpdateInterface();
        UpdateVariables();
    }

	void FixedUpdate () {
		UpdateSeagulls ();
	}
    [Command]   
	void CmdFireLeft () {
		GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, leftSpawn.position, Quaternion.identity);
        instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = leftSpawn.up * currProjectileSpeed;
		instantiatedProjectile.GetComponent<Projectile>().damage = currProjectileStrength;
        NetworkServer.Spawn(instantiatedProjectile);
    }
	[Command]
	void CmdFireRight() {
		GameObject instantiatedProjectile = (GameObject)Instantiate(projectile, rightSpawn.position, Quaternion.identity);
		instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = rightSpawn.up * currProjectileSpeed;
		instantiatedProjectile.GetComponent<Projectile>().damage = currProjectileStrength;
		NetworkServer.Spawn(instantiatedProjectile);
	}
	[Command]
	void CmdSpawnResources() {
		GameObject instantiatedResource = (GameObject)Instantiate(resourceObj, transform.position, Quaternion.identity);
		//instantiatedResource.GetComponent<Rigidbody2D>().velocity = rightSpawn.up * currProjectileSpeed;
		NetworkServer.Spawn(instantiatedResource);
	}
	[Command]
	void CmdUpgrade(Upgrade upgrade, bool positive) {
		if (positive) {
			if (resources >= UPGRADE_COST) {
				upgradeRanks[(int)upgrade]++;
				resources -= UPGRADE_COST;
			}
		} else {
			if (upgradeRanks[(int)upgrade] > 0) {
				upgradeRanks[(int)upgrade]--;
				resources += UPGRADE_COST;
			}
		}
	}
	[Command]
	void CmdRespawn() {
		CmdSpawnResources ();
		transform.position = originalSpawnPos;
		currentHealth = BASE_MAX_HEALTH;
	}

    private void UpdateInterface() {
        if (Input.GetKeyDown(menu)) {
            if (!inGameMenu) {
                CreateInGameMenu();
            }
            inGameMenuActive = !inGameMenuActive;
            inGameMenu.SetActive(inGameMenuActive);
        }
        if (Input.GetKeyDown(upgrade)) {
            if (!upgradeMenu) {
                CreateUpgradeMenu();
            }
            upgradeMenuActive = !upgradeMenuActive;
            upgradeMenu.SetActive(upgradeMenuActive);
        }
    }
    void OnChangePlayer(float health) {
        if (!isLocalPlayer) {
            return;
        }
		healthBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f - 0.3f * (currMaxHealth - health) / 100.0f, 0.95f);
    }
    void OnChangeResources(int resources) {
        if (!isLocalPlayer) {
            return;
        }
        resourcesText.GetComponent<Text>().text = "Resources " + resources;
    }

    private void GetMovement() {
        if (Input.GetKey(left)) {
            transform.Rotate(new Vector3(0.0f, 0.0f, currRotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(right)) {
            transform.Rotate(new Vector3(0.0f, 0.0f, -currRotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(up)) {
            transform.Translate(0.0f, currMoveSpeed * Time.deltaTime, 0.0f);
            //rb.AddForce(transform.up * moveSpeed);
        }
        if (Input.GetKeyDown(down)) {
            //rb.AddForce(-transform.up * moveSpeed / 4);
			ApplyDamage(10f);
        }
    }
    public void ApplyDamage(float damage) {
        if (!isServer) {
            return;
        }
        currentHealth -= damage;
        // respawn the player if they are dead
        if (currentHealth <= 0.0f) {
			CmdRespawn ();
        }
    }
	public void AddGold(int gold) {
		if (!isServer) {
			return;
		}
		resources += gold;
	}

    private void RenderInterface() {
        UI.CreatePanel("Profile", sprite, Color.white, canvas.transform, Vector3.zero, new Vector2(0.05f, 0.8f), new Vector2(0.2f, 0.95f));
        healthBar = UI.CreatePanel("Health Bar", null, Color.green, canvas.transform, Vector3.zero, new Vector2(0.2f, 0.9f), new Vector2(0.5f, 0.95f));
        GameObject bar = UI.CreatePanel("Bar", null, new Color(0.8f, 0.8f, 0.1f), canvas.transform, Vector3.zero, new Vector2(0.2f, 0.8f), new Vector2(0.5f, 0.9f));
		resourcesText = UI.CreateText("Resources Text", "Resources " + resources, font, Color.black, 20, bar.transform, Vector3.zero, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, true);
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
            delegate { inGameMenuActive = !inGameMenuActive; inGameMenu.SetActive(inGameMenuActive); });
        inGameMenu.SetActive(inGameMenuActive);
    }

    private void CreateUpgradeMenu() {
        upgradeMenu = UI.CreatePanel("Upgrade Menu", menuBackground, Color.white, canvas.transform,
            Vector3.zero, new Vector2(0.25f, 0.25f), new Vector3(0.75f, 0.75f));
        for(int i = 0; i < (int) Upgrade.COUNT; ++i) {
            // creating this extra variable because delegates don't work on for loop variables for some reason
            int dupe = i;
            // Upgrade Minus Button
            GameObject upgradeMinusButton = UI.CreateButton("Minus Button " + i, "-", font, Color.black, 24, upgradeMenu.transform,
                sprite, highlightedSprite, Vector3.zero, new Vector2(0.1f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.2f, 1.0f / (int)Upgrade.COUNT * (i + 1)), delegate { UpgradePlayer((Upgrade) dupe, false); UpdateVariables(); });
            // Upgrade Plus Button
            GameObject upgradePlusButton = UI.CreateButton("Plus Button " + i, "+", font, Color.black, 24, upgradeMenu.transform,
                sprite, highlightedSprite, Vector3.zero, new Vector2(0.8f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.9f, 1.0f / (int)Upgrade.COUNT * (i + 1)), delegate { UpgradePlayer((Upgrade) dupe, true); UpdateVariables(); });
            // Upgrade Text
            upgradeTexts[i] = UI.CreateText("Upgrade Text " + i, UpgradeToString((Upgrade)i), font, Color.black, 24, upgradeMenu.transform,
                Vector3.zero, new Vector2(0.3f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.7f, 1.0f / (int)Upgrade.COUNT * (i + 1)), TextAnchor.MiddleCenter, true);
        }
        upgradeMenu.SetActive(upgradeMenuActive);
    }

    private void UpgradePlayer(Upgrade upgrade, bool positive) {
		if (!isServer) {
			return;
		}
		/*if (positive) {
            if (resources >= UPGRADE_COST) {
                upgradeRanks[(int)upgrade]++;
                resources -= UPGRADE_COST;
            }
        } else {
            if (upgradeRanks[(int)upgrade] > 0) {
                upgradeRanks[(int)upgrade]--;
                resources += UPGRADE_COST;
            }
        }*/
		CmdUpgrade (upgrade, positive);
    }

    public static string UpgradeToString(Upgrade upgrade) {
        switch (upgrade) {
            case Upgrade.MANEUVERABILITY:
                return "Maneuverabilty";
            case Upgrade.SPEED:
                return "Speed";
            case Upgrade.HULL_STRENGTH:
                return "Hull Strength";
            case Upgrade.CANNON_SPEED:
                return "Cannon Speed";
            case Upgrade.CANNON_STRENGTH:
                return "Cannon Strength";
            default:
                return "";
        }
    }

    public override void OnStartLocalPlayer() {
        //GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void UpdateVariables() {
		currRotationSpeed = BASE_ROTATION_SPEED * (1 + (upgradeRanks[0] / 10.0f));
		currMoveSpeed = BASE_MOVE_SPEED * (1 + (upgradeRanks[1] / 10.0f));
		currFiringDelay = BASE_FIRING_DELAY * (1 - (upgradeRanks[2] / 10.0f));
		currProjectileSpeed = BASE_PROJECTILE_SPEED * (1 + (upgradeRanks[3] / 10.0f));
		currProjectileStrength = BASE_PROJECTILE_STRENGTH * (1 + (upgradeRanks[4] / 1.0f));
        for(int i = 0; i < (int)Upgrade.COUNT; ++i) {
            upgradeTexts[i].GetComponent<Text>().text = UpgradeToString((Upgrade)i) + ": " + upgradeRanks[i];
        }
        
    }

	private void UpdateSeagulls() {
		seagullTimer -= Time.deltaTime;
		if (seagullTimer <= 0) {
			seagullTimer = 15 + Random.value * 15;
			AudioSource.PlayClipAtPoint (seagullS, transform.position, 100.0f);
		}
	}
}
