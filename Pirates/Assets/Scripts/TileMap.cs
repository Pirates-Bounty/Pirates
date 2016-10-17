using UnityEngine;
using System.Collections;

public class TileMap {
    public GameObject[] tiles;
    private int width;
    private int height;
    private int uniqueElements;
    public int[,] map;
    public TileMap(int width, int height,int uniqueElements)
    {
        this.width = width;
        this.height = height;
        this.uniqueElements = uniqueElements;
        map = new int[width, height];
    }
	
    public void Generate()
    {
        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                map[i, j] = Random.Range(0, uniqueElements);
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
