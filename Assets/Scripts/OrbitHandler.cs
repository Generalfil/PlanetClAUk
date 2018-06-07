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

    public double E;
    public double T;
    public double r;
    public bool caluclateArgP;
    public double W;

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

        lr.positionCount = resolution + 2;

        lr.SetPosition(0,AddPointToLineRenderer( 0));
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
        E = GetEccentricAnomaly(meanAnomaly, e);
        T = GetTrueAnomaly(e, E) * Mathf.Deg2Rad;

        r = a * ((1 - (e*e)) / (1 + e * Math.Cos(T)));

        Vector3 o = new Vector3((float)(r * Math.Cos(T)), (float)(r * Math.Sin(T)), 0);

        double rx, ry, rz;
        rx = o.x; ry = o.y; rz = o.z;

        rx = (o.x * (Math.Cos(W) * Math.Cos(O) - Math.Sin(W) * Math.Cos(inc) * Math.Sin(O)) -
                o.y * (Math.Sin(W) * Math.Cos(O) + Math.Cos(W) * Math.Cos(inc) * Math.Sin(O)));
        ry = (o.x * (Math.Cos(W) * Math.Sin(O) + Math.Sin(W) * Math.Cos(inc) * Math.Cos(O)) +
            o.y * (Math.Cos(W) * Math.Cos(inc) * Math.Cos(O) - Math.Sin(W) * Math.Sin(O)));
        rz = (o.x * (Math.Sin(W) * Math.Sin(inc)) + o.y * (Math.Cos(W) * Math.Sin(inc)));

        //2D Code
        /*double C = Math.Cos(E);
        double S = Math.Sin(E);

        rx = r * (C - e);
        ry = r * Math.Sqrt(1.0 - e * e) * S;
        rz = 0;*/

        return new Vector3((float)rx *10, (float) rz*10, (float) ry*10);
    }

    private double GetTrueAnomaly(double e, double E)
    {
        int dp = 6;
        double phi = Math.Atan2(Math.Sqrt(1 - e) * Math.Cos(E / 2), Math.Sqrt(1 + e) * Math.Sin(E / 2)) / (Math.PI / 180);

        return Math.Round(phi * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }

    private double GetEccentricAnomaly(double meanAnomaly, double e)
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

        while ((Math.Abs(F) > delta) && (i < maxIter))
        {

            E = E - F / (1.0 - e * Math.Cos(E));
            F = E - e * Math.Sin(E) - meanAnomaly;
            i++;
        }

        E /= (Math.PI / 180.0);

        return Math.Round(E * Math.Pow(10, tolerance)) / Math.Pow(10, tolerance);
    }
}
