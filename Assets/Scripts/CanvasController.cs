using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    Camera camera = null;

    private float startScaleX;
    private float startScaleY;
    private Vector3 cScale;
    private const float BodyScaleDivider = 0.1f;

    // Use this for initialization
    void Start () {

        camera = Camera.main;
        cScale = transform.localScale;
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);

        /*gameObject.transform.localScale = new Vector3(
            cScale.x * OrbitalCamera.currentCameraZoom * BodyScaleDivider,
            cScale.y * OrbitalCamera.currentCameraZoom * BodyScaleDivider,
            
            Mathf.Clamp(cScale.x * OrbitalCamera.currentCameraZoom * BodyScaleDivider, cScale.x, cScale.x * 2),
            Mathf.Clamp(cScale.y * OrbitalCamera.currentCameraZoom * BodyScaleDivider, cScale.y, cScale.y * 2),
            1);*/
    }
}
