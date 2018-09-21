using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour {

    public static float currentCameraZoom = 10f;

    public float MouseSensitivityX;
    public float MouseSensitivityY;
    public float ScrollSensitvity;
    public float OrbitDampening;
    public float ScrollDampening;
    public float MaxZoomValue;
    public float PlanetZoom;
    public bool CameraDisabled = false;

    private Transform cameraObj;
    private Transform cameraTarget;
    private Vector3 _LocalRotation;
    private Vector3 startVector;
    private Vector3 hitVector;
    private bool moveCamera = false;
    private bool CameraMoveZoom = false;


    // Use this for initialization
    void Start()
    {
        cameraObj = transform;
        cameraTarget = transform.parent;
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
                    CameraMoveZoom = true;
                }
            }
        }

        if (moveCamera)
        {
            startVector = cameraTarget.position;
            cameraTarget.position = Vector3.Lerp(startVector, hitVector, Time.deltaTime * ScrollDampening);

            if (Vector3.Distance(cameraTarget.position, hitVector) > 1)
            { 
                cameraObj.localPosition = new Vector3(0f, 0f, Mathf.Lerp(cameraObj.localPosition.z, PlanetZoom * -1f, Time.deltaTime * ScrollDampening));
                currentCameraZoom = -cameraObj.localPosition.z;
            }
            else
            {
                CameraMoveZoom = false;
            }

            if (cameraTarget.position == hitVector)
            {
                Debug.Log("movedone");
                moveCamera = false;
            }
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

                ScrollAmount *= (currentCameraZoom * 0.3f);

                currentCameraZoom += ScrollAmount * -1f;

                currentCameraZoom = Mathf.Clamp(currentCameraZoom, 5f, MaxZoomValue);
            }
        }

        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        cameraTarget.rotation = Quaternion.Lerp(cameraTarget.rotation, QT, Time.deltaTime * OrbitDampening);

        if (cameraObj.localPosition.z != currentCameraZoom * -1f && !CameraMoveZoom)
        {
            cameraObj.localPosition = new Vector3(0f, 0f, Mathf.Lerp(cameraObj.localPosition.z, currentCameraZoom * -1f, Time.deltaTime * ScrollDampening));
        }
    }
}