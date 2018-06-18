using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class OrbitHandler : MonoBehaviour {

    public int resolution = 50;

    //public double mAnomaly;
    public double semiMajorAxis;
    public double eccentrity;
    public double LongitudeofP;
    public double longOfAccNode;
    public double inclination;

    public double eccentricAnomaly;
    public double trueAnomaly;
    public double distanceToCentralBody;
    public bool caluclateArgP;
    public double W; //Argument Of periapsis if that is calculated

    private Vector3[] positions;
    private LineRenderer lr;
    private int orbitTolerance = 6;


    void OnValidate()
    {
        UpdateEllipse();
    }

    public void UpdateEllipse()
    {
        if (lr == null)
            lr = GetComponent<LineRenderer>();

        lr.positionCount = resolution + 2;

        lr.SetPosition(0,AddPointToLineRenderer(0));
        for (int i = 1; i <= resolution + 1; i++)
        {
            lr.SetPosition(i, AddPointToLineRenderer(i));
        }
    }

    Vector3 AddPointToLineRenderer(float index)
    {
        Vector3 pointPosition;

        pointPosition = KeplerToCarthesian(index * Mathf.Deg2Rad , semiMajorAxis,eccentrity,LongitudeofP,longOfAccNode, inclination);

        return pointPosition;        
    }

    /// <summary>
    /// Convert keplarian elements into vector3, needs (Mean Anomaly, Semi Majoris Axis, Eccentricity, Longitude of Periapsis, Longitude of Ascending node, Inclination) 
    /// </summary>
    /// <returns></returns>
    Vector3 KeplerToCarthesian(double meanAnomaly , double a, double e, double w, double O, double inc)
    {
        //Deg2Rad so varbibles calculates properly
        inc *= Mathf.Deg2Rad;
        w *= Mathf.Deg2Rad;
        O *= Mathf.Deg2Rad;

        //Longitude of P to Argument of P
        if (caluclateArgP)
            W = w - O;
        else
            W = w;
 
        //Gets E and True Anomaly
        eccentricAnomaly = GetEccentricAnomaly(meanAnomaly, e);
        trueAnomaly = GetTrueAnomaly(e, eccentricAnomaly) * Mathf.Deg2Rad;

        distanceToCentralBody = a * ((1 - (e*e)) / (1 + e * Math.Cos(trueAnomaly)));

        Vector3 o = new Vector3((float)(distanceToCentralBody * Math.Cos(trueAnomaly)), (float)(distanceToCentralBody * Math.Sin(trueAnomaly)), 0);

        double rx, ry, rz;
        rx = o.x; ry = o.y; rz = o.z;

        rx = (o.x * (Math.Cos(W) * Math.Cos(O) - Math.Sin(W) * Math.Cos(inc) * Math.Sin(O)) -
                o.y * (Math.Sin(W) * Math.Cos(O) + Math.Cos(W) * Math.Cos(inc) * Math.Sin(O)));
        ry = (o.x * (Math.Cos(W) * Math.Sin(O) + Math.Sin(W) * Math.Cos(inc) * Math.Cos(O)) +
            o.y * (Math.Cos(W) * Math.Cos(inc) * Math.Cos(O) - Math.Sin(W) * Math.Sin(O)));
        rz = (o.x * (Math.Sin(W) * Math.Sin(inc)) + o.y * (Math.Cos(W) * Math.Sin(inc)));

        return new Vector3((float) rx * BodyController.auMultiplier, (float) rz * BodyController.auMultiplier, (float) ry * BodyController.auMultiplier);
    }

    private double GetTrueAnomaly(double e, double E)
    {
        double phi = Math.Atan2(Math.Sqrt(1 - e) * Math.Cos(E / 2), Math.Sqrt(1 + e) * Math.Sin(E / 2)) / (Math.PI / 180);

        return Math.Round(phi * Math.Pow(10, orbitTolerance)) / Math.Pow(10, orbitTolerance);
    }

    private double GetEccentricAnomaly(double meanAnomaly, double e)
    {
        //Solve kepler equation to get Ecentric anomaly
        
        int maxIter = 30, i = 0;
        double delta = Math.Pow(10, -orbitTolerance);
        double E, Function;

        meanAnomaly /= 180.0f;

        meanAnomaly = 2.0 * Math.PI * (meanAnomaly - Math.Floor(meanAnomaly));

        if (e < 0.8)
            E = meanAnomaly;
        else
            E = Math.PI;

        Function = E - e * Math.Sin(meanAnomaly) - meanAnomaly;

        while ((Math.Abs(Function) > delta) && (i < maxIter))
        {
            E = E - Function / (1.0 - e * Math.Cos(E));
            Function = E - e * Math.Sin(E) - meanAnomaly;
            i++;
        }

        E /= (Math.PI / 180.0);

        return Math.Round(E * Math.Pow(10, orbitTolerance)) / Math.Pow(10, orbitTolerance);
    }
}
