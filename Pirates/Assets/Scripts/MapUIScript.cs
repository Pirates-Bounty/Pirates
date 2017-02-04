using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class MapUIScript : MonoBehaviour {


    public Slider landFrequency;
    public MapGenerator mapGen;
    public Toggle randSeed;
    public GameObject mapPanel;
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

        mapGen.landFreq = landFrequency.value;

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

    public void LobbyButton()
    {
        mapPanel.SetActive(false);
        mapGen.CmdReGenerate();
    }

    
}
