using UnityEngine;
using System.Collections;
// using System.Net.Http;

public class Login : MonoBehaviour {

  	// private static readonly HttpClient client = new HttpClient();

	private GameObject playButton;
	private GameObject loginButton;
    private Transform canvas;
    private Font font;
    private Sprite sprite;
    private Sprite highlightedSprite;
    private Color color = Color.black;
    private int fontSize = 20;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find("Canvas").transform;

		playButton = UI.CreateButton("Skip", "Skip", font, color, fontSize, canvas, 
		Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/PlayButtonUnClicked"), Resources.Load<Sprite>("Art/Sprites/MARCH 21, 2017/PlayButtonClicked"),
        Vector3.zero, new Vector2(0.05f, 0.05f), new Vector2(0.325f, 0.2f),
        delegate {
            Application.LoadLevel("Menu");
        } );

        loginButton = GameObject.Find("LoginButton");
	}

	// Update is called once per frame
	void Update () {
	}

	// public async void createUser() {
	//     // Values that we will send to the server. AKA Request Data
	//     // Replace foo and bar with values that the user inputted
	//     var values = new Dictionary<string, string>
	//     {
	//        { "username", "foo" },
	//        { "password", "bar" }
	//     };

	//     var content = new FormUrlEncodedContent(values);

	//     // Make POST request
	//     var response = await client.PostAsync("http://pirates.chrisbanh.com/user/create", content);

	//     var responseString = await response.Content.ReadAsStringAsync();

	//     // not sure what to do from here
 //  }
}
