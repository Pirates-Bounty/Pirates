using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {

    public GameObject backG;
    public GameObject activeObject;
    public int children;
    public int counter = 0;
    
    // Use this for initialization
    void Start()
    {
        children = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Return) && counter != children)
        {
			StartCoroutine (switchActive (5));

        }
    }


	IEnumerator switchActive(int time)
	{
		activeObject = transform.GetChild(counter).gameObject;
		activeObject.SetActive (true);
		yield return new WaitForSeconds (time);
		activeObject.SetActive (false);
		counter++;
		if (counter < children) {
			StartCoroutine(switchActive(time));
		}
	}
}
