using System;
using UnityEngine;

[Serializable]
public class BodySaveData
{
    [SerializeField]
    private string m_id;
    [SerializeField]
    private double m_x_value;
    [SerializeField]
    private double m_y_value;
    [SerializeField]
    private double m_z_value;
    [SerializeField]
    private string m_data_date;

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
    public string Data_Date
    {
        get { return m_data_date;}
        set { m_data_date = Data_Date; }
    }

    public BodySaveData(string id, double x, double y, double z, string dateTime)
    {
        m_id = id;
        m_x_value = x;
        m_y_value = y;
        m_z_value = z;
        m_data_date = dateTime;
    }
}

