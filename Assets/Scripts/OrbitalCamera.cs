﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour {

    private Transform CameraObj;
    private Transform CameraTarget;
    private Vector3 _LocalRotation;
    private Vector3 startVector;
    private Vector3 hitVector;
    private float CameraZoom = 10f;
    private bool moveCamera = false;

    public float MouseSensitivityX;
    public float MouseSensitivityY;
    public float ScrollSensitvity;
    public float OrbitDampening;
    public float ScrollDampening;
    public float MaxZoomValue;
    public float PlanetZoom;
    public bool CameraDisabled = false;



    // Use this for initialization
    void Start()
    {
        CameraObj = transform;
        CameraTarget = transform.parent;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if left button pressed...
            Ray ray = this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Body")
                {
                    hitVector = hit.transform.position;
                    moveCamera = true;
                }
            }
        }

        if (moveCamera)
        {
            startVector = CameraTarget.position;
            CameraTarget.position = Vector3.Lerp(startVector, hitVector, Time.deltaTime * ScrollDampening);
            CameraObj.localPosition = new Vector3(0f, 0f, Mathf.Lerp(CameraObj.localPosition.z, PlanetZoom * -1f, Time.deltaTime * ScrollDampening));
            if (CameraTarget.position == hitVector)
                moveCamera = false;
        }
    }
    void LateUpdate()
    {
        //Debug only
        if (Input.GetKeyDown(KeyCode.LeftShift))
            CameraDisabled = !CameraDisabled;

        if (!CameraDisabled)
        {
            if (Input.GetMouseButton(1))
            {
                //Rotation of the Camera based on Mouse Coordinates
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivityX;
                    _LocalRotation.y += -Input.GetAxis("Mouse Y") * MouseSensitivityY;

                    //Clamp the y Rotation to horizon and not flipping over at the top
                    if (_LocalRotation.y < -90f)
                        _LocalRotation.y = -90f;
                    else if (_LocalRotation.y > 90f)
                        _LocalRotation.y = 90f;
                }
            }
            //Zooming Input from our Mouse Scroll Wheel
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

                ScrollAmount *= (CameraZoom * 0.3f);

                CameraZoom += ScrollAmount * -1f;

                CameraZoom = Mathf.Clamp(CameraZoom, 5f, MaxZoomValue);
            }
        }

        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        CameraTarget.rotation = Quaternion.Lerp(CameraTarget.rotation, QT, Time.deltaTime * OrbitDampening);

        if (CameraObj.localPosition.z != CameraZoom * -1f)
        {
            CameraObj.localPosition = new Vector3(0f, 0f, Mathf.Lerp(CameraObj.localPosition.z, CameraZoom * -1f, Time.deltaTime * ScrollDampening));
        }
    }
}