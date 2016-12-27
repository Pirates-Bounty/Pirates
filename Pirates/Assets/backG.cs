using UnityEngine;
using System.Collections;
public class backG : MonoBehaviour {

    private float tileHeight;
    private float tileWidth;
    public float xSize;
    public float ySize;
    public GameObject mapG;
    public Material waterMat;
    private Renderer rend;
    private float oldX;
    private float oldY;
      
    // Use this for initialization
    void Start () {
        tileHeight = mapG.GetComponent<MapGenerator>().height;
        tileWidth = mapG.GetComponent<MapGenerator>().width;

        transform.localScale = new Vector3(tileWidth, tileHeight, 1);

        rend = GetComponent<Renderer>();
        rend.material.mainTextureScale = new Vector2(xSize, ySize);
    }
	
	// Update is called once per frame
	void Update () {
        if (xSize != oldX || ySize != oldY)
        {
            oldX = xSize;
            oldY = ySize;
            rend.material.mainTextureScale = new Vector2(xSize, ySize);
            
        }
       
    }
}
