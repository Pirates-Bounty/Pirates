using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class MapGenerator : NetworkBehaviour {
    public int width;
    public int height;
    public float frequency;
    public float centerWeight;
    //public float amplitude;
    public Sprite[] sprites;
    public GameObject resourcePrefab;
    // no longer needed because we have actual art instead of placeholders
    //public Color[] colors;
    public int seed;
    public string[] tileNames;
    public int octaves = 3;
    public int[,] map;
    public int quadWidth;
    public int quadHeight;
    private int maxResources;
    private Transform canvas;
    private Camera minMap;
    private Sprite minMapBorder;
    private bool addResources = false;

    

    void Awake()
    {

        maxResources = Mathf.RoundToInt((width + height) / 50);
        Generate();
        GenerateGameObjects();
        minMap = GameObject.Find("MinCam").GetComponent<Camera>();
        minMap.orthographicSize = width / 2;
        canvas = GameObject.Find("Canvas").transform;
        minMapBorder = Resources.Load<Sprite>("Art/Sprites/UI Updated 11-19-16/UI Main Menu Mini Map Border");
        UI.CreatePanel("minMap Border", minMapBorder, Color.white, canvas, Vector3.zero, new Vector2(0.8f, 0.0f), new Vector2(1.0f, 0.4f));

        int numPlayers = LobbyManager.numPlayers;

        //Circle radius and degree calculation for spawning spawn points
        int rad = (width / 2) - 5;
		float deg = 90;
		if (numPlayers != 0)
		{
			deg = 360 / numPlayers;
		}


        //Loop through the players and spawn a spawn point for each player along the circle
        for (int i = 1; i < numPlayers + 1; i++)
        {
            bool spawnable = false;
            GameObject Spawner = new GameObject();
            Spawner.AddComponent<NetworkStartPosition>();
            int x = (int)(rad * Mathf.Cos(deg * i));
            int y = (int)(rad * Mathf.Sin(deg * i));
            //Checks to see if a good spot to spawn the spawnPoints
            while (spawnable)
            {
                bool resetLoop = false;
                for (int j = x - quadWidth / 2; j < x + quadWidth / 2; j++)
                {
                    for (int k = y - quadHeight / 2; k < y + quadHeight / 2; k++)
                    {

                        if (map[j, k] != (int)TileType.WATER)
                        {
                            x -= x / Mathf.Abs(x);
                            y -= y / Mathf.Abs(x);
                            resetLoop = true;
                            break;
                        }
                        if (resetLoop)
                        {
                            break;
                        }
                    }
                }
                if (!resetLoop)
                {
                    spawnable = true;
                }
            }
            Spawner.transform.position = new Vector2(x, y);
            Vector3 dir = -Spawner.transform.position;
            dir = dir.normalized;
            Spawner.transform.up = dir;
            
            //Spawner.transform.LookAt(new Vector3(transform.position.x, transform.position.z, 0));
        }

    }
    void Start()
    {

        Random.InitState(System.DateTime.Now.Millisecond);
        for (int i = 0; i < maxResources; i++)
        {

            CmdSpawnResource();
        }
    }

    // Use this for initialization

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.R))
        {
            DeleteChildren();
            Generate();
            GenerateGameObjects();
        }




	
	}

    [Command]
    void CmdSpawnResource()
    {
        if (!isServer)
        {
            return;
        }
        ClientScene.RegisterPrefab(resourcePrefab);
        GameObject instantiatedResource = Instantiate(resourcePrefab, new Vector2(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2)), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(instantiatedResource);
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
                // change the noise so that it is also weighted based on the euclidean distance from the center of the map
                // this way, there will be a larger island in the middle of the map
                // comment this line to go back to the old generation
                noise *= centerWeight * noise * Mathf.Pow((Mathf.Pow(i - width/2, 2) + Mathf.Pow(j - height/2, 2)), 0.5f)/(width/2 + height/2);
                if (noise < .45f)
                {
                    
                    map[i, j] = (int)TileType.GRASS;
                }

                else if (noise < .5f)
                {
                    map[i, j] = (int)TileType.SAND;
                }


                else if (noise >= .5f)
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
                Vector2 tilePos = new Vector2(i - width / 2, j - height / 2);

                int id = map[i, j];

                //Don't want to spawn water tiles if we don't need to
                //We already have seperate water tile background so only spawn water tiles that create
                //boundary for the map
                if (map[i, j] != (int)TileType.WATER || (i == 0 || j == 0 || i == width - 1 || j == height - 1))
                {
                    GameObject Tile = new GameObject(tileNames[id]);

                    //Don't add a sprite renderer to boundary water tiles
                    if(map[i,j] != (int)TileType.WATER)
                    {
                        SpriteRenderer sR = Tile.AddComponent<SpriteRenderer>();
                        sR.sprite = sprites[id];
                    }
                    //sR.color = colors[id];
                    Tile.transform.position = tilePos;
                    Tile.transform.parent = transform;




                    switch (map[i, j])
                    {
                        case (int)TileType.WATER:
                            {
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
}
