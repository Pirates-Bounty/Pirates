using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {
    public float radius;
    public Vector3 position;
    public Player player;
    Renderer rend;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        rend.material.SetFloat("Radius", radius);
        rend.material.SetVector("Center", position);
	}
}
