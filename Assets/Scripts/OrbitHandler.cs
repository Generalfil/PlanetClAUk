using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class OrbitHandler : MonoBehaviour {

    public Vector3 radius = new Vector3(1f, 1f, 1f);
    public float rotationAngle = 45;
    public int resolution = 500;

    private Vector3[] positions;
    private LineRenderer lr;


    void OnValidate()
    {
        UpdateEllipse();
    }

    public void UpdateEllipse()
    {
        if (lr == null)
            lr = GetComponent<LineRenderer>();

        lr.positionCount = resolution + 3;

        AddPointToLineRenderer(0f, 0);
        for (int i = 1; i <= resolution + 1; i++)
        {
            AddPointToLineRenderer((float)i / (float)(resolution) * 2.0f * Mathf.PI, i);
        }
        AddPointToLineRenderer(0f, resolution + 2);
    }

    void AddPointToLineRenderer(float angle, int index)
    {
        Quaternion pointQuaternion = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
        Vector3 pointPosition;

        pointPosition = new Vector3(radius.x * Mathf.Cos(angle), radius.z * Mathf.Sin(angle), radius.y * Mathf.Sin(angle));
        pointPosition = pointQuaternion * pointPosition;

        lr.SetPosition(index, pointPosition);
    }
}
