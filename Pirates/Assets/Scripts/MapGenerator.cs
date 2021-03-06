﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


public class MapGenerator : NetworkBehaviour {

    [SyncVar]
    public int width = 6;

    [SyncVar]
    public int height = 6;
    public float frequency;
    public float borderRadius = 0.75f;

    [SyncVar]
    public float landFreq;

    [SyncVar]
    public int seed = 200;

    //[HideInInspector]
    //public float centerWeight = 7;

    public static bool gameStart = false;
    //public float amplitude;
    public Sprite[] sprites;
    public Sprite[] plantSprites;
    public GameObject resourcePrefab;
    public GameObject mapPanel;
    private GameObject plane;
    private GameObject quad;
    private List<Vector2> waterPos = new List<Vector2>();
    public Sprite[] plants;
    public string[] tileNames;

    [HideInInspector]
    public int octaves = 3;

    [HideInInspector]
    public int[,] map;
    public int[,] bitmaskedMap;

    public int quadWidth;
    public int quadHeight;

    [SyncVar]
    public float maxResources;


    private Tilemap tMap;

    private bool addResources = false;
    public MapGenerator Instance;
    public float tileSize;
    public Slider landSlider;
    public Slider widthSlider;
    public Slider resourceSlider;
    public InputField seedInputField;
    public Material waterMat;
    public Material boundaryMat;
    public RawImage mapPic;
    private BoundaryGenerator bg;
    private Texture2D minimapTexture;
    private GameObject minimap;
    public GameObject inGameMenu;
    public LobbyTopPanel inGameMenuPanel;
    public LobbyManager lm;

    private int resourceMult = 1;

    [SyncVar]
    public int numPlayers = 0;
    public int localNumberPlayers = 0;

    void Start() {
        if (!Instance) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        tMap = GameObject.Find("Tile Map").GetComponentInChildren<Tilemap>();
        bg = GetComponent<BoundaryGenerator>();
        Generate();

        //GenerateGameObjects();
        if (isServer)
        {
            numPlayers = localNumberPlayers;
        }

        maxResources = (.4f * resourceMult) * width;
		tileSize = tMap.cellSize.x;
    }


    [Command]
    public void CmdChangeSeed(int newSeed) {
        seed = newSeed;
    }

    [Command]
    public void CmdChangeLandFreq(float newLandFreq) {
        landFreq = newLandFreq;
    }

    [Command]
    public void CmdChangeWidth(int newWidth) {
        width = newWidth;
        height = newWidth;
    }

    public void WidthChange() {
        width = (int)(widthSlider.value * 1000);
        height = (int)(widthSlider.value * 1000);
        CmdChangeWidth(width);
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
        CmdChangeLandFreq(landFreq);
    }

    public void MaxResourceChange() {
        maxResources = (int)((resourceSlider.value * resourceMult) * (width * 1.5));
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
        CmdChangeSeed(seed);
    }


    public void LobbyButton() {
        mapPanel.SetActive(false);
        //CmdReGenerate();
    }


    // Use this for initialization

    // Update is called once per frame
    void Update() {
        if (!minimap) {
            minimap = GameObject.Find("MainCanvas/UI/Minimap");
            return;
        }
        minimap.GetComponent<RawImage>().texture = minimapTexture;
        if (isServer)
        {
            if(localNumberPlayers != numPlayers)
            {
                numPlayers = localNumberPlayers;
                if(inGameMenuPanel != null)
                {
                    inGameMenuPanel.numberPlayers = numPlayers;
                }
                else
                {
                    inGameMenu = GameObject.Find("InGameMenu");
                    if (inGameMenu != null)
                    {
                        inGameMenuPanel = inGameMenu.GetComponent<LobbyTopPanel>();
                    }
                }

            }
        }
    }



    void DeleteChildren() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ClearMap()
    {
        map = new int[width, height];
        waterPos = new List<Vector2>();
        //tMap.ClearAllTiles();

        int cCount = transform.childCount;
        for(int i = 0; i < cCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void GeneratePreviewTexture() {
        Texture2D tex = GenerateTexture();
        mapPic.texture = tex;
    }

    public Texture2D GenerateTexture() {
        Texture2D tex = new Texture2D(width, height);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (IsInCircle(x - width / 2, y - height / 2, width / 2)) {
                    if (map[x, y] == (int)TileType.WATER) {
                        tex.SetPixel(x, y, Color.blue);
                    } else if (map[x, y] == (int)TileType.GRASS) {
                        tex.SetPixel(x, y, Color.green);
                    } else if (map[x, y] == (int)TileType.SAND) {
                        tex.SetPixel(x, y, Color.yellow);
                    }
                } else {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
        tex.Apply();
        
        return tex;
    }
    public static bool IsInCircle(int x, int y, int radius) {
        return x * x + y * y < radius * radius;
    }
    public void Generate() {

        ClearMap();
        if(tMap != null)
        {
            tMap.ClearAllTiles();
        }
        Random.InitState(seed);
        float xOffset = Random.Range(-100000, 100000);
        float yOffset = Random.Range(-100000, 100000);

        for (int i = 0; i < width; ++i) {
            for (int j = 0; j < height; ++j) {
                if (IsInCircle(i - width / 2, j - height / 2, width / 2)) {
                    if (i == 0 || j == 0 || i == width - 1 || j == height - 1) {
                        map[i, j] = (int)TileType.WATER;
                        continue;
                    }
                    float noise = PerlinFractal(new Vector2(i + xOffset, j + yOffset), octaves, frequency / 1000.0f);
                    // change the noise so that it is also weighted based on the euclidean distance from the center of the map
                    // this way, there will be a larger island in the middle of the map
                    // comment this line to go back to the old generation
                    //noise *= centerWeight * noise * Mathf.Pow((Mathf.Pow(i - width / 2, 2) + Mathf.Pow(j - height / 2, 2)), 0.5f) / (width / 2 + height / 2);
                    if (noise <= landFreq)
                    {
                        if (noise > Random.Range(0, .5f))
                        {
                            map[i, j] = (int)TileType.TREE;
                        }
                        else
                        {
                            map[i, j] = (int)TileType.GRASS;
                        }
                    }
                    else if (noise > landFreq) {
                        //Debug.Log("water");
                        map[i, j] = (int)TileType.WATER;
                        waterPos.Add(new Vector2(i, j));
                    }
                }
            }
        }
        GenerateBitmaskedMap();
    }

    private void GenerateBitmaskedMap() {
        bitmaskedMap = new int[width, height];
        for (int j = 0; j < height; ++j) {
            for (int i = 0; i < width; ++i) {
                int count = 0;
                bitmaskedMap[i, j] = 0;
                for (int y = -1; y <= 1; ++y) {
                    for (int x = -1; x <= 1; ++x) {
                        if (x == 0 && y == 0) {
                            continue;
                        }
                        if (IsInBounds(i + x, j - y) && map[i + x, j - y] != (int)TileType.WATER && map[i, j] != (int)TileType.WATER) {
                            bitmaskedMap[i, j] += (int)Mathf.Pow(2, count);
                        }
                        count++;
                    }
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

    private bool IsInBounds(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void GenerateGameObjects() {
        // Background tiles and boundary
        bg.Generate(width * borderRadius * tileSize);
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y, plane.transform.position.z + 5);
        plane.transform.Rotate(new Vector3(90, 0, 180));
        plane.GetComponent<MeshRenderer>().material = waterMat;
        plane.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(width / 7.5f, height / 7.5f);
        plane.transform.localScale = new Vector3(width * tileSize / 5f, 1, height * tileSize / 5f);
        plane.transform.parent = transform;

        // boundary mesh
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.localScale = new Vector3(width * 2f * tileSize, width * 2f * tileSize, 1);
        quad.GetComponent<MeshRenderer>().material = boundaryMat;
        quad.gameObject.layer = 12;
        MeshCollider mc = quad.GetComponent<MeshCollider>();
        mc.convex = true;
        quad.transform.parent = transform;
        Rigidbody rb = quad.AddComponent<Rigidbody>();
        rb.useGravity = false;
        quad.gameObject.isStatic = true;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;



        // minimap
        minimapTexture = GenerateTexture();
       
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int id = bitmaskedMap[i, j];
                switch ((TileType)map[i, j]) {
                    case TileType.WATER:
                        //GameObject waterObj = new GameObject();
                        //waterObj.tag = "water";
                        //BoxCollider2D coll = waterObj.AddComponent<BoxCollider2D>();
                        //coll.size = new Vector2(tMap.cellSize.x, tMap.cellSize.y);
                        //Instantiate(waterObj, new Vector3(i - width / 2 * tMap.cellSize.x, j - height / 2 * tMap.cellSize.y, 0), Quaternion.identity);
                        Sprite ws = Resources.Load<Sprite>("Art/Sprites/Tiles/Bitmasked Tiles/" + 1);
      
                        AddTileToMap(new Vector3Int(i - width / 2, j - height / 2, 0), null, null);
                        break;
                    case TileType.GRASS:
					    Sprite s = Resources.Load<Sprite>("Art/Sprites/Tiles/Bitmasked Tiles/"+id);
					    AddTileToMap(new Vector3Int(i - width/2, j - height/2, 0), s, null);
                            break;
                    case TileType.TREE:
                        //GameObject tree = new GameObject();
                        //tree.AddComponent<SpriteRenderer>().sortingOrder = 5;
                        Sprite sp = Resources.Load<Sprite>("Art/Sprites/Tiles/Bitmasked Tiles/" + id);
                        AddTileToMap(new Vector3Int(i - width / 2, j - height / 2, 0), sp, null);
                        //AddTileToMap(new Vector3Int(i - width / 2, j - height / 2, 0), ts, null);
                        GameObject plant = new GameObject();
                        SpriteRenderer pRender = plant.AddComponent<SpriteRenderer>();

                        int r = Random.Range(1, 5);
                        for(int k = 0; k < r; k++)
                        {
                            pRender.sprite = plants[Random.Range(0, plants.Length)];
                            GameObject p = Instantiate(plant, new Vector3(((i - width / 2) * tMap.cellSize.x) + Random.Range(5, 20),((j - height / 2) * tMap.cellSize.y) + Random.Range(5, 20), 0), Quaternion.identity);
                            p.transform.parent = gameObject.transform;
                            p.transform.localScale *= Random.Range(.5f, 1.5f);
                        }

                        break;
                }

            }
        }

    }

    public Vector2 GetRandWaterTile(int size) {
        int radius = width / 2 - 1;
        Random.InitState(System.DateTime.Now.Millisecond);
        Vector2 RandPos = waterPos[Random.Range(0, waterPos.Count)];
        Debug.Log(RandPos);
        int xRand = (int)RandPos.x;
        int yRand = (int)RandPos.y;
        int tile = map[xRand, yRand];
        Vector2 tilePos = new Vector2((xRand - width / 2) * tMap.cellSize.x, (yRand - height / 2) * tMap.cellSize.y);
        bool Occupied = true;
        while (!Occupied) {
            RandPos = waterPos[Random.Range(0, waterPos.Count)];
            xRand = (int)RandPos.x;
            yRand = (int)RandPos.y;
            tile = map[xRand, yRand];
            tilePos = new Vector2((xRand - width / 2) * tMap.cellSize.x, (yRand - height / 2) * tMap.cellSize.y);
            //randTilePos = new Vector2(tilePos.x + Random.Range(-tileSize / 2, tileSize / 2), tilePos.y + Random.Range(-tileSize / 2, tileSize / 2));
            Collider2D collision = Physics2D.OverlapArea(new Vector2(tilePos.x - size, tilePos.y - size), new Vector2(tilePos.x + size, tilePos.y + size));
            if(collision != null && (collision.tag != "Resource" && collision.tag != "Player"))
            {
                Occupied = false;
            }
            else if(collision == null)
            {
                Occupied = false;
            }
        }

        return tilePos;
    }

    public Vector2 GetRandLocAwayFromLand(int size) {
        Vector2 returnLoc = Vector2.zero;
        bool end = false;
        while (!end)
        {
            returnLoc = GetRandWaterTile(size);
            end = CheckNeighborsForWater(size, returnLoc);
        }

        return returnLoc;
    }

    public bool CheckNeighborsForWater(int size, Vector2 loc) {
        for (int i = -size / 2; i < size / 2; i++) {
            for (int j = -size / 2; j < size / 2; j++) {
                Vector2 tileLoc = LocToMap(loc);
                if (tileLoc.x + i < width && tileLoc.x + i >= 0 && tileLoc.y + j < height && tileLoc.y + j >= 0) {

                    int Tile = map[(int)tileLoc.x + i, (int)tileLoc.y + j];
                    if (((TileType)Tile != TileType.WATER)) {
                        return false;
                    }


                } else {
                    return false;
                }


            }
        }
        return true;
    }

    public Vector2 LocToMap(Vector2 loc) {
        return new Vector2((loc.x / tileSize) + width / 2, (loc.y / tileSize) + height / 2);
    }


    public void AddTileToMap(Vector3Int pos, Sprite sp, GameObject g) {
        Tile tile = Tile.CreateInstance<Tile>();
        tile.sprite = sp;
        if (g) {
            tile.gameObject = g;
        }
        tMap.SetTile(pos, tile);
    }


    public void OnExitButtonPressed()
    {
        if (isServer)
        {
            localNumberPlayers = 0;
            inGameMenuPanel.numberPlayers = 0;
            Debug.Log("Server press exit");
            lm.ResetGame();
            return;
        }
        localNumberPlayers--;
        inGameMenuPanel.numberPlayers--;
        lm.ResetGame();

    }


}
