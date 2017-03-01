using UnityEngine;
using System.Collections;

public class Tooltip : MonoBehaviour {
    GameObject tooltip;
	// Use this for initialization
	void Start () {
        tooltip = transform.Find("Tooltip").gameObject;
        tooltip.SetActive(false);
	}
	
    public void Show() {
        tooltip.SetActive(true);
    }

    public void Hide() {
        tooltip.SetActive(false);
    }
}
