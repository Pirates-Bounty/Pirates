using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityEngine.UI;


public class MapGenerator : NetworkBehaviour {
    public int width;
    public int height;
    public float frequency;

    [SyncVar]
    public float landFreq;

    [SyncVar]
    public int seed = 200;

    [HideInInspector]
    public float centerWeight = 7;

    public static bool gameStart = false;
    //public float amplitude;
    public Sprite[] sprites;
    public Sprite[] plantSprites;
    public GameObject resourcePrefab;
    public GameObject mapPanel;
    private GameObject plane;
    public string[] tileNames;

    [HideInInspector]
    public int octaves = 3;

    [HideInInspector]
    public int[,] map;

    public int quadWidth;
    public int quadHeight;

    [SyncVar]
    public float maxResources;

    private Transform canvas;
    private Camera minMap;
    private Sprite minMapBorder;
    private bool addResources = false;
    public MapGenerator Instance;
    public Slider landSlider;
    public Slider widthSlider;
    public Slider resourceSlider;
    public InputField seedInputField;
    public Material waterMat;
    public RawImage mapPic;

    void Start() {
        if (!Instance) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        Generate();
        GenerateGameObjects();
        int numPlayers = LobbyManager.numPlayers;
        maxResources = (.2f * 20000) / width ;


    }

    [Command]
    public void CmdChangeSeed(int newSeed) {
        seed = newSeed;
    }

    [Command]
    public void CmdChangeLandFreq(int newLandFreq) {
        landFreq = newLandFreq;
    }

    [Command]
    public void CmdChangeWidth(int newWidth) {
        width = newWidth;
    }

    public void WidthChange() {
        width = (int)(widthSlider.value * 1000);
        height = (int)(widthSlider.value * 1000);
    }

    [Command]
    public void CmdChangeHeight(int newHeight) {
        height = newHeight;
    }

    [Command]
    public void CmdChangeMaxResource(int newResource) {
        maxResources = newResource;
    }

    public void SliderChange() {
        landFreq = landSlider.value;
    }

    public void MaxResourceChange() {
        maxResources = (int)((resourceSlider.value * 20000) / width);
    }

    public void InputSeed() {
        try {
            seed = System.Convert.ToInt32(seedInputField.text);
        } catch {
            seedInputField.text = "200";
        }
    }

    public void SeedChange() {
        seed = System.DateTime.Now.Millisecond;
        seedInputField.text = seed.ToString();
    }


    public void LobbyButton() {
        mapPanel.SetActive(false);
        CmdReGenerate();
    }


    // Use this for initialization

    // Update is called once per frame
    void Update() {
    }



    void DeleteChildren() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void GenerateTexture() {
        Texture2D tex = new Texture2D(width, height);
        mapPic.GetComponent<RawImage>().texture = tex;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (map[x, y] == (int)TileType.WATER) {
                    tex.SetPixel(x, y, Color.blue);
                } else if (map[x, y] == (int)TileType.GRASS) {
                    tex.SetPixel(x, y, Color.green);
                } else if (map[x, y] == (int)TileType.SAND) {
                    tex.SetPixel(x, y, Color.yellow);
                }
            }
        }
        tex.Apply();
    }

    public void Generate() {

        Random.InitState(seed);
        float xOffset = Random.Range(-100000, 100000);
        float yOffset = Random.Range(-100000, 100000);


        map = new int[width, height];

        for (int i = 0; i < width; ++i) {
            for (int j = 0; j < height; ++j) {
                if (i == 0 || j == 0 || i == width - 1 || j == height - 1) {
                    map[i, j] = (int)TileType.WATER;
                    continue;
                }

                //Mathf.PerlinNoise(x + xOffset, y + yOffset);
                float noise = PerlinFractal(new Vector2(i + xOffset, j + yOffset), octaves, frequency / 1000.0f);
                // change the noise so that it is also weighted based on the euclidean distance from the center of the map
                // this way, there will be a larger island in the middle of the map
                // comment this line to go back to the old generation
                noise *= centerWeight * noise * Mathf.Pow((Mathf.Pow(i - width / 2, 2) + Mathf.Pow(j - height / 2, 2)), 0.5f) / (width / 2 + height / 2);
                if (noise < landFreq) {
                    if (noise > Random.Range(0, 40f)) {
                        map[i, j] = (int)TileType.TREE;
                    } else {
                        map[i, j] = (int)TileType.GRASS;
                    }

                } else if (noise < landFreq + .05) {
                    map[i, j] = (int)TileType.SAND;
                } else if (noise >= landFreq + .05) {

                    map[i, j] = (int)TileType.WATER;
                }
            }
        }
    }



    public static float PerlinFractal(Vector2 v, int octaves, float frequency, float persistence = 0.5f, float lacunarity = 2.0f) {
        float total = 0.0f;
        float amplitude = 1.0f;
        float maxAmp = 0.0f; // keeps track of max possible noise

        for (int i = 0; i < octaves; ++i) {
            float noise = Mathf.PerlinNoise(v.x * frequency, v.y * frequency);
            noise = noise * 2 - 1;
            noise = 1.0f - Mathf.Abs(noise);
            total += noise * amplitude;
            maxAmp += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        //Debug.Log(total);
        return total / maxAmp;
    }

    [ClientRpc]
    public void RpcReGenerate() {
        DeleteChildren();
        Generate();
        GenerateGameObjects();
    }

    [Command]
    public void CmdReGenerate() {
        RpcReGenerate();
    }


    void GenerateGameObjects() {
        // Background tiles
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y, plane.transform.position.z + 5);
        plane.transform.Rotate(new Vector3(90, 0, 180));
        plane.GetComponent<MeshRenderer>().material = waterMat;
        plane.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(width / 5, height / 5);
        plane.transform.localScale = new Vector3(width / 10, 1, height / 10);
        plane.transform.parent = transform;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector2 tilePos = new Vector2(i - width / 2, j - height / 2);

                int id = map[i, j];

                //Don't want to spawn water tiles if we don't need to
                //We already have seperate water tile background so only spawn water tiles that create
                //boundary for the map
                if (map[i, j] != (int)TileType.WATER || (i == 0 || j == 0 || i == width - 1 || j == height - 1)) {
                    GameObject Tile = new GameObject(tileNames[id]);

                    //Don't add a sprite renderer to boundary water tiles
                    if (map[i, j] != (int)TileType.WATER) {
                        SpriteRenderer sR = Tile.AddComponent<SpriteRenderer>();
                        sR.sprite = sprites[id];
                        sR.sortingOrder = 0;
                    }
                    //sR.color = colors[id];
                    Tile.transform.position = tilePos;
                    Tile.transform.parent = transform;




                    switch (map[i, j]) {
                        case (int)TileType.WATER: {
                                Tile.AddComponent<BoxCollider2D>();
                                SpriteRenderer sR = Tile.AddComponent<SpriteRenderer>();
                                sR.sortingOrder = 1;
                            }
                            //Change Sprite

                            //Move parts out, only have switch for gameObject
                            break;


                        case (int)TileType.GRASS:
                            Tile.AddComponent<BoxCollider2D>();
                            break;

                        case (int)TileType.TREE:
                            Tile.AddComponent<BoxCollider2D>();
                            GameObject plant = new GameObject();
                            plant.transform.parent = Tile.transform;
                            plant.transform.localPosition = Vector3.zero;
                            SpriteRenderer sP = plant.AddComponent<SpriteRenderer>();
                            sP.sprite = plantSprites[Random.Range(0, plantSprites.Length)];
                            sP.sortingOrder = 1;
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

    public Vector2 GetRandWaterTile() {
        int xRand = Random.Range(0, width);
        int yRand = Random.Range(0, height);
        int tile = map[xRand, yRand];
        Vector2 tilePos = new Vector2(xRand - width / 2, yRand - height / 2);
        while ((TileType)tile != TileType.WATER) {
            xRand = Random.Range(0, width);
            yRand = Random.Range(0, height);
            tile = map[xRand, yRand];
            tilePos = new Vector2(xRand - width / 2, yRand - height / 2);
        }

        return tilePos;
    }
}
