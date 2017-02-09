using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public enum Upgrade {
    RAM, // ram damage
    CANNON, // cannon damage
    HULL, // health
    AGILITY, //rotation speed
    SPEED, // move speed    
    //CANNON_SPEED, // cannon firing time
    COUNT //num items in the enum
}

public class UpgradePanel : MonoBehaviour {
    public Player player;
    public Text[] costTexts = new Text[(int)Upgrade.COUNT];
    // Use this for initialization
    void Start () {
        UpdateUI();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Hide() {
        gameObject.SetActive(false);
    }

    public static string UpgradeToString(Upgrade upgrade) {
        switch (upgrade) {
            case Upgrade.AGILITY:
                return "Agility";
            case Upgrade.SPEED:
                return "Speed";
            case Upgrade.HULL:
                return "Hull";
            case Upgrade.RAM:
                return "Ram";
            case Upgrade.CANNON:
                return "Cannon";
            default:
                return "";
        }
    }

    public void UpgradePlayer(string upgrade) {
        //player.UpgradePlayer(upgrade, true);
        UpdateUI();
    }

    void UpdateUI() {
        for (int i = 0; i < (int)Upgrade.COUNT; ++i) {
            costTexts[i].text = Player.UPGRADE_COST * Player.UPGRADE_SCALE[player.upgradeRanks[i]] + "g";
        }
    }
}
