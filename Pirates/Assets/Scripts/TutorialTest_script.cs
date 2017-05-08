using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTest_script : MonoBehaviour {
    //---initialized from inspector---code from LobbyManager.cs
    public GameObject mapGen; //going to be DontDestroyOnLoad at lobby
    //value: "MapGenTopLevel"GO in Hierarchy
    public GameObject gameSetUp;
    //value: "GameSetUp"Prefab in Assets/Prefabs/GameComponents/GameSetUp
    public GameObject inGameMenuPanel;
    //value: "InGameMenu"GO in Hierarchy

    // Use this for initialization
    void Start () {
        // the process starts from public LobbyManager.cs -> override void OnLobbyServerPlayersReady() -> StartCoroutine(ServerCountdownCoroutine());
        // the code below is taken from LobbyManager.cs -> public IEnumerator ServerCountdownCoroutine()

        MapGenerator mg = mapGen.GetComponentInChildren<MapGenerator>();
        //value: "MapGen" in Hierarchy
        mg.localNumberPlayers = 1; // edited for tutorial
        mg.CmdReGenerate(); //i wanna know if any objects initialized here is set from the inspector/lobby somewhere...




        Instantiate(gameSetUp, transform.position, Quaternion.identity);

        //just activate inGameMenuPanel, with inspector null checker
        if (inGameMenuPanel != null)
        {
            Debug.Log("Valid in game menu panel");
            inGameMenuPanel.SetActive(true);
        }
        else Debug.Log("null ingamemenupanel");

        mapGen.SetActive(true); //just activate mapGen




        //everytime server change scene, set all clients to not ready?
        //    NetworkServer.SetAllClientsNotReady();
        //then... not important?
        //ServerChangeScene(playScene);

        //List of all override functions in LobbyManager.cs
        //    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        //    public override void ServerChangeScene(string sceneName)
        //    public override void OnStartHost()
        //    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        //    public override void OnDestroyMatch(bool success, string extendedInfo)
        //  * public override void OnLobbyServerPlayersReady()
        //    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        //    public override void OnLobbyServerDisconnect(NetworkConnection conn)
        //    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        //    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        //    public override void OnClientSceneChanged(NetworkConnection conn)
        //    public override void OnClientConnect(NetworkConnection conn)
        //    public override void OnClientDisconnect(NetworkConnection conn)
        //    public override void OnClientError(NetworkConnection conn, int errorCode)
        // * = understood

    }

    // Update is called once per frame
    void Update () {
		
	}


}
