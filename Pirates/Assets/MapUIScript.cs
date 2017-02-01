using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class MapUIScript : MonoBehaviour {


    public Slider landFrequency;
    public Slider waterFrequency;
    public Slider sandFrequency;
    public MapGenerator mapGen;
    public bool changeLand = false;
    public bool changeWater = false;
    public bool changeSand = false;
    public LobbyManager lman;
    public Toggle randSeed;
    private int origSeed;
    
	// Use this for initialization
	void Start () {
        origSeed = mapGen.seed;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            SliderChange();
        }
	}


    public void SliderChange()
    {
        if (changeLand)
        {
            changeLand = false;
            
            float diff = 1 - landFrequency.value;

            mapGen.waterFreq = (diff / 2);
            mapGen.sandFreq = (diff / 2);
            mapGen.landFreq = landFrequency.value;

        }
        else if (changeWater)
        {
            changeWater = false;
            float diff = 1 - waterFrequency.value;

            mapGen.landFreq = (diff / 2);
            mapGen.sandFreq = (diff / 2);
            mapGen.waterFreq = waterFrequency.value;

        }
        else if (changeSand)
        {
            changeSand = false;
            float diff = 1 - sandFrequency.value;

            mapGen.waterFreq = (diff / 2);
            mapGen.landFreq = (diff / 2);
            mapGen.sandFreq = sandFrequency.value;

        }

        waterFrequency.value = mapGen.waterFreq;
        sandFrequency.value = mapGen.sandFreq;
        landFrequency.value = mapGen.landFreq;
        changeWater = false;
        changeLand = false;
        changeSand = false;
    }


    public void waterChange()
    {
        changeWater = true;
    }

    public void sandChange()
    {
        changeSand = true;
    }

    public void landChange()
    {
        changeLand = true;
    }

    public void changeToLobby()
    {
        gameObject.SetActive(false);
        mapGen.reGenerate();
    }

    public void toggleRandomSeed()
    {
        if (randSeed.isOn)
        {
            Debug.Log(System.DateTime.Now.Millisecond);
            mapGen.seed = System.DateTime.Now.Millisecond;
        }
        else
        {
            Debug.Log(origSeed);
            mapGen.seed = origSeed;
        }
    }

    
}
