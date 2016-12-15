﻿ using UnityEngine;
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
    public const float BASE_ROTATION_SPEED = 35.0f;
    public const float BASE_MOVE_SPEED = 10.0f;
    public const int MAX_UPGRADES = 4;
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
    // now uses mouse buttons to fire so no longer needed
    // public KeyCode fireLeft;
    // public KeyCode fireRight;
    public KeyCode menu;
    public KeyCode upgrade;
    public Transform leftSpawn;
    public Transform rightSpawn;
	public Transform[] leftSpawners;
	public Transform[] rightSpawners;
    public GameObject projectile;
	public GameObject resourceObj;


    private Camera playerCamera;
    private Canvas canvas;
    private GameObject healthBar;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private Sprite healthBarSprite;
    private Sprite resourceBarSprite;
    // GameObject references
    private GameObject inGameMenu;
    private GameObject upgradeMenu;
    private GameObject[] upgradeTexts = new GameObject[(int)Upgrade.COUNT];
    private GameObject resourcesText;
    private Sprite menuBackground;
    private Rigidbody2D rb;
    // upgrade menu ranks
	public SyncListInt upgradeRanks = new SyncListInt();
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
	private float seagullTimer = 10;
	// other sounds
	public AudioClip shotS;

	private NetworkStartPosition[] spawnPoints;

	public enum UpgradeID
	{
		MNV,
		SPD,
		HULL,
		CSPD,
		CSTR
	}


    // Use this for initialization
    void Start () {
		foreach (var value in System.Enum.GetValues(typeof(UpgradeID)))
		{
			upgradeRanks.Add (0);
		}

        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        menuBackground = Resources.Load<Sprite>("Art/Textures/Menu Background");
        font = Resources.Load<Font>("Art/Fonts/riesling");
        sprite = Resources.Load<Sprite>("Art/Textures/Button");
        highlightedSprite = Resources.Load<Sprite>("Art/Textures/HighlightedButton");
        healthBarSprite = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Health Bar");
        resourceBarSprite = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Booty Count");
        if (!isLocalPlayer) {
            return;
        }
        RenderInterface();
        CreateInGameMenu();
        CreateUpgradeMenu();

		spawnPoints = FindObjectsOfType<NetworkStartPosition>();
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
			if (/*Input.GetKeyDown(fireLeft)*/Input.GetMouseButtonDown(0) && !upgradeMenuActive) {
                // left cannon
				AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
				CmdFireLeft((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay;
            }
			if (/*Input.GetKeyDown(fireRight)*/Input.GetMouseButtonDown(1) && !upgradeMenuActive) {
                // right cannon
				AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
				CmdFireRight((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay;
            }
        }
        UpdateInterface();
        UpdateVariables();
		//CmdDisplayHealth ();
    }

	void FixedUpdate () {
		UpdateSeagulls ();
	}
    [Command]   
	void CmdFireLeft (int damageStrength) {
		//print (damageStrength);
		for (int i = 0; i < damageStrength/10; i++) {
			GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, leftSpawners[i].position, Quaternion.identity);
			instantiatedProjectile.GetComponent<Rigidbody2D> ().velocity = leftSpawners[i].up * currProjectileSpeed;
			instantiatedProjectile.GetComponent<Projectile> ().damage = damageStrength;
			NetworkServer.Spawn (instantiatedProjectile);
		}
    }
	[Command]
	void CmdFireRight(int damageStrength) {
		for (int i = 0; i < damageStrength / 10; i++) {
			GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, rightSpawners[i].position, Quaternion.identity);
			instantiatedProjectile.GetComponent<Rigidbody2D> ().velocity = rightSpawners[i].up * currProjectileSpeed;
			instantiatedProjectile.GetComponent<Projectile> ().damage = damageStrength;
			NetworkServer.Spawn (instantiatedProjectile);
		}
	}
	[Command]
	void CmdSpawnResources() {
		GameObject instantiatedResource = (GameObject)Instantiate(resourceObj, transform.position, Quaternion.identity);
		//instantiatedResource.GetComponent<Rigidbody2D>().velocity = rightSpawn.up * currProjectileSpeed;
		NetworkServer.Spawn(instantiatedResource);
	}
	[Command]
	void CmdUpgrade(Upgrade upgrade, bool positive) {
		//int upgradeMod = 0;
		if (positive) {
			if ((upgradeRanks[(int)upgrade] < MAX_UPGRADES) && (resources >= UPGRADE_COST)) {
				//upgradeMod++;
				upgradeRanks[(int)upgrade]++;
				resources -= UPGRADE_COST;
			}
		} else {
			if (upgradeRanks[(int)upgrade] > 0) {
				//upgradeMod--;
				upgradeRanks[(int)upgrade]--;
				resources += UPGRADE_COST;
			}
		}
		//upgradeRanks [(int)upgrade] += upgradeMod;
	}
	[Command]
	void CmdChangeHealth(float setHealth, bool flatSet) {
		if (flatSet) {
			currentHealth = setHealth;
		} else {
			currentHealth += setHealth;
		}
	}
	[ClientRpc]
	void RpcRespawn() {
		if (!isLocalPlayer) {
			return;
		}
		//CmdSpawnResources ();
		transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
		Vector3 dir = -transform.position;
		dir = dir.normalized;
		transform.up = dir;
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
		healthBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f - 0.3f * (currMaxHealth - health) / currMaxHealth, 0.95f);
    }
    void OnChangeResources(int resources) {
        if (!isLocalPlayer) {
            return;
        }
        resourcesText.GetComponent<Text>().text = "" + resources;
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
			//ApplyDamage(10f);
        }

    }
    public void ApplyDamage(float damage) {
        if (!isServer) {
            return;
        }
		print ("DAMAGE! " + damage);
		currentHealth -= damage;
        // respawn the player if they are dead
        if (currentHealth <= 0.0f) {
			RpcRespawn ();
			print ("Respawning " + (currMaxHealth-currentHealth) + " health");
			CmdChangeHealth(currMaxHealth, true);
        }
    }
	public void AddGold(int gold) {
		if (!isServer) {
			return;
		}
		resources += gold;
	}

    private void RenderInterface() {
        UI.CreatePanel("Profile", sprite, Color.white, canvas.transform, Vector3.zero, new Vector2(0.05f, 0.75f), new Vector2(0.2f, 0.95f));
        healthBar = UI.CreatePanel("Health", null, new Color(0.0f, 0.0f, 0.0f, 0.0f), canvas.transform, Vector3.zero, new Vector2(0.2f, 0.85f), new Vector2(0.5f, 0.95f));
        GameObject healthBarOverlay = UI.CreatePanel("Health Bar", null, Color.green, healthBar.transform, Vector3.zero, new Vector2(0.02f, 0.2f), new Vector2(0.98f, 0.75f));
        UI.CreatePanel("Health Bar Overlay", healthBarSprite, Color.white, healthBar.transform, Vector3.zero, Vector2.zero, Vector2.one);

        GameObject barOverlay = UI.CreatePanel("Resources", null, new Color(0.0f, 0.0f, 0.0f, 0.0f), canvas.transform, Vector3.zero, new Vector2(0.2f, 0.75f), new Vector2(0.5f, 0.85f));
        GameObject bar = UI.CreatePanel("Resource Bar", null, new Color(0.8f, 0.8f, 0.1f), barOverlay.transform, Vector3.zero, new Vector2(0.02f, 0.2f), new Vector2(0.98f, 0.75f));
        UI.CreatePanel("Resource Bar Overlay", resourceBarSprite, Color.white, barOverlay.transform, Vector3.zero, Vector2.zero, Vector2.one);
        resourcesText = UI.CreateText("Resources Text", "" + resources, font, Color.black, 20, bar.transform, Vector3.zero, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, true);
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
		if (!isLocalPlayer) {
			return;
		}
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
		currRotationSpeed = BASE_ROTATION_SPEED * (1 + (upgradeRanks[(int)UpgradeID.MNV] / 4.0f));
		currMoveSpeed = BASE_MOVE_SPEED * (1 + (upgradeRanks[(int)UpgradeID.SPD] / 4.0f));
		//currFiringDelay = BASE_FIRING_DELAY * (1 - (upgradeRanks[UpgradeID.CSPD] / 10.0f));
		currProjectileSpeed = BASE_PROJECTILE_SPEED * (1 + (upgradeRanks[(int)UpgradeID.CSPD] / 4.0f));
		currProjectileStrength = BASE_PROJECTILE_STRENGTH * (1 + (upgradeRanks[(int)UpgradeID.CSTR] / 1.0f));

		float oldMaxHealth = currMaxHealth;
		currMaxHealth = BASE_MAX_HEALTH * (1 + (upgradeRanks [(int)UpgradeID.HULL] / 2.0f));
		if (oldMaxHealth != currMaxHealth) {
			CmdChangeHealth(currMaxHealth - oldMaxHealth, false);
		}

        for(int i = 0; i < (int)Upgrade.COUNT; ++i) {
			upgradeTexts[i].GetComponent<Text>().text = UpgradeToString((Upgrade)i) + ": " + (upgradeRanks[i]+1);
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
