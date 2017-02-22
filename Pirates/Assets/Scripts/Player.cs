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
    public const float MAX_SHOTS = 4.0f;
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
    public int resources;
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

    public GameObject leaderArrow;
    private Player[] playerList;

    private Camera playerCamera;
    private Canvas canvas;
    private GameObject healthBar;
    private RectTransform healthBarRect;
    private GameObject purpleCannon;
    private RectTransform purpleCannonRect;
    private GameObject redCannon;
    private RectTransform redCannonRect;
    private Font font;
    private Text resourcesText;
    //private FogOfWar fogOfWar;
    //private MapGenerator mapGenerator;
    // GameObject references
    //private GameObject inGameMenu;
    private UpgradePanel upgradePanel;
    private GameObject respawnTimerText;
    private Image sprintCooldownImage;
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
    public float firingTimerLeft = BASE_FIRING_DELAY;
	public float firingTimerRight = BASE_FIRING_DELAY;
    public float boostTimer = BASE_BOOST_DELAY;
    public float currMaxHealth = BASE_MAX_HEALTH;
    public float currVelocity = 0.0f;
    public float numPurpleShots = MAX_SHOTS;
    public float numRedShots = MAX_SHOTS;
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
    void Start() {
        playerList = FindObjectsOfType<Player>();
        //StartCoroutine(BoatRepairs());
        if (isServer) {
            //print ("Adding to bounty manager, here we go.");
            GameObject bm = GameObject.Find("BountyManager");
            if (bm != null) {
                playerID = bm.GetComponent<BountyManager>().AddID();
            }
        }


        for (int i = 0; i < (int)Upgrade.COUNT; ++i) {
            upgradeRanks.Add(0);
        }
        dead = false;

        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        font = Resources.Load<Font>("Art/Fonts/riesling");

        sfx_upgradeMenuOpen = Resources.Load<AudioClip>("Sound/SFX/UI/Paper");
        sfx_upgradeMenuClose = Resources.Load<AudioClip>("Sound/SFX/UI/PaperReverse");

        if (!isLocalPlayer) {
            return;
        }
        //CreateInGameMenu();

        leaderArrow = GameObject.Find("Canvas/UI/Compass");

        upgradePanel = FindObjectOfType<UpgradePanel>();
        upgradePanel.player = this;
        upgradePanel.UpdateUI();
        upgradePanel.Hide();
        healthBar = GameObject.Find("Canvas/UI/Health Bar");
        healthBarRect = healthBar.GetComponent<RectTransform>();
        purpleCannon = GameObject.Find("Canvas/UI/Purple Cannon");
        purpleCannonRect = purpleCannon.GetComponent<RectTransform>();
        redCannon = GameObject.Find("Canvas/UI/Red Cannon");
        redCannonRect = redCannon.GetComponent<RectTransform>();
        sprintCooldownImage = GameObject.Find("Canvas/UI/Sprint Cooldown").GetComponent<Image>();
        resourcesText = GameObject.Find("Canvas/UI/Bounty Display Text").GetComponent<Text>();

        respawnTimerText = UI.CreateText("Respawn Timer Text", "10", font, Color.black, 200, canvas.transform,
            Vector3.zero, new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.7f), TextAnchor.MiddleCenter, true);
        respawnTimerText.SetActive(false);
        Text timerText = respawnTimerText.GetComponent<Text>();
        timerText.resizeTextMaxSize = timerText.fontSize;

        //mapGenerator = FindObjectOfType<MapGenerator>();
        /*fogOfWar = FindObjectOfType<FogOfWar>();
        fogOfWar.player = this;
        fogOfWar.transform.localScale = new Vector3(mapGenerator.width, mapGenerator.height, 1);*/

        lowUpgrades = 0; midUpgrades = 0; highUpgrades = 0;


        SoundManager.Instance.SwitchBGM((int)TrackID.BGM_FIELD, 1.0f);
        InvokeRepeating("EnemyDetection", 1f, 0.5f);
    }

    void Update() {

        if (playerID < 0 && isServer) {
            //print ("Better late than never, adding to bounty manager.");
            GameObject bm = GameObject.Find("BountyManager");
            if (bm != null) {
                playerID = bm.GetComponent<BountyManager>().AddID();
            }
        }

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    RpcRespawn();
        //}

        UpdateSprites();
        // networking check
        if (!isLocalPlayer || dead) {
            return;
        }

        // update the camera's position
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        //fogOfWar.position = new Vector3(-transform.position.x / mapGenerator.width, -transform.position.y / mapGenerator.height, 0);
        HandleBoost();
        // get player's movement
        GetMovement();
        GetCannonFire();
        UpdateInterface();
        UpdateVariables();
        //CmdDisplayHealth ();

        DrawLineToLeader();

        //SOUND - SoundManager reposition & BGMswitch debugger (space key)
        if (GameObject.Find("SoundManager") != null) {
            GameObject.Find("SoundManager").transform.position = GameObject.Find("Camera").transform.position;
        }


    }

    void DrawLineToLeader() {
        //For all players who are not in the lead, draws a local onscreen arrow pointing to the leader.
        //Encourages constant fighting due to all non-leader players having a location they can go to
        //To find a player

        //TODO: hide at beginning, hide until certain bounty is earned, hide when leader near
        //TODO: make this a compass on the corner of the screen instead of under the boat
        if (!isLocalPlayer || dead || currentHealth <= 0) {
            leaderArrow.SetActive(false);
            return;
        }

        GameObject bm = GameObject.Find("BountyManager");
        if (bm == null) {
            return;
        }

        if (playerList.Length < LobbyManager.numPlayers) 
            playerList = FindObjectsOfType<Player>();

        if (playerList.Length <= 1) {
            leaderArrow.SetActive(false);
            return;
        }

        int leaderID = bm.GetComponent<BountyManager>().GetHighestBounty();
        Player leader = null;

        if (playerID == leaderID) {
            leaderArrow.SetActive(false);
            return;
        }

        for (int i = 0; i < playerList.Length; i++) {
            if (playerList[i].playerID == leaderID) {
                leader = playerList[i];
                break;
            }
        }
        if (leader == null) {
            leaderArrow.SetActive(false);
            return;
        }
        leaderArrow.SetActive(true);

        leaderArrow.transform.up = (leader.transform.position - transform.position).normalized;
    }

    void HandleBoost() {
        if (boostTimer > 0) {
            boostTimer -= Time.deltaTime;
        } else {
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                SpeedBoost();
                SoundManager.Instance.PlaySFX(whooshS, 1.0f);
                gofast = true;
                boostTimer = currBoostDelay;
            }
        }

        if (gofast == true && boost > 0) {
            boost -= Time.deltaTime;
            currMoveSpeed -= Time.deltaTime * currMoveSpeed;
            currRotationSpeed += Time.deltaTime * currRotationSpeed;
        } else if (boost < 0) {
            gofast = false;
            boost = BASE_BOOST;
        }
    }
    void GetCannonFire() {
        if (firingTimerLeft > 0) {
			firingTimerLeft -= Time.deltaTime;
		} else {
			// fire cannons
			if ((Input.GetMouseButtonDown (0) || Input.GetKeyDown (KeyCode.LeftArrow)) && !upgradePanel.gameObject.activeSelf && numPurpleShots >= 1) {
				// left cannon
				SoundManager.Instance.PlaySFX (shotS, 1.0f);
				CmdFireLeft ((int)currProjectileStrength);
				// reset timer
				firingTimerLeft = currFiringDelay;
				numPurpleShots--;
			}
		}

		if (firingTimerRight > 0) {
			firingTimerRight -= Time.deltaTime;
		} else {
			if ((Input.GetMouseButtonDown (1) || Input.GetKeyDown (KeyCode.RightArrow)) && !upgradePanel.gameObject.activeSelf && numRedShots >= 1) {
				// right cannon
				SoundManager.Instance.PlaySFX (shotS, 1.0f);
				CmdFireRight ((int)currProjectileStrength);
				// reset timer
				firingTimerRight = currFiringDelay;
				numRedShots--;
			}
		}
		if (firingTimerLeft > 0 && firingTimerRight > 0) {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !upgradePanel.gameObject.activeSelf) {
                // triple volley - fire all at once
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireLeftVolley((int)currProjectileStrength);
                // reset timer
                firingTimerLeft = currFiringDelay; //+2.0f;
				firingTimerRight = currFiringDelay; //+2.0f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && !upgradePanel.gameObject.activeSelf) {
                // triple shotgun spray
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireLeftTriple((int)currProjectileStrength);
                // reset timer
				firingTimerLeft = currFiringDelay; //+2.0f;
				firingTimerRight = currFiringDelay; //+2.0f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && !upgradePanel.gameObject.activeSelf) {
                // front shot
                SoundManager.Instance.PlaySFX(shotS, 1.0f);
                CmdFireBowChaser((int)currProjectileStrength);
                // reset timer
				firingTimerLeft = currFiringDelay; //+2.0f;
				firingTimerRight = currFiringDelay; //+2.0f;
            }
        }
    }

    void FixedUpdate() {
        UpdateSeagulls();
        creakTimer -= Time.deltaTime;
    }

    void SpeedBoost() {
        currMoveSpeed *= 5;
        currRotationSpeed /= 5;
    }

    [Command]
    void CmdFireLeft(int damageStrength) {
        //print (damageStrength);
        for (int i = 0; i < damageStrength / 10; i++) {
            GameObject instantiatedProjectile = (GameObject)Instantiate(projectile, leftSpawners[i].position, Quaternion.identity);
            instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = leftSpawners[i].up * BASE_PROJECTILE_SPEED;
            instantiatedProjectile.GetComponent<Projectile>().assignedID = playerID;
            NetworkServer.Spawn(instantiatedProjectile);
        }
    }
    [Command]
    void CmdFireRight(int damageStrength) {
        for (int i = 0; i < damageStrength / 10; i++) {
            GameObject instantiatedProjectile = (GameObject)Instantiate(projectile, rightSpawners[i].position, Quaternion.identity);
            instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = rightSpawners[i].up * BASE_PROJECTILE_SPEED;
            instantiatedProjectile.GetComponent<Projectile>().assignedID = playerID;
            NetworkServer.Spawn(instantiatedProjectile);
        }
    }
    [Command]
    void CmdFireLeftVolley(int damageStrength) {
        //triple volley shot
        for (int i = 0; i < 3; i++) {
            GameObject instantiatedProjectile = (GameObject)Instantiate(projectile, leftSpawners[i].position, Quaternion.identity);
            instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = leftSpawners[i].up * BASE_PROJECTILE_SPEED / 2;
            instantiatedProjectile.GetComponent<Projectile>().assignedID = playerID;
            NetworkServer.Spawn(instantiatedProjectile);
        }
    }
    [Command]
    void CmdFireLeftTriple(int damageStrength) {
        //triple spread shot

        GameObject instantiatedProjectile1 = (GameObject)Instantiate(projectile, leftSpawners[0].position, Quaternion.identity);
        instantiatedProjectile1.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 45) * leftSpawners[0].up * BASE_PROJECTILE_SPEED;
        instantiatedProjectile1.GetComponent<Projectile>().assignedID = playerID;
        NetworkServer.Spawn(instantiatedProjectile1);

        //angled 45 degrees backward
        GameObject instantiatedProjectile2 = (GameObject)Instantiate(projectile, leftSpawners[1].position, Quaternion.identity);
        instantiatedProjectile2.GetComponent<Rigidbody2D>().velocity = leftSpawners[1].up * BASE_PROJECTILE_SPEED;
        instantiatedProjectile2.GetComponent<Projectile>().assignedID = playerID;
        NetworkServer.Spawn(instantiatedProjectile2);

        GameObject instantiatedProjectile3 = (GameObject)Instantiate(projectile, leftSpawners[2].position, Quaternion.identity);
        instantiatedProjectile3.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -45) * leftSpawners[2].up * BASE_PROJECTILE_SPEED;
        instantiatedProjectile3.GetComponent<Projectile>().assignedID = playerID;
        NetworkServer.Spawn(instantiatedProjectile3);
    }
    [Command]
    void CmdFireBowChaser(int damageStrength) {
        //forward firing cannon
        for (int i = 0; i < damageStrength / 10; i++) {
            GameObject instantiatedProjectile = (GameObject)Instantiate(projectile, frontSpawners[0].position, Quaternion.identity);
            instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = frontSpawners[0].up * BASE_PROJECTILE_SPEED;
            instantiatedProjectile.GetComponent<Projectile>().assignedID = playerID;
            NetworkServer.Spawn(instantiatedProjectile);
        }
    }
    [Command]
    void CmdSpawnResources(Vector3 pos) {
        GameObject instantiatedResource = Instantiate(resourceObj, pos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(instantiatedResource);
    }
    [Command]
    void CmdUpgrade(Upgrade upgrade, bool positive) {
        if (positive) {
            int upgradePrice = UPGRADE_COST * UPGRADE_SCALE[upgradeRanks[(int)upgrade]];

            if ((upgradeRanks[(int)upgrade] < MAX_UPGRADES) && (resources >= upgradePrice)) {
                upgradeRanks[(int)upgrade]++;
                resources -= upgradePrice;
                switch (upgradeRanks[(int)upgrade]) {
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
                        print("Erm, unexpected upgrade rank");
                        break;
                }
            }
        }
    }
    [Command]
    void CmdChangeHealth(float setHealth, bool flatSet) {
        if (flatSet) {
            currentHealth = setHealth;
        } else {
            if (currentHealth + setHealth <= currMaxHealth) {
                currentHealth += setHealth;
            } else {
                currentHealth = currMaxHealth;
            }

        }
    }
    [Command]
    void CmdDeath(bool isDead) {
        dead = isDead;
        if (isDead) {
            GetComponent<Collider2D>().enabled = false;
            gameObject.transform.FindChild("Sprite").gameObject.SetActive(false);
            GameObject instantiatedResource = Instantiate(deathExplode, transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(instantiatedResource);
        } else {
            GetComponent<Collider2D>().enabled = true;
            gameObject.transform.FindChild("Sprite").gameObject.SetActive(true);
            RpcFinishRespawn();
        }
    }
    [Command]
    void CmdApplyDamage(float damage, int enemyID) {
        if (dead || currentHealth <= 0) {
            return;
        }
        print("DAMAGE! " + damage);
        currentHealth -= damage;
        // respawn the player if they are dead
        if (currentHealth <= 0.0f) {
            SoundManager.Instance.PlaySFX(deathS, 1.0f);
            RpcRespawn();

            GameObject bm = GameObject.Find("BountyManager");
            if (bm != null) {
                bm.GetComponent<BountyManager>().ReportHit(playerID, enemyID);
            }
        }
    }
    [Command]
    void CmdSetRamDamage(float ramDam) {
        appliedRamDamage = ramDam;
    }
    [ClientRpc]
    void RpcRespawn() {
        gameObject.transform.FindChild("Sprite").gameObject.SetActive(false);
        if (!isLocalPlayer) {
            return;
        }
        CmdDeath(true);
        StartCoroutine(Death());
    }
    [ClientRpc]
    void RpcFinishRespawn() {
        gameObject.transform.FindChild("Sprite").gameObject.SetActive(true);
    }
    void UpdateSprites() {
        boatBase.GetComponent<SpriteRenderer>().sprite = bases[upgradeRanks[(int)Upgrade.HULL]];
        sail.GetComponent<SpriteRenderer>().sprite = sails[upgradeRanks[(int)Upgrade.SPEED]];
        rudder.GetComponent<SpriteRenderer>().sprite = rudders[upgradeRanks[(int)Upgrade.AGILITY]];
        cannon.GetComponent<SpriteRenderer>().sprite = cannons[upgradeRanks[(int)Upgrade.CANNON]];
        ram.GetComponent<SpriteRenderer>().sprite = rams[upgradeRanks[(int)Upgrade.RAM]];
    }
    private void UpdateInterface() {
        if (Input.GetKeyDown(menu)) {
            if (upgradePanel.gameObject.activeSelf) {
                upgradePanel.gameObject.SetActive(false);
            }
        }
        if (Input.GetKeyDown(upgrade) && !inGameMenuActive) {
            upgradePanel.gameObject.SetActive(!upgradePanel.gameObject.activeSelf);

            if (upgradePanel.gameObject.activeSelf)
                SoundManager.Instance.PlaySFX(sfx_upgradeMenuOpen, 0.15f);
            else
                SoundManager.Instance.PlaySFX(sfx_upgradeMenuClose, 0.15f);
        }
        purpleCannonRect.anchorMin = new Vector2(0.13f + 0.22f * (MAX_SHOTS - numPurpleShots) / MAX_SHOTS, purpleCannonRect.anchorMin.y);
        redCannonRect.anchorMax = new Vector2(0.66f - 0.22f * (MAX_SHOTS - numRedShots) / MAX_SHOTS, redCannonRect.anchorMax.y);
        sprintCooldownImage.fillAmount = boostTimer / currBoostDelay;
    }
    void OnChangePlayer(float newHealth) {
        if (!isLocalPlayer) {
            return;
        }
        currentHealth = newHealth;
        healthBarRect.anchorMax = new Vector2(0.67f - 0.545f * (currMaxHealth - currentHealth) / currMaxHealth, healthBarRect.anchorMax.y);
    }
    void OnChangeResources(int newResources) {
        if (!isLocalPlayer) {
            return;
        }
        resources = newResources;
        resourcesText.text = resources + "g";
        upgradePanel.UpdateUI();
    }

    private void GetMovement() {
        if (Input.GetKey(left)) { // click left
            if (creakTimer <= 0 && Input.GetKeyDown(left)) {
                creakTimer = 3.0f;
                SoundManager.Instance.PlaySFX(turnS, 0.7f);
            }
            float turnVelocity = Mathf.Max(currRotationSpeed, currRotationSpeed * currVelocity * 0.1f);
            transform.Rotate(new Vector3(0.0f, 0.0f, turnVelocity * Time.deltaTime));
        }

        if (Input.GetKey(right)) {
            if (creakTimer <= 0 && Input.GetKeyDown(right)) {
                creakTimer = 3.0f;
                SoundManager.Instance.PlaySFX(turnS, 0.7f);
            }
            float turnVelocity = Mathf.Max(currRotationSpeed, currRotationSpeed * currVelocity * 0.1f);
            transform.Rotate(new Vector3(0.0f, 0.0f, -turnVelocity * Time.deltaTime));
        }
        if (Input.GetKey(up)) {
            currVelocity = Mathf.Min(currMoveSpeed, currVelocity + currMoveSpeed * Time.deltaTime);
        } else if (Input.GetKey(down)) {
            currVelocity = Mathf.Max(-currMoveSpeed * (1 + (currRotationSpeed / BASE_ROTATION_SPEED / 4)) / 2f, currVelocity - currMoveSpeed * .85f * Time.deltaTime);
        } else {
            if (currVelocity > 0) {
                currVelocity = Mathf.Max(0f, currVelocity - currMoveSpeed / 2f * Time.deltaTime);
            } else if (currVelocity < 0) {
                currVelocity = Mathf.Min(0f, currVelocity + currMoveSpeed / 2f * Time.deltaTime);
            }
        }
        transform.Translate(0.0f, currVelocity * Time.deltaTime, 0.0f);
        CmdSetRamDamage(currRamDamage * currVelocity / BASE_MOVE_SPEED);
    }

    IEnumerator Death() {
        yield return new WaitForSeconds(2f);

        Text timerText = respawnTimerText.GetComponent<Text>();
        respawnTimerText.SetActive(true);
        for (int i = 5; i > 0; i--) {
            timerText.text = i + "";
            yield return new WaitForSeconds(1f);
        }
        respawnTimerText.SetActive(false);

        GameObject[] sl = GameObject.FindGameObjectsWithTag("spawner");
        GameObject farthestSpawn = sl[0];
        float maxDistSum = 0;
        foreach (GameObject g in sl) {
            float distSum = 0;
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
                if (p != this.gameObject) {
                    distSum += (g.transform.position - p.transform.position).sqrMagnitude;
                }

            }
            if (distSum > maxDistSum) {
                maxDistSum = distSum;
                farthestSpawn = g;
            }
        }
        transform.position = farthestSpawn.transform.position;
        Vector3 dir = -transform.position;
        dir = dir.normalized;
        transform.up = dir;
        CmdChangeHealth(currMaxHealth, true);

        CmdDeath(false);
    }



    IEnumerator BoatRepairs() {
        while (true) {

            yield return new WaitForSeconds(2);
            if (currentHealth != currMaxHealth && !dead) {
                if (GetComponent<Rigidbody2D>().velocity.SqrMagnitude() >= 0 && GetComponent<Rigidbody2D>().velocity.SqrMagnitude() <= .5) {
                    CmdChangeHealth(5, false);
                }
            }
        }

    }

    public void ApplyDamage(float damage, int enemyID) {
        CmdApplyDamage(damage, enemyID);
    }


    public void OnCollisionEnter2D(Collision2D collision) {
        if (!isLocalPlayer) {
            return;
        }

        //if rammed
        Player otherPlayer = collision.collider.gameObject.GetComponent<Player>();
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        RaycastHit2D hit = Physics2D.Raycast(collision.gameObject.transform.position, collision.gameObject.transform.up);

        if (otherPlayer != null && collision.collider.gameObject.CompareTag("Player") && hit.collider != null && hit.collider.tag == "Player" && !invuln) {
            ApplyDamage(otherPlayer.appliedRamDamage, otherPlayer.playerID);
            SoundManager.Instance.PlaySFX(ramS, 1.0f);
            //3 second invulnerability before you can take ram damage again
            StartCoroutine(RamInvuln());
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
        //upgradePanel.UpdateUI();
    }

    public void UpgradePlayer(Upgrade upgrade, bool positive) {
        if (!isLocalPlayer) {
            return;
        }
        CmdUpgrade(upgrade, positive);
        UpdateVariables();
        //UpdateSprites ();
    }

    public override void OnStartLocalPlayer() {
    }

    private void UpdateVariables() {
        if (gofast == false) // only update movement speed if not in boost mode
        {
            currMoveSpeed = BASE_MOVE_SPEED * (1 + (upgradeRanks[(int)Upgrade.SPEED] / 2.0f));
            currRotationSpeed = BASE_ROTATION_SPEED * (1 + (upgradeRanks[(int)Upgrade.AGILITY] / 3.0f));
        }
        currRamDamage = BASE_RAM_DAMAGE * (1 + (upgradeRanks[(int)Upgrade.RAM] / 1.5f));
        currProjectileStrength = BASE_PROJECTILE_STRENGTH * (1 + (upgradeRanks[(int)Upgrade.CANNON] / 1.0f));
        currBoostDelay = BASE_BOOST_DELAY * (1 - (upgradeRanks[(int)Upgrade.AGILITY] / 5.0f));

        float oldMaxHealth = currMaxHealth;
        currMaxHealth = BASE_MAX_HEALTH * (1 + (upgradeRanks[(int)Upgrade.HULL] / 2.0f));
        if (oldMaxHealth != currMaxHealth) {
            CmdChangeHealth(currMaxHealth - oldMaxHealth, false);
        }
		if (firingTimerLeft <= 0) {
			numPurpleShots += Time.deltaTime;
			numPurpleShots = Mathf.Clamp (numPurpleShots, 0, MAX_SHOTS);
		}
		if (firingTimerRight <= 0) {
			numRedShots += Time.deltaTime;
			numRedShots = Mathf.Clamp (numRedShots, 0, MAX_SHOTS);
		}
    }

    private void UpdateSeagulls() {
        seagullTimer -= Time.deltaTime;
        if (seagullTimer <= 0) {
            seagullTimer = 15 + Random.value * 15;
            SoundManager.Instance.PlaySFX(seagullS, 1.0f);
        }
    }

    //SOUND - battle bgm
    private void EnemyDetection() {
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
