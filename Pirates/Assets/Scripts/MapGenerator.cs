using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public int width;
    public int height;
    public float frequency;
    public Sprite[] sprites;
    public Color[] colors;
    public int seed;
    public string[] tileNames;
    private Transform canvas;
    public int[,] map;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas").transform;
        Generate();
        GenerateGameObjects();


    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.R))
        {
            DeleteChildren();
            Generate();
            GenerateGameObjects();
        }

	
	}

    void DeleteChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public void Generate()
    {

        Random.InitState(seed);
        float xOffset = Random.Range(-100000, 100000);
        float yOffset = Random.Range(-100000, 100000);


        map = new int[width, height];

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                //Make this less random
                float x = (float)i * frequency / 1000f;
                float y = (float)j * frequency / 1000f;
                float noise = Mathf.PerlinNoise(x + xOffset, y + yOffset);

                if (noise < .6f)
                {
                    map[i, j] = (int)TileType.WATER;
                }

                else if (noise < .7f)
                {
                    map[i, j] = (int)TileType.SAND;
                }


                else if (noise >= .7f)
                {
                    map[i, j] = (int)TileType.GRASS;
                }
            }
        }
    }





    void GenerateGameObjects()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tilePos = new Vector2(i, j);

                int id = map[i, j];
                GameObject Tile = new GameObject(tileNames[id]);
               SpriteRenderer sR = Tile.AddComponent<SpriteRenderer>();
                sR.sprite = sprites[id];
                sR.color = colors[id];
                Tile.transform.position = tilePos;
                Tile.transform.parent = transform;



                switch (map[i, j])
                {
                    case (int)TileType.WATER:
                        //Change Sprite

                        //Move parts out, only have switch for gameObject
                        break;


                    case (int)TileType.GRASS:
                        Tile.AddComponent<BoxCollider2D>();
                        break;


                    case (int)TileType.SAND:
                        Tile.AddComponent<BoxCollider2D>();
                        break;
                    default:
                        
                        break;
                }
                
            }
        }
    }
}
