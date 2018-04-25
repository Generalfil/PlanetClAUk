using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    JPLConnect jplConnect = new JPLConnect();

    public List<string> bodiesToAccess;

    public List<OrbitalBody> orbitalBodies;

    public GameObject body;

    // Use this for initialization
    void Start () {
        orbitalBodies = new List<OrbitalBody>();
    }
    private void Awake()
    {
        jplConnect.ServerSocket("horizons.jpl.nasa.gov", 6775, bodiesToAccess);
    }


    // Update is called once per frame
    void Update () {
        if (jplConnect.clientDone)
        {
            orbitalBodies = jplConnect.GetBodyList();
            Debug.Log("copied bodies");
            foreach (var item in orbitalBodies)
            {
                body.name = item.ID;           
                Instantiate(body, new Vector3((float)item.X_value, (float)item.Z_value, (float)item.Y_value), transform.rotation);
            }
            jplConnect.clientDone = false;
        }
	}
}


