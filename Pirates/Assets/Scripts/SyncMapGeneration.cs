using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncMapGeneration : NetworkBehaviour
{
    [SyncVar]
    public bool startGen;
    public SyncMapGeneration Instance;
    // Use this for initialization

	void Start () {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    [Command]
    public void CmdChangeStartGen()
    {
        startGen = !startGen;
    }
}
