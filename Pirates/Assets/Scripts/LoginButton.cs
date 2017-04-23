
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginButton : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("Menu");
    }
}

