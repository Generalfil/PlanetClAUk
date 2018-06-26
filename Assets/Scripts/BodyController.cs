using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    JPLConnect jplConnect = new JPLConnect();

    private List<string> bodiesToAccess = new List<string>();
    private List<GameObject> activeBodies = new List<GameObject>();
    private SaveLoad saveLoad;

    public List<BodySaveData> orbitalBodyData = new List<BodySaveData>();

    //Sets modfier for AU scale. Default: 10 (1 unit in Unity = 0.1 AU) 
    public static int auMultiplier = 10;

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
        saveLoad = gameObject.AddComponent<SaveLoad>();
        
        //Load saved positions
        saveLoad.LoadOrbitalBodies();
        UpdateActiveBodies(saveLoad.GetLoadedBodies());

        //Contact Nasa
        AccessJPLHorizon();
    }

    // Update is called once per frame
    void Update () {

        if (jplConnect.clientDone)
        {
            orbitalBodyData = jplConnect.GetBodyList();
            Debug.Log("Copied bodies");

            UpdateActiveBodies(orbitalBodyData);

            Debug.Log("Try save");
            saveLoad.SaveOrbitalBodies(orbitalBodyData);

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

    /// <summary>
    /// Method to update all bodies in scene
    /// </summary>
    private void UpdateActiveBodies<T>(List<T> m_bodyRefrence)
    {
        Debug.Log("Trying to update active bodies");
        for (int i = 0; i < activeBodies.Count; i++)
        {
            try
            {
                activeBodies[i].SetActive(true);

                if (m_bodyRefrence is List<OrbitalBody>)
                {
                    activeBodies[i].GetComponent<OrbitalBody>()
                        .SetBodyReference(m_bodyRefrence[i] as OrbitalBody);
                }
                else
                {
                    activeBodies[i].GetComponent<OrbitalBody>()
                        .SetBodyReference(m_bodyRefrence[i] as BodySaveData);
                }
                
                activeBodies[i].GetComponent<OrbitalBody>()
                    .SetObjectPosition();
            }
            catch
            {
                Debug.Log("Cant update component on: " + activeBodies[i].name);
                activeBodies[i].SetActive(false);
            }
        }
    }

    private void AccessJPLHorizon()
    {
        jplConnect.ServerSocket("horizons.jpl.nasa.gov", 6775, bodiesToAccess);
    }
}


