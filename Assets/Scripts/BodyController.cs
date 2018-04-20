using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    JPLConnect jplConnect = new JPLConnect();

    // Use this for initialization
    void Start () {
        jplConnect.ServerSocket("horizons.jpl.nasa.gov", 6775);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}


