using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTextHandler : MonoBehaviour {

    public List<string> loadingStrings;
    private Text t;

	// Use this for initialization
	void Start ()
    {
        loadingStrings = new List<string>();
        t = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (!(loadingStrings.Count < 1))
        {
            t.text = "Loading:";
        }
    }
}
