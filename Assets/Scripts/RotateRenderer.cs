using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRenderer : MonoBehaviour
{
	//public int debugV = 1;
	private int latestUpdateMin;
	private int latestUpdateSec;

	public bool debugRotate = false;

	DateTime debugTime;

	void Awake()
	{
		transform.localRotation = RotateOrbitalBody(transform.localRotation, DateTime.UtcNow);
		latestUpdateMin = DateTime.UtcNow.Hour + DateTime.UtcNow.Minute;
		latestUpdateSec = DateTime.UtcNow.Second;
		debugTime = DateTime.UtcNow;
	}

	// Update is called once per frame
	void Update ()
    {
		if (!debugRotate)
		{
			int currentMin = DateTime.UtcNow.Hour * 60 + DateTime.UtcNow.Minute;
			if (currentMin > latestUpdateMin || currentMin == 0)
			{
				transform.localRotation = RotateOrbitalBody(transform.localRotation, DateTime.UtcNow);
				latestUpdateMin = currentMin;
			}
		}
		else
		{
			int currentSec = DateTime.UtcNow.Second;
			if (currentSec > latestUpdateSec || currentSec == 0)
			{
				Debug.Log(debugTime);
				debugTime = debugTime.AddHours(1);
				Debug.Log(debugTime);
				transform.localRotation = RotateOrbitalBody(transform.localRotation, debugTime);
				latestUpdateSec = currentSec;
			}
		}
		
	}

    private Quaternion RotateOrbitalBody(Quaternion orgRotation, DateTime now)
    {
		Debug.Log("Hr:" + now.Hour);
		Debug.Log("Min:" + now.Minute);
        float currentMinute = (0.25f*(now.Hour * 60 + now.Minute));

        orgRotation = Quaternion.Euler(0, -currentMinute-90, 0);

        return orgRotation;
    }
}