using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class OrbitHandler : MonoBehaviour {

    public Vector3 radius = new Vector3(1f, 1f, 1f);
    //public float rotationAngle = 45;
    private int resolution = 10;

    //public double mAnomaly;
    public float semiMajorAxis;
    public float eccentrity;
    public float argumentofP;
    public float longOfAccNode;
    public float inclination;

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

        AddPointToLineRenderer( 0);
        for (int i = 1; i <= resolution + 1; i++)
        {
            AddPointToLineRenderer(i);
        }
        AddPointToLineRenderer(resolution + 2);
    }

    void AddPointToLineRenderer(int index)
    {
        // pointQuaternion = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
        Vector3 pointPosition;

        pointPosition = KeplerToCarthesian(index, semiMajorAxis,eccentrity,argumentofP,longOfAccNode, inclination);
        //pointPosition = pointQuaternion * pointPosition;

        lr.SetPosition(index, pointPosition);
    }

    /// <summary>
    /// Convert keplarian elements into vector3, needs (Mean Anomaly, Semi Majoris Axis, Eccentricity, Argument of Periapsis, Longitude of Ascending node, Inclination) 
    /// </summary>
    /// <returns></returns>
    Vector3 KeplerToCarthesian(double meanAnomaly , float a, float e, float w, float lan, float inc)
    {
        double E = GetEccentricAnomaly(meanAnomaly, e);
        double T = GetTrueAnomaly(e, E);

        double nu = 2 * Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E / 2));

        double r = a * (1 - e * Math.Cos(E));

        //h = Math.Sqrt(mu * a * (1 - e **2));

        double X = r * (Math.Cos(lan) * Math.Cos(w + nu) - Math.Sin(lan) * Math.Sin(w + nu) * Math.Cos(inc));
        double Y = r * (Math.Sin(lan) * Math.Cos(w + nu) + Math.Cos(lan) * Math.Sin(w + nu) * Math.Cos(inc));
        double Z = r * (Math.Sin(inc) * Math.Sin(w + nu));

        return new Vector3((float)X *10, (float) Z*10, (float) Y*10);
    }

    private double GetTrueAnomaly(float e, double E)
    {
        int dp = 6;
        double S = Math.Sin(E);

        double C = Math.Cos(E);

        double fak = Math.Sqrt(1.0 - e * e);

        double phi = Math.Atan2(fak * S, C - e) / Mathf.Deg2Rad;

        return Math.Round(phi * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }

    private double GetEccentricAnomaly(double meanAnomaly, float e)
    {
        //Solve kepler equation to get Ecentric anomaly
        int tolerance = 6;
        int maxIter = 30, i = 0;
        double delta = Math.Pow(10, -tolerance);
        double E, F;

        meanAnomaly /= 360.0f;

        meanAnomaly = 2.0 * Math.PI * (meanAnomaly - Math.Floor(meanAnomaly));

        if (e < 0.8)
            E = meanAnomaly;
        else
            E = Math.PI;

        F = E - e * Math.Sin(meanAnomaly) - meanAnomaly;

        //Refine Function(E) loop
        while ((Math.Abs(F) > delta) && (i < maxIter))
        {

            E = E - F / (1.0 - e * Math.Cos(E));
            F = E - e * Math.Sin(E) - meanAnomaly;
            i++;
        }

        E *= Mathf.Deg2Rad;

        return Math.Round(E * Math.Pow(10, tolerance)) / Math.Pow(10, tolerance);
    }
}
