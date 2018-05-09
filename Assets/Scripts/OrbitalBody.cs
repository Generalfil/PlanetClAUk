using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalBody : MonoBehaviour {

    private string m_id;
    private double m_x_value;
    private double m_y_value;
    private double m_z_value;

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
    public OrbitalBody(string id, double x, double y, double z)
    {
        m_id = id;
        m_x_value = x;
        m_y_value = y;
        m_z_value = z;

        //GameObject.Find(m_id).AddComponent<OrbitalBody>();
    }
}
