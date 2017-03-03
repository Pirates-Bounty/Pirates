using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour
{
    public GameObject startText;
    private GameObject activeObject;
    private int children;
    private int counter = 0;
    private bool startTut = false;
    // Use this for initialization
    void Start()
    {
        children = transform.childCount;
        if (Navigator.Tutorial == false)
        {
            startText.SetActive(false);
            gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return) && !startTut)
        {
            startTut = true;
            startText.SetActive(false);
            StartCoroutine(switchActive(5));

        }
    }


    IEnumerator switchActive(int time)
    {
        activeObject = transform.GetChild(counter).gameObject;
        activeObject.SetActive(true);
        yield return new WaitForSeconds(time);
        activeObject.SetActive(false);
        counter++;
        if (counter < children)
        {
            StartCoroutine(switchActive(time));
        }
    }
}
