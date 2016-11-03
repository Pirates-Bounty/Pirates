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
    public float perlinMult = 2.5f;
    public float octaves = 3;
    public int[,] map;

    // Use this for initialization
    void Start () {
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
                if(i == 0 || j == 0 || i == width - 1 || j == height - 1) {
                    map[i, j] = (int)TileType.WATER;
                    continue;
                }
                //Make this less random
                float x = (float)i * frequency / 1000f;
                float y = (float)j * frequency / 1000f;
                float noise = Mathf.PerlinNoise(x + xOffset, y + yOffset);



                float amplitude = 1f;
                float range = 1f;
                for (int o = 1; o < octaves/2; o++)
                {
                    
                    x *= perlinMult;
                    y *= perlinMult;
                    //perlinMult-= .1f;
                    amplitude = 0.5f;
                    range += amplitude;
                    noise += Mathf.PerlinNoise(x + xOffset, y + yOffset) * amplitude;
                }
                for (int o = 1; o < octaves / 2; o++)
                {

                    x /= perlinMult;
                    y /= perlinMult;
                    //perlinMult-= .1f;
                    amplitude = 0.5f;
                    range += amplitude;
                    noise += Mathf.PerlinNoise(x + xOffset, y + yOffset) * amplitude;
                }
                noise =  noise / range;



                if (noise < .6f)
                {
                    map[i, j] = (int)TileType.WATER;
                }

                else if (noise < .65f)
                {
                    map[i, j] = (int)TileType.SAND;
                }


                else if (noise >= .65f)
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
                        if (i == 0 || j == 0 || i == width - 1 || j == height - 1) {
                            Tile.AddComponent<BoxCollider2D>();
                        }
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
