using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Game : MonoBehaviour {
    public static Game Instance;
    public Player player1;
    public Player player2;
    public Player[] players;
	//private AudioSource lobbyYo;


    const int MAX_PLAYERS = 2;

    void Awake()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
			//lobbyYo = GetComponent<AudioSource>();
            Instance = this;
            players = new Player[MAX_PLAYERS];
            players[0] = player1;
            players[1] = player2;
            //// Player 1
            //player1.up = KeyCode.W;
            //player1.down = KeyCode.S;
            //player1.right = KeyCode.D;
            //player1.left = KeyCode.A;
            //player1.fire = KeyCode.Q;
            //player1.menu = KeyCode.Escape;
            ////Player 2
            //player2.up = KeyCode.I;
            //player2.down = KeyCode.K;
            //player2.right = KeyCode.L;
            //player2.left = KeyCode.J;
            //player2.fire = KeyCode.U;
            //player2.menu = KeyCode.Delete;

        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool GameOver()
    {
        // returns if the game is over or not
        return players.Length <= 1;

    }
	
	// Update is called once per frame
	void Update () {
		//if (SceneManager.GetActiveScene().name != "Lobby") {
		//	lobbyYo.Stop();
		//} else {
		//	lobbyYo.UnPause(); // holy guacamole!
		//}
    }

}













// ayy its ur boy