 using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
public enum Upgrade {
    MANEUVERABILITY, //rotation speed
    SPEED, // move speed
    HULL_STRENGTH, // health
    //CANNON_SPEED, // cannon firing time
	RAM_STRENGTH, // ram damage
    CANNON_STRENGTH, // cannon damage
    COUNT //num items in the enum
}
public enum DamageType {
    CANNON,
    RAM
}
public class Player : NetworkBehaviour {
    // const vars
    public const float BASE_MAX_HEALTH = 100.0f;
    public const float BASE_PROJECTILE_SPEED = 70.0f;
	public const float BASE_PROJECTILE_STRENGTH = 10.0f;
    public const float BASE_FIRING_DELAY = 1.0f;
    public const float BASE_BOOST_DELAY = 5.0f;
    public const float BASE_BOOST = 1.2f;
    public const float BASE_ROTATION_SPEED = 35.0f;
    public const float BASE_MOVE_SPEED = 10.0f;
    public const int MAX_UPGRADES = 3;
    public const int UPGRADE_COST = 100;
	public const float BASE_RAM_DAMAGE = 20.0f;


    [SyncVar]
	public int playerID = -2;
	[SyncVar]
	public int lowUpgrades = 0;
	[SyncVar]
	public int midUpgrades = 0;
	[SyncVar]
	public int highUpgrades = 0;

    [SyncVar(hook = "OnChangePlayer")]
	public float currentHealth = BASE_MAX_HEALTH;
    [SyncVar(hook = "OnChangeResources")]
    public int resources = 1000;
    public Sprite[] bases;
    public Sprite[] sails;
    public Sprite[] rudders;
    public Sprite[] cannons;
    public Sprite[] rams;
    public GameObject boatBase;
    public GameObject sail;
    public GameObject rudder;
    public GameObject cannon;
    public GameObject ram;
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
    public Transform[] frontSpawners;
    public GameObject projectile;
	public GameObject resourceObj;
	public GameObject deathExplode;


    private Camera playerCamera;
    private Canvas canvas;
    private GameObject healthBar;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private Sprite healthBarSprite;
    private Sprite resourceBarSprite;
    private bool spawned = false;
    // GameObject references
    private GameObject inGameMenu;
    private GameObject upgradeMenu;
    private GameObject[] upgradeTexts = new GameObject[(int)Upgrade.COUNT];
	private GameObject[] costTexts = new GameObject[(int)Upgrade.COUNT];
    private GameObject resourcesText;
    private Sprite menuBackground;
    private Rigidbody2D rb;
    // upgrade menu ranks
	public SyncListInt upgradeRanks = new SyncListInt();
    // base stats
	public float currMoveSpeed = BASE_MOVE_SPEED;
	public float currRotationSpeed = BASE_ROTATION_SPEED;
	public float currFiringDelay = BASE_FIRING_DELAY;
    public float currBoostDelay = BASE_BOOST_DELAY;
	//public float currProjectileSpeed = BASE_PROJECTILE_SPEED;
	public float currRamDamage = BASE_RAM_DAMAGE;
	public float currProjectileStrength = BASE_PROJECTILE_STRENGTH;
	public float firingTimer = BASE_FIRING_DELAY;
    public float boostTimer = BASE_BOOST_DELAY;
	public float currMaxHealth = BASE_MAX_HEALTH;
	public float currVelocity = 0.0f;
    // menu checks
    private bool inGameMenuActive = false;
    private bool upgradeMenuActive = false;
    private bool anchorDown = false;
	// ayy it's dem seagulls
	public AudioClip seagullS;
	private float seagullTimer = 10;
	// other sounds
	public AudioClip shotS;
	public AudioClip turnS;
	private float creakTimer = 0;
    public AudioClip ramS;
    public AudioClip deathS;

	private NetworkStartPosition[] spawnPoints;
    [SyncVar]
    public bool dead;
    public bool gofast = false;
    public float boost = BASE_BOOST;
    [SyncVar]
    public int pSpawned = 0;

    public GameObject MapGen;

	public enum UpgradeID
	{
		MNV,
		SPD,
		HULL,
		RSTR,
		CSTR
	}




    //ramming cooldown
    private IEnumerator coroutine;
    private bool invuln = false;


    // Use this for initialization
    void Start () {
		if (isServer) {
			//print ("Adding to bounty manager, here we go.");
			GameObject bm = GameObject.Find ("BountyManager");
			if (bm != null) {
				playerID = bm.GetComponent<BountyManager> ().AddID ();
			}
        }


		foreach (var value in System.Enum.GetValues(typeof(UpgradeID)))
		{
			upgradeRanks.Add (0);
		}
        dead = false;

        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        menuBackground = Resources.Load<Sprite>("Art/Textures/Menu Background");
        font = Resources.Load<Font>("Art/Fonts/riesling");
        sprite = Resources.Load<Sprite>("Art/Sprites/UPDATED 12-19-16/UI 11-19-16/Golden Button Unpushed");
        highlightedSprite = Resources.Load<Sprite>("Art/Sprites/UPDATED 12-19-16/UI 11-19-16/Golden Button Pushed");
        healthBarSprite = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Health Bar");
        resourceBarSprite = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Booty Count");
        if (!isLocalPlayer) {
            return;
        }
        RenderInterface();
        CreateInGameMenu();
        CreateUpgradeMenu();

		lowUpgrades = 0; midUpgrades = 0; highUpgrades = 0;



       
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();

    //    if (!isLocalPlayer)
    //    {
    //        spawnPlayer(0);
    //    }

    //}

    //public void spawnPlayer(int ind)
    //{
    //    if (GameObject.FindGameObjectsWithTag("spawner")[ind].GetComponent<SpawnScript>().spawned == false)
    //    {
    //        transform.position = GameObject.FindGameObjectsWithTag("spawner")[ind].transform.position;
    //        GameObject.FindGameObjectsWithTag("spawner")[ind].GetComponent<SpawnScript>().spawned = true;
    //    }
    //    else if (ind >= GameObject.FindGameObjectsWithTag("spawner").Length)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        spawnPlayer(ind + 1);
    //    }
    //}


    /*[ClientRpc]
    public void RpcIncrement()
    {
        pSpawned++;
    }

    [Command]
    public void CmdIncrement()
    {
        RpcIncrement();
    }*/


    void Update()
    {
        
        if (!spawned)
        {
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("spawner");
			if (spawners.Length >= LobbyManager.numPlayers && playerID >= 0)
            {
                spawned = true;
				transform.position = spawners[playerID].transform.position;
                //CmdIncrement();
            }
        }
        if (playerID < 0 && isServer)
        {
            //print ("Better late than never, adding to bounty manager.");
            GameObject bm = GameObject.Find("BountyManager");
            if (bm != null)
            {
                playerID = bm.GetComponent<BountyManager>().AddID();
            }
        }

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    RpcRespawn();
        //}

        UpdateSprites ();
        // networking check
        if (!isLocalPlayer || dead)
        {
            return;
        }
        
        // update the camera's position
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);

        boostTimer -= Time.deltaTime;
        if (boostTimer < 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SpeedBoost();
                gofast = true;
                boostTimer = BASE_BOOST_DELAY;
            }
        }

        if (gofast == true && boost > 0)
        {
           boost -= Time.deltaTime;
            currMoveSpeed -= Time.deltaTime * currMoveSpeed;
        }
        else if (boost < 0)
        {
            gofast = false;
            boost = BASE_BOOST;
        }

        // get player's movement
        GetMovement();

        firingTimer -= Time.deltaTime;
        if (firingTimer < 0)
        {
            // fire cannons
            if (/*Input.GetKeyDown(fireLeft)*/Input.GetMouseButtonDown(0) && !upgradeMenuActive)
            {
                // left cannon
                AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                CmdFireLeft((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay;
            }
            if (/*Input.GetKeyDown(fireRight)*/Input.GetMouseButtonDown(1) && !upgradeMenuActive)
            {
                // right cannon
                AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                CmdFireRight((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && !upgradeMenuActive)
            {
                // triple volley - fire all at once
                AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                CmdFireLeftVolley((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay; //+2.0f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && !upgradeMenuActive)
            {
                // triple shotgun spray
                AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                CmdFireLeftTriple((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay; //+2.0f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && !upgradeMenuActive)
            {
                // front shot
                AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                CmdFireBowChaser((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay+0.7f; //+2.0f;
            }
        }
        UpdateInterface();
        UpdateVariables();
        //CmdDisplayHealth ();
    }


	void FixedUpdate () {
		UpdateSeagulls ();
		creakTimer -= Time.deltaTime;
	}

    void SpeedBoost()
    {
        currMoveSpeed *= 5;
    }

    [Command]   
	void CmdFireLeft (int damageStrength) {
		//print (damageStrength);
		for (int i = 0; i < damageStrength/10; i++) {
			GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, leftSpawners[i].position, Quaternion.identity);
			instantiatedProjectile.GetComponent<Rigidbody2D> ().velocity = leftSpawners[i].up * BASE_PROJECTILE_SPEED;
			//instantiatedProjectile.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile.GetComponent<Projectile> ().assignedID = playerID;
			NetworkServer.Spawn (instantiatedProjectile);
		}
    }
	[Command]
	void CmdFireRight(int damageStrength) {
		for (int i = 0; i < damageStrength / 10; i++) {
			GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, rightSpawners[i].position, Quaternion.identity);
			instantiatedProjectile.GetComponent<Rigidbody2D> ().velocity = rightSpawners[i].up * BASE_PROJECTILE_SPEED;
			//instantiatedProjectile.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile.GetComponent<Projectile> ().assignedID = playerID;
			NetworkServer.Spawn (instantiatedProjectile);
		}
	}
    [Command]
    void CmdFireLeftVolley (int damageStrength) {
        //triple volley shot
        for (int i = 0; i < 3; i++) {
            GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, leftSpawners[i].position, Quaternion.identity);
            instantiatedProjectile.GetComponent<Rigidbody2D> ().velocity = leftSpawners[i].up * BASE_PROJECTILE_SPEED/2;
            //instantiatedProjectile1.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile.GetComponent<Projectile> ().assignedID = playerID;
            NetworkServer.Spawn (instantiatedProjectile);
        }
    }
    [Command]
    void CmdFireLeftTriple (int damageStrength) {
        //triple spread shot

            GameObject instantiatedProjectile1 = (GameObject)Instantiate (projectile, leftSpawners[0].position, Quaternion.identity);
            instantiatedProjectile1.GetComponent<Rigidbody2D> ().velocity = Quaternion.Euler(0, 0, 45) * leftSpawners[0].up * BASE_PROJECTILE_SPEED;
            //instantiatedProjectile1.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile1.GetComponent<Projectile> ().assignedID = playerID;
            NetworkServer.Spawn (instantiatedProjectile1);

            //angled 45 degrees backward
            GameObject instantiatedProjectile2 = (GameObject)Instantiate (projectile, leftSpawners[1].position, Quaternion.identity);
            instantiatedProjectile2.GetComponent<Rigidbody2D> ().velocity = leftSpawners[1].up * BASE_PROJECTILE_SPEED;
            //instantiatedProjectile1.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile2.GetComponent<Projectile> ().assignedID = playerID;
            NetworkServer.Spawn (instantiatedProjectile2);

            GameObject instantiatedProjectile3 = (GameObject)Instantiate (projectile, leftSpawners[2].position, Quaternion.identity);
            instantiatedProjectile3.GetComponent<Rigidbody2D> ().velocity = Quaternion.Euler(0, 0, -45) * leftSpawners[2].up * BASE_PROJECTILE_SPEED;
            //instantiatedProjectile1.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile3.GetComponent<Projectile> ().assignedID = playerID;
            NetworkServer.Spawn (instantiatedProjectile3);
    }
    [Command]
    void CmdFireBowChaser (int damageStrength) {
        //forward firing cannon
        for (int i = 0; i < damageStrength/10; i++) {
            GameObject instantiatedProjectile = (GameObject)Instantiate (projectile, frontSpawners[0].position, Quaternion.identity);
            instantiatedProjectile.GetComponent<Rigidbody2D> ().velocity = frontSpawners[0].up * BASE_PROJECTILE_SPEED;
            //instantiatedProjectile1.GetComponent<Projectile> ().damage = damageStrength;
			instantiatedProjectile.GetComponent<Projectile> ().assignedID = playerID;
            NetworkServer.Spawn (instantiatedProjectile);
        }
    }
	[Command]
	void CmdSpawnResources(Vector3 pos) {
        GameObject instantiatedResource = Instantiate(resourceObj,pos,Quaternion.identity) as GameObject;
        NetworkServer.Spawn(instantiatedResource);
    }
	[Command]
	void CmdUpgrade(Upgrade upgrade, bool positive) {
		//int upgradeMod = 0;
		if (positive) {
			int upgradePrice = UPGRADE_COST;
			switch (upgradeRanks [(int)upgrade]) {
			case 0:
				break;
			case 1:
				upgradePrice *= 5;
				break;
			case 2:
				upgradePrice *= 20;
				break;
			default:
				upgradePrice *= 50;
				print ("That's a pricy upgrade...");
				break;
			}

			if ((upgradeRanks[(int)upgrade] < MAX_UPGRADES) && (resources >= upgradePrice)) {
				//upgradeMod++;
				upgradeRanks[(int)upgrade]++;
				resources -= upgradePrice;
				switch (upgradeRanks [(int)upgrade]) {
				case 1:
					lowUpgrades++;
					break;
				case 2:
					midUpgrades++;
					break;
				case 3:
					highUpgrades++;
					break;
				default:
					print ("Erm, unexpected upgrade rank");
					break;
				}
			}
		} /*else {
			if (upgradeRanks[(int)upgrade] > 0) {
				//upgradeMod--;
				upgradeRanks[(int)upgrade]--;
				resources += UPGRADE_COST;
			}
		}*/
        //UpdateSprites();
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
	[Command]
	void CmdDeath (bool isDead) {
		dead = isDead;
		if (isDead) {
			GetComponent<Collider2D>().enabled = false;
			gameObject.transform.FindChild ("Sprite").gameObject.SetActive (false);
			GameObject instantiatedResource = Instantiate(deathExplode,transform.position,Quaternion.identity) as GameObject;
			NetworkServer.Spawn(instantiatedResource);
		} else {
			GetComponent<Collider2D>().enabled = true;
			gameObject.transform.FindChild ("Sprite").gameObject.SetActive (true);
			RpcFinishRespawn ();
		}
	}
	[ClientRpc]
	void RpcRespawn () {
		gameObject.transform.FindChild ("Sprite").gameObject.SetActive (false);
		if (!isLocalPlayer) {
			return;
		}
		CmdDeath (true);
		StartCoroutine(Death());
	}
	[ClientRpc]
	void RpcFinishRespawn () {
		gameObject.transform.FindChild ("Sprite").gameObject.SetActive (true);
	}
    void UpdateSprites() {
		boatBase.GetComponent<SpriteRenderer>().sprite = bases[upgradeRanks[(int)UpgradeID.HULL]];
        sail.GetComponent<SpriteRenderer>().sprite = sails[upgradeRanks[(int) UpgradeID.SPD]];
        rudder.GetComponent<SpriteRenderer>().sprite = rudders[upgradeRanks[(int)UpgradeID.MNV]];
        cannon.GetComponent<SpriteRenderer>().sprite = cannons[upgradeRanks[(int)UpgradeID.CSTR]];
		ram.GetComponent<SpriteRenderer>().sprite = rams[upgradeRanks[(int)UpgradeID.RSTR]];
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
		if (Input.GetKey(left)) { // click left
			if (creakTimer <= 0 && Input.GetKeyDown (left)) {
				creakTimer = 3.0f;
				AudioSource.PlayClipAtPoint (turnS, transform.position, 0.7f);
			}

            float turnVelocity = Mathf.Max(currRotationSpeed, currRotationSpeed * currVelocity*0.1f);

            transform.Rotate(new Vector3(0.0f, 0.0f, turnVelocity * Time.deltaTime));

            //             transform.Rotate(new Vector3(0.0f, 0.0f, currRotationSpeed * Time.deltaTime));
        }

        if (Input.GetKey(right)) {
			if (creakTimer <= 0 && Input.GetKeyDown (right)) {
				creakTimer = 3.0f;
				AudioSource.PlayClipAtPoint (turnS, transform.position, 0.7f);
			}

            float turnVelocity = Mathf.Max(currRotationSpeed, currRotationSpeed * currVelocity * 0.1f);

            transform.Rotate(new Vector3(0.0f, 0.0f, -turnVelocity * Time.deltaTime));

            //             transform.Rotate(new Vector3(0.0f, 0.0f, -currRotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey (up)) {
 			currVelocity = Mathf.Min (currMoveSpeed, currVelocity + currMoveSpeed * Time.deltaTime);
			//transform.Translate (0.0f, currMoveSpeed * Time.deltaTime, 0.0f);
			//rb.AddForce(transform.up * currMoveSpeed*1000 * Time.deltaTime);
		} else if (Input.GetKey (down)) {
			currVelocity = Mathf.Max(-currMoveSpeed/2f, currVelocity - currMoveSpeed*.75f * Time.deltaTime);
			//transform.Translate (0.0f, -currMoveSpeed / 4 * Time.deltaTime, 0.0f);
			//rb.AddForce(-transform.up * currMoveSpeed*1000 / 4 * Time.deltaTime);
			//ApplyDamage(10f, playerID);
		} else {
			if (currVelocity > 0) {
				currVelocity = Mathf.Max (0f, currVelocity - currMoveSpeed / 2f * Time.deltaTime);
			} else if (currVelocity < 0) {
				currVelocity = Mathf.Min (0f, currVelocity + currMoveSpeed / 2f * Time.deltaTime);
			}
		}
		transform.Translate (0.0f, currVelocity * Time.deltaTime, 0.0f);

    }

    IEnumerator Death()
    {
        /*dead = true;
        GetComponent<Collider2D>().enabled = false;
		gameObject.transform.FindChild ("Sprite").gameObject.SetActive (false);*/
        //CmdSpawnResources(transform.position);
        
        yield return new WaitForSeconds(2f);
        //Debug.Log(LobbyManager.numPlayers);
        GameObject[] sl = GameObject.FindGameObjectsWithTag("spawner");
        transform.position = sl[Random.Range(0,sl.Length)].transform.position;
		Vector3 dir = -transform.position;
		dir = dir.normalized;
		transform.up = dir;
        CmdChangeHealth(currMaxHealth, true);

		/*GetComponent<Collider2D>().enabled = true;
		gameObject.transform.FindChild ("Sprite").gameObject.SetActive (true);
        dead = false;*/

		CmdDeath (false);
    }



	public void ApplyDamage(float damage, int enemyID) {
		if (!isServer || dead || currentHealth <= 0) {
            return;
        }
		//print ("DAMAGE! " + damage);
		currentHealth -= damage;
        // respawn the player if they are dead
        if (currentHealth <= 0.0f) {
            AudioSource.PlayClipAtPoint(deathS, transform.position, 100.0f);
			RpcRespawn ();

			GameObject bm = GameObject.Find ("BountyManager");
			if (bm != null) {
				bm.GetComponent<BountyManager> ().ReportHit (playerID, enemyID);
			}
        }
    }
    public void OnCollisionEnter2D(Collision2D collision) {
        //if rammed

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        // Debug.Log(transform.position + " " + hit.point);
        // Debug.DrawLine(transform.position, hit.point, Color.red, 3);

        // print(collision.gameObject.CompareTag("Player") + " " + (hit.collider != null) + " " + (hit.collider.tag == "Player") + " " + (!invuln));
        // print(hit.collider.tag);
        if (collision.gameObject.CompareTag("Player") && hit.collider != null && hit.collider.tag == "Player" && !invuln) {
            collision.gameObject.GetComponent<Player>().ApplyDamage(currRamDamage*(currVelocity/currMoveSpeed), playerID);
            AudioSource.PlayClipAtPoint(ramS, transform.position, 100.0f);
            //3 second invulnerability before you can take ram damage again
            collision.gameObject.GetComponent<Player>().coroutine = collision.gameObject.GetComponent<Player>().RamInvuln();
            collision.gameObject.GetComponent<Player>().StartCoroutine(collision.gameObject.GetComponent<Player>().coroutine);
        }
    }
    private IEnumerator RamInvuln() {
        //make player invulnerable to ramming for X seconds (currently 3)
         invuln = true;
         yield return new WaitForSeconds(3);
         invuln = false;
    }

	public void AddGold(int gold) {
		if (!isServer) {
			return;
		}
		resources += gold;
	}

    private void RenderInterface() {
        //UI.CreatePanel("Profile", sprite, Color.white, canvas.transform, Vector3.zero, new Vector2(0.05f, 0.75f), new Vector2(0.2f, 0.95f));
        healthBar = UI.CreatePanel("Health", null, new Color(0.0f, 0.0f, 0.0f, 0.0f), canvas.transform, Vector3.zero, new Vector2(0.2f, 0.85f), new Vector2(0.5f, 0.95f));
        GameObject healthBarOverlay = UI.CreatePanel("Health Bar", null, Color.green, healthBar.transform, Vector3.zero, new Vector2(0.02f, 0.2f), new Vector2(0.98f, 0.75f));
		UI.CreatePanel("Health Bar Overlay", healthBarSprite, Color.white, canvas.transform, Vector3.zero, new Vector2(0.2f, 0.85f), new Vector2(0.5f, 0.95f));

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
		upgradeMenu = UI.CreatePanel("Upgrade Menu", menuBackground, new Color(1.0f, 1.0f, 1.0f, 0.85f), canvas.transform,
            Vector3.zero, new Vector2(0.25f, 0.25f), new Vector3(0.75f, 0.75f));
        for(int i = 0; i < (int) Upgrade.COUNT; ++i) {
            // creating this extra variable because delegates don't work on for loop variables for some reason
            int dupe = i;
            // Upgrade Minus Button
            /*GameObject upgradeMinusButton = UI.CreateButton("Minus Button " + i, "-", font, Color.black, 24, upgradeMenu.transform,
                sprite, highlightedSprite, Vector3.zero, new Vector2(0.1f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.2f, 1.0f / (int)Upgrade.COUNT * (i + 1)), delegate { UpgradePlayer((Upgrade) dupe, false); UpdateVariables(); });/**/
            // Upgrade Plus Button
            GameObject upgradePlusButton = UI.CreateButton("Plus Button " + i, "+", font, Color.black, 24, upgradeMenu.transform,
                sprite, highlightedSprite, Vector3.zero, new Vector2(0.6f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.7f, 1.0f / (int)Upgrade.COUNT * (i + 1)), delegate { UpgradePlayer((Upgrade) dupe, true); UpdateVariables(); });
            // Upgrade Text
            upgradeTexts[i] = UI.CreateText("Upgrade Text " + i, UpgradeToString((Upgrade)i), font, Color.black, 24, upgradeMenu.transform,
                Vector3.zero, new Vector2(0.1f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.5f, 1.0f / (int)Upgrade.COUNT * (i + 1)), TextAnchor.MiddleCenter, true);
			costTexts[i] = UI.CreateText("Cost Text " + i, UPGRADE_COST + "g", font, Color.black, 24, upgradeMenu.transform,
				Vector3.zero, new Vector2(0.75f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.95f, 1.0f / (int)Upgrade.COUNT * (i + 1)), TextAnchor.MiddleCenter, true);
        }
		//UpdateVariables ();
        upgradeMenu.SetActive(upgradeMenuActive);
    }

    private void UpgradePlayer(Upgrade upgrade, bool positive) {
		if (!isLocalPlayer) {
			return;
		}
		CmdUpgrade (upgrade, positive);
		//UpdateSprites ();
    }

    public static string UpgradeToString(Upgrade upgrade) {
        switch (upgrade) {
            case Upgrade.MANEUVERABILITY:
                return "Maneuverabilty";
            case Upgrade.SPEED:
                return "Speed";
            case Upgrade.HULL_STRENGTH:
                return "Hull Strength";
			case Upgrade.RAM_STRENGTH:
                return "Ram Strength";
            case Upgrade.CANNON_STRENGTH:
                return "Cannon Strength";
            default:
                return "";
        }
    }

    public override void OnStartLocalPlayer() {
        //GetComponent<SpriteRenderer>().color = Color.red;
        //UpdateSprites();
    }

    private void UpdateVariables() {
		currRotationSpeed = BASE_ROTATION_SPEED * (1 + (upgradeRanks[(int)UpgradeID.MNV] / 3.0f));
        if (gofast == false) // only update movement speed if not in boost mode
        {
            currMoveSpeed = BASE_MOVE_SPEED * (1 + (upgradeRanks[(int)UpgradeID.SPD] / 2.0f));
        }
		//currFiringDelay = BASE_FIRING_DELAY * (1 - (upgradeRanks[UpgradeID.CSPD] / 10.0f));
		//currProjectileSpeed = BASE_PROJECTILE_SPEED * (1 + (upgradeRanks[(int)UpgradeID.CSPD] / 4.0f));
		currRamDamage = BASE_RAM_DAMAGE * (1 + (upgradeRanks[(int)UpgradeID.RSTR] / 1.5f));
		currProjectileStrength = BASE_PROJECTILE_STRENGTH * (1 + (upgradeRanks[(int)UpgradeID.CSTR] / 1.0f));

		float oldMaxHealth = currMaxHealth;
		currMaxHealth = BASE_MAX_HEALTH * (1 + (upgradeRanks [(int)UpgradeID.HULL] / 2.0f));
		if (oldMaxHealth != currMaxHealth) {
			CmdChangeHealth(currMaxHealth - oldMaxHealth, false);
		}

        for(int i = 0; i < (int)Upgrade.COUNT; ++i) {
			upgradeTexts[i].GetComponent<Text>().text = UpgradeToString((Upgrade)i) + ": " + (upgradeRanks[i]);
			//costTexts [i].GetComponent<Text> ().text = upgradePrices[i] + "g";
			if (upgradeRanks [i] == 0) {
				costTexts [i].GetComponent<Text> ().text = UPGRADE_COST + "g";
			} else if (upgradeRanks [i] == 1) {
				costTexts [i].GetComponent<Text> ().text = UPGRADE_COST * 5 + "g";
			} else if (upgradeRanks [i] == 2) {
				costTexts [i].GetComponent<Text> ().text = UPGRADE_COST * 20 + "g";
			} else {
				costTexts [i].GetComponent<Text> ().text = "SOLD OUT";
			}
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
