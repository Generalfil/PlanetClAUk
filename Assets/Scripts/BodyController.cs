using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    JPLConnect jplConnect = new JPLConnect();

    private List<string> bodiesToAccess = new List<string>();
    private List<GameObject> activeBodies = new List<GameObject>();
    private SaveLoad saveLoad = new SaveLoad();

    public List<OrbitalBody> orbitalBodyData = new List<OrbitalBody>();

    public bool canUpdate = false;

    // Use this for initialization
    void Start () {
        
    }

    private void Awake()
    {
        foreach (Transform bChild in transform)
        {
            bodiesToAccess.Add(bChild.name);
            activeBodies.Add(bChild.gameObject);
        }
        gameObject.AddComponent<SaveLoad>();

        AccessJPLHorizon();
    }

    // Update is called once per frame
    void Update () {

        if (jplConnect.clientDone)
        {
            orbitalBodyData = jplConnect.GetBodyList();
            Debug.Log("Copied bodies");

            for (int i = 0; i < activeBodies.Count; i++)
            {
                try
                {
                    activeBodies[i].SetActive(true);

                    activeBodies[i].GetComponent<OrbitalBody>()
                        .SetBodyReference(orbitalBodyData[i]);

                    activeBodies[i].GetComponent<OrbitalBody>()
                        .SetObjectPosition();
                }
                catch
                {
                    Debug.Log("Cant update component on: " + activeBodies[i].name);
                    activeBodies[i].SetActive(false);
                } 
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
        if (canUpdate && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Try save");
            saveLoad.SaveOrbitalBodies(orbitalBodyData);
        }
    }

    private void AccessJPLHorizon()
    {
        jplConnect.ServerSocket("horizons.jpl.nasa.gov", 6775, bodiesToAccess);
    }
}


