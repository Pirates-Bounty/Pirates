using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {

    public bool spawned = false;
    
    // Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        Debug.Log("spawn Point");
    }
	
	// Update is called once per frame
	void Update () {
        if (spawned)
        {
            spawned = false;
            //foreach (GameObject g in GameObject.FindGameObjectsWithTag("spawner"))
            //{
            //    if (g.transform.position == transform.position)
            //    {
            //        Destroy(gameObject);
            //    }
            //}
        }

    }
}
