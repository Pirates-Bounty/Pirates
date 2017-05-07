using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    public class LobbyTopPanel : NetworkBehaviour
    {
        public bool isInGame = false;
        private LobbyManager lm;
        [SyncVar]
        public int numberPlayers = 0;

        public MapGenerator mg;

        protected bool isDisplayed = true;
        protected Image panelImage;

        void Start()
        {
            lm = transform.parent.GetComponent<LobbyManager>();
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

        public void OnExitButtonPressed()
        {
            if (isServer)
            {
                numberPlayers = 0;
                mg.numPlayers = numberPlayers;
                lm.ResetGame();
                return;
            }
            if (isLocalPlayer)
            {
                numberPlayers--;
                mg.numPlayers = numberPlayers;
                lm.ResetGame();
            }

        }



    }
}