using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRenderer : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        transform.localRotation = RotateOrbitalBody(transform.localRotation);
	}

    private Quaternion RotateOrbitalBody(Quaternion orgRotation)
    {
        var now = DateTime.UtcNow;
        float currentMinute = ((now.Hour * 60 + now.Minute))/360 + 180;

        orgRotation = Quaternion.Euler(0, -currentMinute, 0);

        return orgRotation;
    }
}
