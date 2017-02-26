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
    public GameObject[] bars = new GameObject[(int)Upgrade.COUNT * Player.MAX_UPGRADES];
    public Button[] buttons = new Button[(int)Upgrade.COUNT];
    public Sprite lockedUpgrade;
    public Sprite[] upgradeSprites = new Sprite[(int)Player.MAX_UPGRADES];

	// SFX
	public AudioClip upgradeS;

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
        if(upgrade == "Ram") {
            player.UpgradePlayer(Upgrade.RAM, true);
			SoundManager.Instance.PlaySFX (upgradeS, 0.5f);
        } else if (upgrade == "Cannon") {
			player.UpgradePlayer(Upgrade.CANNON, true);
			SoundManager.Instance.PlaySFX (upgradeS, 0.5f);
        } else if (upgrade == "Hull") {
			player.UpgradePlayer(Upgrade.HULL, true);
			SoundManager.Instance.PlaySFX (upgradeS, 0.5f);
        } else if (upgrade == "Agility") {
			player.UpgradePlayer(Upgrade.AGILITY, true);
			SoundManager.Instance.PlaySFX (upgradeS, 0.5f);
        } else if (upgrade == "Speed") {
			player.UpgradePlayer(Upgrade.SPEED, true);
			SoundManager.Instance.PlaySFX (upgradeS, 0.5f);
        }
        UpdateUI();
    }

    public bool IsUpgradabale(Upgrade upgrade) {
        return player.upgradeRanks[(int)upgrade] < Player.MAX_UPGRADES && player.resources >= Player.UPGRADE_COST * Player.UPGRADE_SCALE[player.upgradeRanks[(int)upgrade]];
    }

    public void UpdateUI() {
        for (int i = 0; i < (int)Upgrade.COUNT; ++i) {
            if(player.upgradeRanks[i] < Player.MAX_UPGRADES) {
				costTexts[i].text = (Player.UPGRADE_COST * Player.UPGRADE_SCALE[player.upgradeRanks[i]]) + "";
            } else {
                costTexts[i].text = "Sold Out";
            }
            
            buttons[i].interactable = IsUpgradabale((Upgrade)i);
        }
        for (int i = 0; i < bars.Length; i += Player.MAX_UPGRADES) {
            for(int j = 0; j < Player.MAX_UPGRADES; ++j) {
                if(player.upgradeRanks[i / Player.MAX_UPGRADES] > j) {
                    bars[i + j].GetComponent<Image>().sprite = upgradeSprites[j];
                } else {
                    bars[i + j].GetComponent<Image>().sprite = lockedUpgrade;
                }
            }
        }
    }
}
