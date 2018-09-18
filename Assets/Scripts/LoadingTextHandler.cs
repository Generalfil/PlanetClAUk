using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingTextHandler : MonoBehaviour {

    public List<string> loadingStrings;

    private Text t;
    private UnityAction eventListener;

	// Update is called once per frame
	void Update ()
    {
		/*if (!(loadingStrings.Count < 1))
        {
            t.text = "Loading:";
        }*/
    }

    void Awake()
    {
        eventListener = new UnityAction(StartLoadingText);
        t = gameObject.GetComponent<Text>();
    }

    void OnEnable()
    {
        EventManager.StartListening("Start", eventListener);
        EventManager.StartListening("JPL1", ConnectingJPL);
        EventManager.StartListening("JPL2", ConnectedJPL);
        EventManager.StartListening("JPL3", DisconnectedJPL);
    }

    void OnDisable()
    {
        EventManager.StopListening("Start", eventListener);
        EventManager.StopListening("JPL1", ConnectingJPL);
        EventManager.StopListening("JPL2", ConnectingJPL);
        EventManager.StopListening("JPL3", DisconnectedJPL);
    }

    void StartLoadingText()
    {
        t.text = "Loading: ";
    }

    void ConnectingJPL()
    {
        t.text = "Connecting to JPL";
    }

    void ConnectedJPL()
    {
        t.text = "Connected to JPL: PROCESSING";
    }

    void DisconnectedJPL()
    {
        t.text = "JPL Done, Disconnecting";
    }
}
