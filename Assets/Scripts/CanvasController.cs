using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    Camera camera = null;

    private float startScaleX;
    private float startScaleY;
    //private Vector3 cScale;
    private const float BodyScaleDivider = 1f;

    // Use this for initialization
    void Start () {

        camera = Camera.main;
        startScaleX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        /*transform.localScale = new Vector3(Mathf.Clamp(startScaleX * OrbitalCamera.currentCameraZoom * BodyScaleDivider, startScaleX, startScaleX * 2),
            Mathf.Clamp(startScaleY* OrbitalCamera.currentCameraZoom * BodyScaleDivider, startScaleY, startScaleY * 2), 1);*/
    }
}
