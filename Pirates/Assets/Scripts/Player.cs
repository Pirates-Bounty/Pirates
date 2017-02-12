 using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

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
	public const float BASE_RAM_DAMAGE = 5.0f;
    public static int[] UPGRADE_SCALE = { 1, 5, 20 };


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
    private Sprite upgradeButtonSprite;
    private Sprite upgradeButtonDisabledSprite;
    private Sprite healthBarSprite;
    private Sprite resourceBarSprite;
    private FogOfWar fogOfWar;
    private MapGenerator mapGenerator;
    // GameObject references
    private GameObject inGameMenu;
    private UpgradePanel upgradePanel;

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
	[SyncVar]
	public float appliedRamDamage = 0.0f;
    // menu checks
    private bool inGameMenuActive = false;
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
	public AudioClip whooshS;
    //other UI sounds
    public AudioClip sfx_upgradeMenuOpen;
    public AudioClip sfx_upgradeMenuClose;

    [SyncVar]
    public bool dead;
    public bool gofast = false;
    public float boost = BASE_BOOST;
    [SyncVar]
    public int pSpawned = 0;




    //ramming cooldown
    private IEnumerator coroutine;
    private bool invuln = false;


    // Use this for initialization
    void Start () {
        //StartCoroutine(BoatRepairs());
        if (isServer) {
			//print ("Adding to bounty manager, here we go.");
			GameObject bm = GameObject.Find ("BountyManager");
			if (bm != null) {
				playerID = bm.GetComponent<BountyManager> ().AddID ();
			}
        }


		for(int i = 0; i < (int) Upgrade.COUNT; ++i) {
            upgradeRanks.Add(0);
        }
        dead = false;

        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        menuBackground = Resources.Load<Sprite>("Art/Sprite/UI Upgrade/UI UpgradeBackDrop");
        font = Resources.Load<Font>("Art/Fonts/riesling");
        upgradeButtonSprite = Resources.Load<Sprite>("Art/Sprites/UI Upgrade/UI Upgrade Button(Purchase)");
        upgradeButtonDisabledSprite = Resources.Load<Sprite>("Art/Sprites/UI Upgrade/UI Upgrade Button(Limit)");
        healthBarSprite = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Health Bar");
        resourceBarSprite = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Booty Count");

        sfx_upgradeMenuOpen = Resources.Load<AudioClip>("Sound/SFX/UI/Paper");
        sfx_upgradeMenuClose = Resources.Load<AudioClip>("Sound/SFX/UI/PaperReverse");

        if (!isLocalPlayer) {
            return;
        }
        RenderInterface();
        CreateInGameMenu();
        upgradePanel = FindObjectOfType<UpgradePanel>();
        upgradePanel.player = this;
        upgradePanel.UpdateUI();
        upgradePanel.Hide();
        
        mapGenerator = FindObjectOfType<MapGenerator>();
        fogOfWar = FindObjectOfType<FogOfWar>();
        fogOfWar.player = this;
        fogOfWar.transform.localScale = new Vector3(mapGenerator.width, mapGenerator.height, 1);

        lowUpgrades = 0; midUpgrades = 0; highUpgrades = 0;

        
        SoundManager.Instance.SwitchBGM((int)TrackID.BGM_FIELD, 1.0f);
        InvokeRepeating("EnemyDetection", 1f, 0.5f);
    }

    void DrawLineToLeader() {
        if (!isLocalPlayer || dead) {
            return;
        }

        Debug.Log("DEBUG A");
        GameObject bm = GameObject.Find ("BountyManager");
        Debug.Log("DEBUG B");
        Player[] playerList = FindObjectsOfType<Player> ();
        Debug.Log("DEBUG C");
        if (bm == null) Debug.Log("NULL!!!!");
        Debug.Log("INCOMINGGG!!!!");
        BountyManager test = bm.GetComponent<BountyManager>();
            Debug.Log("DEBUG C2");
            int test2 = test.GetHighestBounty();
            Debug.Log("DEBUG C3");
            int leaderID = bm.GetComponent<BountyManager>().GetHighestBounty();
            Debug.Log("DEBUG D");
            Player leader = null;
            Debug.Log("DEBUG E");
        

        if (playerID == leaderID) {
            return;
        }

        for (int i = 0; i < playerList.Length; i++) {
            if (playerList[i].playerID == leaderID) {
                leader = playerList[i];
                break;
            }
        }

        //Vector3 LeaderLine = transform.position - leader.transform.position;
        //Debug.Log(LeaderLine);
        Debug.DrawLine(transform.position, leader.transform.position, Color.blue, 3.0f);

    }

    void Update()
    {


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
        fogOfWar.position = new Vector3(-transform.position.x / mapGenerator.width, -transform.position.y / mapGenerator.height, 0);
		if (boostTimer > 0) {
			boostTimer -= Time.deltaTime;
		} else {
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				SpeedBoost ();
				//AudioSource.PlayClipAtPoint (whooshS, transform.position, 100.0f);
				SoundManager.Instance.PlaySFX (whooshS, 1.0f);
				gofast = true;
				boostTimer = currBoostDelay;
			}
		}

        if (gofast == true && boost > 0)
        {
			boost -= Time.deltaTime;
            currMoveSpeed -= Time.deltaTime * currMoveSpeed;
			currRotationSpeed += Time.deltaTime * currRotationSpeed;
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
            if (/*Input.GetKeyDown(fireLeft)*/Input.GetMouseButtonDown(0) && !upgradePanel.gameObject.activeSelf)
            {
                // left cannon
                //AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireLeft((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay;
            }
            if (/*Input.GetKeyDown(fireRight)*/Input.GetMouseButtonDown(1) && !upgradePanel.gameObject.activeSelf)
            {
                // right cannon
                //AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireRight((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && !upgradePanel.gameObject.activeSelf)
            {
                // triple volley - fire all at once
                //AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireLeftVolley((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay; //+2.0f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && !upgradePanel.gameObject.activeSelf)
            {
                // triple shotgun spray
                //AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireLeftTriple((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay; //+2.0f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && !upgradePanel.gameObject.activeSelf)
            {
                // front shot
                //AudioSource.PlayClipAtPoint(shotS, transform.position, 100.0f);
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireBowChaser((int)currProjectileStrength);
                // reset timer
                firingTimer = currFiringDelay+0.7f; //+2.0f;
            }
        }
        UpdateInterface();
        UpdateVariables();
        //CmdDisplayHealth ();

        DrawLineToLeader();

        //SOUND - SoundManager reposition & BGMswitch debugger (space key)
        if (GameObject.Find("SoundManager") != null)
        {
            GameObject.Find("SoundManager").transform.position = GameObject.Find("Camera").transform.position;
        }


    }


	void FixedUpdate () {
		UpdateSeagulls ();
		creakTimer -= Time.deltaTime;
	}

    void SpeedBoost()
    {
        currMoveSpeed *= 5;
		currRotationSpeed /= 5;
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
			int upgradePrice = UPGRADE_COST * UPGRADE_SCALE[upgradeRanks[(int) upgrade]];

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
            if (currentHealth + setHealth <= currMaxHealth)
            {
                currentHealth += setHealth;
            }
            else
            {
                currentHealth = currMaxHealth;
            }
			
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
	[Command]
	void CmdApplyDamage(float damage, int enemyID) {
		if (dead || currentHealth <= 0) {
			return;
		}
		print ("DAMAGE! " + damage);
		currentHealth -= damage;
		// respawn the player if they are dead
		if (currentHealth <= 0.0f) {
            //AudioSource.PlayClipAtPoint(deathS, transform.position, 100.0f);
            SoundManager.Instance.PlaySFX(deathS, 1.0f);
            RpcRespawn ();

			GameObject bm = GameObject.Find ("BountyManager");
			if (bm != null) {
				bm.GetComponent<BountyManager> ().ReportHit (playerID, enemyID);
			}
		}
	}
	[Command]
	void CmdSetRamDamage(float ramDam) {
		appliedRamDamage = ramDam;
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
		boatBase.GetComponent<SpriteRenderer>().sprite = bases[upgradeRanks[(int)Upgrade.HULL]];
        sail.GetComponent<SpriteRenderer>().sprite = sails[upgradeRanks[(int) Upgrade.SPEED]];
        rudder.GetComponent<SpriteRenderer>().sprite = rudders[upgradeRanks[(int)Upgrade.AGILITY]];
        cannon.GetComponent<SpriteRenderer>().sprite = cannons[upgradeRanks[(int)Upgrade.CANNON]];
		ram.GetComponent<SpriteRenderer>().sprite = rams[upgradeRanks[(int)Upgrade.RAM]];
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
            upgradePanel.gameObject.SetActive(!upgradePanel.gameObject.activeSelf);

            if (upgradePanel.gameObject.activeSelf)
                SoundManager.Instance.PlaySFX(sfx_upgradeMenuOpen,0.15f);
            else
                SoundManager.Instance.PlaySFX(sfx_upgradeMenuClose,0.15f);
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
                //AudioSource.PlayClipAtPoint (turnS, transform.position, 0.7f);
                SoundManager.Instance.PlaySFX(turnS, 0.7f);
            }

            float turnVelocity = Mathf.Max(currRotationSpeed, currRotationSpeed * currVelocity*0.1f);

            transform.Rotate(new Vector3(0.0f, 0.0f, turnVelocity * Time.deltaTime));

            //             transform.Rotate(new Vector3(0.0f, 0.0f, currRotationSpeed * Time.deltaTime));
        }

        if (Input.GetKey(right)) {
			if (creakTimer <= 0 && Input.GetKeyDown (right)) {
				creakTimer = 3.0f;
				//AudioSource.PlayClipAtPoint (turnS, transform.position, 0.7f);
                SoundManager.Instance.PlaySFX(turnS, 0.7f);
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
			//currVelocity = Mathf.Max(-currMoveSpeed/2f, currVelocity - currMoveSpeed*.75f * Time.deltaTime);
			currVelocity = Mathf.Max(-currMoveSpeed * (1+(currRotationSpeed/BASE_ROTATION_SPEED/4))/2f, currVelocity - currMoveSpeed*.85f * Time.deltaTime);
			//transform.Translate (0.0f, -currMoveSpeed / 4 * Time.deltaTime, 0.0f);
			//rb.AddForce(-transform.up * currMoveSpeed*1000 / 4 * Time.deltaTime);
			//CmdApplyDamage(10f, playerID);
		} else {
			if (currVelocity > 0) {
				currVelocity = Mathf.Max (0f, currVelocity - currMoveSpeed / 2f * Time.deltaTime);
			} else if (currVelocity < 0) {
				currVelocity = Mathf.Min (0f, currVelocity + currMoveSpeed / 2f * Time.deltaTime);
			}
		}
		transform.Translate (0.0f, currVelocity * Time.deltaTime, 0.0f);
		CmdSetRamDamage (currRamDamage * currVelocity / BASE_MOVE_SPEED);
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(2f);
        GameObject[] sl = GameObject.FindGameObjectsWithTag("spawner");
        GameObject farthestSpawn = sl[0];
        float maxDistSum = 0;
        foreach (GameObject g in sl)
        {
            float distSum = 0;
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (p != this.gameObject)
                {
                    distSum += (g.transform.position - p.transform.position).sqrMagnitude;
                }

            }
            if (distSum > maxDistSum)
            {
                maxDistSum = distSum;
                farthestSpawn = g;
            }
        }
        transform.position = farthestSpawn.transform.position;
		Vector3 dir = -transform.position;
		dir = dir.normalized;
		transform.up = dir;
        CmdChangeHealth(currMaxHealth, true);

		CmdDeath (false);
    }



    IEnumerator BoatRepairs()
    {
        while (true)
        {

            yield return new WaitForSeconds(2);
            if (currentHealth != currMaxHealth && !dead)
            {
                if (GetComponent<Rigidbody2D>().velocity.SqrMagnitude() >= 0 && GetComponent<Rigidbody2D>().velocity.SqrMagnitude() <= .5)
                {
                    CmdChangeHealth(5, false);
                }
            }
        }

    }

	public void ApplyDamage(float damage, int enemyID) {
		CmdApplyDamage (damage, enemyID);
	}


    public void OnCollisionEnter2D(Collision2D collision) {
		if (!isLocalPlayer) {
			return;
		}

		//if rammed
		Player otherPlayer = collision.collider.gameObject.GetComponent<Player> ();
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
		RaycastHit2D hit = Physics2D.Raycast(collision.gameObject.transform.position, collision.gameObject.transform.up);

		if (otherPlayer != null && collision.collider.gameObject.CompareTag("Player") && hit.collider != null && hit.collider.tag == "Player" && !invuln) {
			ApplyDamage(otherPlayer.appliedRamDamage, otherPlayer.playerID);
            //AudioSource.PlayClipAtPoint(ramS, transform.position, 100.0f);
            SoundManager.Instance.PlaySFX(ramS, 1.0f);
            //3 second invulnerability before you can take ram damage again
            StartCoroutine (RamInvuln ());
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
        upgradePanel.UpdateUI();
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
            upgradeButtonSprite, upgradeButtonDisabledSprite, Vector3.zero, new Vector2(0.25f, 0.7f), new Vector2(0.75f, 0.9f),
            delegate {
                UI.CreateYesNoDialog("Confirmation", "Are you sure?",
                 font, Color.black, 24, menuBackground, upgradeButtonSprite, upgradeButtonDisabledSprite,
                 Color.white, inGameMenu.transform, Vector3.zero, new Vector2(0.25f, 0.1f),
                 new Vector2(0.75f, 0.9f),
                 delegate {
                     CancelInvoke("EnemyDetection");
                     Navigator.Instance.LoadLevel("Menu");

                 });
            });
        GameObject optionButton = UI.CreateButton("Options Button", "Options", font, Color.black, 24, inGameMenu.transform,
            upgradeButtonSprite, upgradeButtonDisabledSprite, Vector3.zero, new Vector2(0.25f, 0.4f), new Vector2(0.75f, 0.6f), delegate {; });
        GameObject returnButton = UI.CreateButton("Return to Game Button", "Return to Game", font, Color.black, 24, inGameMenu.transform,
            upgradeButtonSprite, upgradeButtonDisabledSprite, Vector3.zero, new Vector2(0.25f, 0.1f), new Vector2(0.75f, 0.3f),
            delegate { inGameMenuActive = !inGameMenuActive; inGameMenu.SetActive(inGameMenuActive); });
        inGameMenu.SetActive(inGameMenuActive);
    }

  //  private void CreateUpgradeMenu() {
		//upgradeMenu = UI.CreatePanel("Upgrade Menu", menuBackground, new Color(1.0f, 1.0f, 1.0f, 0.85f), canvas.transform,
  //          Vector3.zero, new Vector2(0.25f, 0.25f), new Vector3(0.75f, 0.75f));
  //      for(int i = 0; i < (int) Upgrade.COUNT; ++i) {
  //          // creating this extra variable because delegates don't work on for loop variables for some reason
  //          int dupe = i;
  //          // Upgrade Plus Button
  //          GameObject upgradePlusButton = UI.CreateButton("Upgrade Button " + i, "+", font, Color.black, 24, upgradeMenu.transform,
  //              upgradeButtonSprite, upgradeButtonDisabledSprite, Vector3.zero, new Vector2(0.7f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.9f, 1.0f / (int)Upgrade.COUNT * (i + 1)), delegate { UpgradePlayer((Upgrade) dupe, true); UpdateVariables(); });
  //          // Upgrade Text
  //          upgradeTexts[i] = UI.CreateText("Upgrade Text " + i, UpgradeToString((Upgrade)i), font, Color.black, 24, upgradeMenu.transform,
  //              Vector3.zero, new Vector2(0.1f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.5f, 1.0f / (int)Upgrade.COUNT * (i + 1)), TextAnchor.MiddleCenter, true);
		//	costTexts[i] = UI.CreateText("Cost Text " + i, UPGRADE_COST + "g", font, Color.black, 24, upgradeMenu.transform,
		//		Vector3.zero, new Vector2(0.75f, 1.0f / (int)Upgrade.COUNT * i), new Vector2(0.95f, 1.0f / (int)Upgrade.COUNT * (i + 1)), TextAnchor.MiddleCenter, true);
  //          //Highlight Sound
  //          UnityEngine.EventSystems.EventTrigger.Entry entry_highlight = new UnityEngine.EventSystems.EventTrigger.Entry(); //entry object creation
  //          entry_highlight.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter; //setting the trigger type; how is it triggered
  //          entry_highlight.callback.AddListener((data) => SoundManager.Instance.PlaySFX(GameObject.Find("SoundManager").GetComponent<SoundManager>().highlightAudio,0.3f)); //call function=> playAudio(...)
  //          upgradePlusButton.AddComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(entry_highlight);
  //      }
		////UpdateVariables ();
  //      upgradeMenu.SetActive(upgradeMenuActive);
  //  }

    public void UpgradePlayer(Upgrade upgrade, bool positive) {
		if (!isLocalPlayer) {
			return;
		}
		CmdUpgrade (upgrade, positive);
        UpdateVariables();
		//UpdateSprites ();
    }

    public override void OnStartLocalPlayer() {
        //GetComponent<SpriteRenderer>().color = Color.red;
        //UpdateSprites();
    }

    private void UpdateVariables() {
        if (gofast == false) // only update movement speed if not in boost mode
        {
            currMoveSpeed = BASE_MOVE_SPEED * (1 + (upgradeRanks[(int)Upgrade.SPEED] / 2.0f));
			currRotationSpeed = BASE_ROTATION_SPEED * (1 + (upgradeRanks[(int)Upgrade.AGILITY] / 3.0f));
        }
		//currFiringDelay = BASE_FIRING_DELAY * (1 - (upgradeRanks[UpgradeID.CSPD] / 10.0f));
		//currProjectileSpeed = BASE_PROJECTILE_SPEED * (1 + (upgradeRanks[(int)UpgradeID.CSPD] / 4.0f));
		currRamDamage = BASE_RAM_DAMAGE * (1 + (upgradeRanks[(int)Upgrade.RAM] / 1.5f));
		currProjectileStrength = BASE_PROJECTILE_STRENGTH * (1 + (upgradeRanks[(int)Upgrade.CANNON] / 1.0f));
		currBoostDelay = BASE_BOOST_DELAY * (1 - (upgradeRanks[(int)Upgrade.AGILITY] / 5.0f));

		float oldMaxHealth = currMaxHealth;
		currMaxHealth = BASE_MAX_HEALTH * (1 + (upgradeRanks [(int)Upgrade.HULL] / 2.0f));
		if (oldMaxHealth != currMaxHealth) {
			CmdChangeHealth(currMaxHealth - oldMaxHealth, false);
		}

    }

	private void UpdateSeagulls() {
		seagullTimer -= Time.deltaTime;
		if (seagullTimer <= 0) {
			seagullTimer = 15 + Random.value * 15;
            //AudioSource.PlayClipAtPoint (seagullS, transform.position, 100.0f);
            SoundManager.Instance.PlaySFX(seagullS, 1.0f);
		}
	}

    //SOUND - battle bgm
    private void EnemyDetection()
    {
        LayerMask detectionLayer;
        Collider2D[] detectionResult = new Collider2D[10];

        detectionLayer = 1 << 9; // layerName: Player
        int detectionBattle = Physics2D.OverlapCircleNonAlloc(transform.position, 60.0f, detectionResult, detectionLayer);
        int detectionEscape = Physics2D.OverlapCircleNonAlloc(transform.position, 90.0f, detectionResult, detectionLayer);

        if (detectionBattle > 1 && SoundManager.Instance.NowPlaying() == (int)TrackID.BGM_FIELD)
            SoundManager.Instance.SwitchBGM((int)TrackID.BGM_BATTLE, 2.0f);
        if (detectionEscape <= 1)
            SoundManager.Instance.SwitchBGM((int)TrackID.BGM_FIELD, 2.0f);
    }
}
