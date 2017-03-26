using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PirateHook : LobbyHook {
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		LobbyPlayer playerDetails = lobbyPlayer.GetComponent<LobbyPlayer>();
		Player player = gamePlayer.GetComponent<Player>();

		player.playerColor = playerDetails.playerColor;
		player.playerName = playerDetails.playerName;
	}
}
