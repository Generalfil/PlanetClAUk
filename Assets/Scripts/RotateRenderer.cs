using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRenderer : MonoBehaviour
{
	//public int debugV = 1;
	int latestUpdateMin;
	void Awake()
	{
		transform.localRotation = RotateOrbitalBody(transform.localRotation);
		latestUpdateMin = DateTime.UtcNow.Minute;
	}

	// Update is called once per frame
	void Update ()
    {
		int currentMin = DateTime.UtcNow.Minute;
		if (currentMin > latestUpdateMin)
		{
			transform.localRotation = RotateOrbitalBody(transform.localRotation);
			latestUpdateMin = currentMin;
		} 
	}

    private Quaternion RotateOrbitalBody(Quaternion orgRotation)
    {
        var now = DateTime.UtcNow;
		/*Debug.Log("Hr:" + now.Hour);
		Debug.Log("Min:" + now.Minute);*/
        float currentMinute = (0.25f*(now.Hour * 60 + now.Minute));

        orgRotation = Quaternion.Euler(0, -currentMinute, 0);

        return orgRotation;
    }
}