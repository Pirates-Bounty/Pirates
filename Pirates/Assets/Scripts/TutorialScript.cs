using UnityEngine;
using System.Collections;

//To trigger the functions, write the example code below
//if(Navigator.Tutorial) GameObject.Find("TutorialGO").GetComponent<TutorialScript>().OnMoveLeft();
public class TutorialScript : MonoBehaviour
{
    GameObject[] tutorialList;
    GameObject activeText;

    KeyCode keyNext;

    const int MOVESIZE = 4;
    int progress;
    int counter;
    int coinCheck;
    bool[] moveCheck;
    bool moveComplete;
    bool boostCheck;
    bool hillEnterCheck;
    bool hillCaptureCheck;
    bool hillDespawnCheck;
    bool enemyDestroyCheck;
    bool upgradeMenuCheck;
    bool upgradeShipCheck;

    void Awake()
    {
        keyNext = KeyCode.Return;
        progress = 0;
        counter = 0;
        moveCheck = new bool[MOVESIZE];
        for (int i = 0; i < MOVESIZE; i++)
            moveCheck[i] = false;
        moveComplete = false;
        boostCheck = false;
        hillEnterCheck = false;
        hillCaptureCheck = false;
        hillDespawnCheck = false;
        enemyDestroyCheck = false;
        upgradeMenuCheck = false;
        upgradeShipCheck = false;
        coinCheck = 0;

        tutorialList = new GameObject[transform.childCount];
        for(int i=0; i<transform.childCount; i++)
            tutorialList[i] = transform.GetChild(i).gameObject;

        activeText = tutorialList[0].transform.GetChild(0).gameObject;
        activeText.SetActive(true);

    }

    // Use this for initialization
    void Start()
    {
        if (Navigator.Tutorial == false)
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(keyNext))
            NextDialog();

        if (counter + 1 == tutorialList[progress].transform.childCount) //if it's the last dialog of the mission, aka if the mission is ongoing...
        {
            switch (progress)
            {
                case 0: //WASD tutorial
                    moveComplete = true;
                    for (int i = 0; i < 4; i++)
                        if (!moveCheck[i])
                            moveComplete = false;
                    if (moveComplete)
                        NextCase();
                    break;

                case 1: //boost tutorial
                    if (boostCheck)
                        NextCase();
                    break;

                case 2: //OnCoinTaken
                    if (coinCheck >= 3)
                        NextCase();

                    break;

                case 3: //hillprepare sound
                    SoundManager.Instance.PlaySFX_HillPrepare();
                    NextCase();
                    break;

                case 4: // !---spawnhill---!
                        //insert spawnhill command here
                    NextCase();
                    break;

                case 5: // hill enter tutorial
                    if (hillEnterCheck)
                        NextCase();
                    break;

                case 6: // hill capture tutorial
                    if (hillCaptureCheck)
                        NextCase();
                    break;

                case 7: // hill despawn tutorial
                    if (hillDespawnCheck)
                        NextCase();
                    break;

                case 8: // !---spawnenemy---!
                         //insert spawnenemy command here
                    NextCase();
                    break;

                case 9: // enemydestroy tutorial
                    if (enemyDestroyCheck)
                        NextCase();
                    break;

                case 10: // upgrademenu tutorial
                    if (upgradeMenuCheck)
                        NextCase();
                    break;

                case 11: // upgraded
                    if (upgradeShipCheck)
                        NextCase();
                    break;
            }
        }
    }

    private void NextCase()
    {
        if (progress+1 < transform.childCount)
        {
            activeText.SetActive(false);
            counter = 0;
            activeText = tutorialList[++progress].transform.GetChild(counter).gameObject;
            activeText.SetActive(true);
        }
    }

    private void NextDialog()
    {
        if (counter+1 < tutorialList[progress].transform.childCount)
        {
            activeText.SetActive(false);
            activeText = tutorialList[progress].transform.GetChild(++counter).gameObject;
            activeText.SetActive(true);
        }
    }

    public void OnMoveForward()
    {
        if (progress == 0)
            moveCheck[0] = true;
    }

    public void OnMoveLeft()
    {
        if (progress == 0)
            moveCheck[1] = true;
    }

    public void OnMoveBackward()
    {
        if (progress == 0)
            moveCheck[2] = true;
    }

    public void OnMoveRight()
    {
        if (progress == 0)
            moveCheck[3] = true;
    }

    public void OnBoost()
    {
        if (progress == 1)
            boostCheck = true;
    }
    public void OnCoinTake()
    {
        if (progress == 2)
            coinCheck++;
    }

    public void OnHillEnter()
    {
        if (progress == 5)
            hillEnterCheck = true;
    }

    public void OnHillCapture()
    {
        if (progress == 6)
            hillCaptureCheck = true;
    }

    public void OnHillDespawn()
    {
        if (progress == 7)
            hillDespawnCheck = true;
    }

    public void OnEnemyDestroy()
    {
        if (progress == 9)
            enemyDestroyCheck = true;
    }

    public void OnUpgradeMenu()
    {
        if (progress == 10)
            upgradeMenuCheck = true;
    }

    public void OnUpgradeShip()
    {
        if (progress == 11)
            upgradeShipCheck = true;
    }

}
