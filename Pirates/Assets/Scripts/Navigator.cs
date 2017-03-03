using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour {
    // singleton
    public static Navigator Instance;
    public static bool Tutorial = false;

    void Start()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update() {
        
    }




    public void LoadLevel(string action) {
		// Loads the scene based on the given string, or exits the game if the string is Quit
        SoundManager.Instance.StopAllBGM();
        if (action == "Quit") {
            Application.Quit();
        } else {
            SceneManager.LoadScene(action);
        }
    }


}
