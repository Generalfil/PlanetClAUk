﻿using System;
using UnityEngine;

public class OrbitalBody : MonoBehaviour
{
    private string m_id;
    private double m_x_value;
    private double m_y_value;
    private double m_z_value;
    private DateTime m_data_date;
    private Vector3 cScale;
    private const float BodyScaleDivider = 0.1f;

    public string ID
    {
        get { return m_id; }
        set { m_id = ID; }
    }
    public double X_value
    {
        get { return m_x_value; }
        set { m_x_value = X_value; }
    }
    public double Y_value
    {
        get { return m_y_value; }
        set { m_y_value = Y_value; }
    }
    public double Z_value
    {
        get { return m_z_value; }
        set { m_z_value = Z_value; }
    }
    public DateTime Data_Date
    {
        get { return m_data_date; }
        set { m_data_date = Data_Date; }
    }

    public OrbitalBody(string id, double x, double y, double z, DateTime dateTime)
    {
        m_id = id;
        m_x_value = x;
        m_y_value = y;
        m_z_value = z;
        m_data_date = dateTime;
        cScale = transform.localScale;
    }

    //Barely Used
    public void SetBodyReference(OrbitalBody o)
    {
        m_id = o.ID;
        m_x_value = o.X_value;
        m_y_value = o.Y_value;
        m_z_value = o.Z_value;
        m_data_date = o.Data_Date;
        cScale = transform.localScale;
    }

    public void SetBodyReference(BodySaveData o)
    {
        m_id = o.ID;
        m_x_value = o.X_value;
        m_y_value = o.Y_value;
        m_z_value = o.Z_value;
        m_data_date = DateTime.Parse(o.Data_Date);
        cScale = transform.localScale;
    }


    public void SetObjectPosition()
    {
        gameObject.transform.position = new Vector3((float)X_value * BodyController.auMultiplier, (float)Z_value * BodyController.auMultiplier, (float)Y_value * BodyController.auMultiplier);
    }

    private void Update()
    {
        //gameObject.transform.localScale = new Vector3(Mathf.Clamp(cScale.x * OrbitalCamera.currentCameraZoom * BodyScaleDivider, cScale.x, cScale.x*2),
        //    Mathf.Clamp(cScale.y * OrbitalCamera.currentCameraZoom * BodyScaleDivider, cScale.y, cScale.y * 2),
         //   Mathf.Clamp(cScale.z * OrbitalCamera.currentCameraZoom * BodyScaleDivider, cScale.z, cScale.z * 2));
    }
}
