using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    Camera camera;

    private float startScaleX;
    private float startScaleY;

    // Use this for initialization
    void Start () {

        camera = Camera.main;
        startScaleX = this.transform.localScale.x;
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}
