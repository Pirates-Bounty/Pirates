using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {

    public GameObject backG;
    public GameObject activeObject;
    public int children;
    public int counter;
    
    // Use this for initialization
    void Start()
    {
        children = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Enter) && counter > 10)
        //{
        //    transform.GetChild(children).SetActive(true);
        //    activeObject.SetActive(false);
        //    nextObject.SetActive(true);
        //}
        //else if(Input.GetKeyDown(KeyCode.Enter))
        //{
        //    counter++;
        //}
    }
}
