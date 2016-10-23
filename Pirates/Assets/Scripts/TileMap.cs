using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TileType
{
    WATER,
    GRASS,
    SAND,
    TREE,
    ROCK,
    CLIFFS
}


public class TileMap {
    //public GameObject[] tiles;
    private int width;
    private int height;
    public int[,] map;
    public TileMap(int width, int height, float frequency)
    {
        this.width = width;
        this.height = height;
        map = new int[width, height];
    }
	
    public void Generate(float frequency)
    {



        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                //Make this less random
                float x = (float)i * frequency/ 1000f;
                float y = (float)j * frequency/ 1000f;
                float noise = Mathf.PerlinNoise(x, y);

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

	// Update is called once per frame
	void Update () {
	
	}
}
