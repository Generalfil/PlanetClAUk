using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    JPLConnect jplConnect = new JPLConnect();

    private List<string> bodiesToAccess = new List<string>();
    public List<OrbitalBody> orbitalBodies = new List<OrbitalBody>();
    public GameObject body;

    public bool canUpdate = false;

    // Use this for initialization
    void Start () {
        
    }

    private void Awake()
    {
        foreach (Transform body in transform)
        {
            bodiesToAccess.Add(body.name);
        }

        AccessJPLHorizon();
    }

    // Update is called once per frame
    void Update () {

        if (jplConnect.clientDone)
        {
            orbitalBodies = jplConnect.GetBodyList();
            Debug.Log("Copied bodies");
            foreach (var item in orbitalBodies)
            {
                //GameObject.Find(item.ID).transform.position = new Vector3((float)item.X_value, (float)item.Z_value, (float)item.Y_value);
                //var go = GameObject.Find(item.ID).AddComponent<OrbitalBody>();

                //Fix so that every body has a orbitalbody as component, then in jplconnect update that component instead of trying to add a new one

            }
            jplConnect.clientDone = false;
            canUpdate = true;
        }

        //Temp update function
        if (canUpdate && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Started update");
            AccessJPLHorizon();
        }
	}

    private void AccessJPLHorizon()
    {
        jplConnect.ServerSocket("horizons.jpl.nasa.gov", 6775, bodiesToAccess);
    }
}


