using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyTopPanel : MonoBehaviour
    {
        public bool isInGame = false;
        

        protected bool isDisplayed = true;
        protected Image panelImage;

        void Start()
        {
            panelImage = GetComponent<Image>();
        }


        void Update()
        {
            if (!isInGame) { 
                ToggleInGameVisibility(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleInGameVisibility(!isDisplayed);

            }

        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }


        public void ToggleInGameVisibility(bool visible)
        {
            if(gameObject.name != "InGameMenu" || GameObject.FindGameObjectWithTag("UpgradePanel") != null)
            {
                return;
            }
            isDisplayed = visible;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}