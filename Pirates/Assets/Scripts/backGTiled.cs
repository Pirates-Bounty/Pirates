using UnityEngine;
using System.Collections;
public class backGTiled : MonoBehaviour {

    private int Height;
    private int Width;
    public GameObject mapG;
    public GameObject tile;
    public int offset;
    public int multWidth = 1;
    public int multHeight = 1;
    private int tWidth;
    private int tHeight;
      
    // Use this for initialization
    void Start () {
        Height = mapG.GetComponent<MapGenerator>().height;
        Width = mapG.GetComponent<MapGenerator>().width;

        tWidth = (Width / multWidth);
        tHeight = (Height / multHeight);

        for (int i = -Width/2 - offset; i < Width/2 + offset; i+=tWidth/2)
        {
            for (int j = -Height / 2 - offset; j < Height / 2 + offset; j += tHeight / 2)
            {
                Vector2 tilePos = new Vector2(i , j);


                GameObject t = Instantiate(tile, tilePos, Quaternion.identity) as GameObject;
                t.transform.parent = transform;
                t.transform.localScale = new Vector3(tWidth/2, tHeight/2, 1);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
       
    }
}
