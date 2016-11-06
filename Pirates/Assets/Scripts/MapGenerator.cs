using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public int width;
    public int height;
    public float frequency;
    //public float amplitude;
    public Sprite[] sprites;
    public Color[] colors;
    public int seed;
    public string[] tileNames;
    public int octaves = 3;
    public int[,] map;
    private Transform canvas;


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

                //Mathf.PerlinNoise(x + xOffset, y + yOffset);
                float noise = PerlinFractal(new Vector2(i+xOffset,j+yOffset), octaves, frequency/1000.0f);
                
                if (noise < .6f)
                {
                    
                    map[i, j] = (int)TileType.GRASS;
                }

                else if (noise < .65f)
                {
                    map[i, j] = (int)TileType.SAND;
                }


                else if (noise >= .65f)
                {
                    
                    map[i, j] = (int)TileType.WATER;
                }
            }
        }
    }



    public static float PerlinFractal(Vector2 v, int octaves, float frequency, float persistence = 0.5f, float lacunarity = 2.0f)
    {
        float total = 0.0f;
        float amplitude = 1.0f;
        float maxAmp = 0.0f; // keeps track of max possible noise

        for (int i = 0; i < octaves; ++i)
        {
            float noise = Mathf.PerlinNoise(v.x * frequency, v.y * frequency);
            noise = noise * 2 - 1;
            noise = 1.0f - Mathf.Abs(noise);
            total += noise * amplitude;
            maxAmp += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        //Debug.Log(total);
        return total/maxAmp;
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
