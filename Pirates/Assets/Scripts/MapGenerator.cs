using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public int width;
    public int height;
    public Sprite[] sprites;
    public Color[] colors;
    private Transform canvas;
    private TileMap tileMap;
    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas").transform;
        tileMap = new TileMap(width, height,sprites.Length);
        tileMap.Generate();
        Render();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Render()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                UI.CreatePanel("(" + i + ", " + j + ")", sprites[tileMap.map[i, j]], colors[tileMap.map[i, j]], canvas, Vector3.zero,
                   new Vector2((1.0f / width) * i, (1.0f / height) * j), new Vector2((1.0f / width) * (i + 1), (1.0f / height) * (j + 1)));
            }
        }
    }
}
