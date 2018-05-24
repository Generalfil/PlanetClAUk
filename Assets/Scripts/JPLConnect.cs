﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;

public class JPLConnect {

    private TcpClient client;
    private Thread readWriteThread;
    private NetworkStream networkStream;
    private DateTime today;
    private DateTime tomorrow;
    private List<string> m_bodiesToAccess;
    private List<OrbitalBody> oribtalBodies;

    public bool clientDone = false;
    private string startingBody;

    public JPLConnect()
    {
        oribtalBodies = new List<OrbitalBody>();
    }

    void OnApplicationExit()
    {
        readWriteThread.Abort();
    }

    public List<OrbitalBody> GetBodyList()
    {
        return oribtalBodies;
    }

    public void ServerSocket(string ip, int port, List<string> bodiesToAccess)
    {
        m_bodiesToAccess = bodiesToAccess;
        startingBody = m_bodiesToAccess[0];

        try
        {
            client = new TcpClient(ip, port);
            Debug.Log("Connected to server.");
            today = DateTime.Now;
            tomorrow = today.AddDays(1);
        }
        catch (SocketException)
        {
            Debug.Log("Failed to connect to server");
            return;
        }

        //Assign networkstream
        networkStream = client.GetStream();

        //start socket read/write thread
        readWriteThread = new Thread(readWrite);
        readWriteThread.Start();

    }

    private void readWrite()
    {
        string command, recieved;
        bool title = false;

        //Read first thing given
        recieved = read();
        Debug.Log(recieved);

        //Set up connection loop
        while (true)
        {
            //Check if at main menu
            if (recieved == "\r\nHorizons> ")
            {
                title = true;
                Debug.Log("Accessed Horizons");
            }

            //If not at main menu, send empty command
            if (!title)
            {
                command = "";
            }
            else
            {
                try
                {
                    //Accesses bodies
                    foreach (var body in m_bodiesToAccess)
                    {
                        short planet_timeout = 50;
                        for (int attempts = 0; attempts < 5; attempts++)
                        {
                            Thread.Sleep(planet_timeout);
                            try
                            {
                                oribtalBodies.Add(AccessBody(body));
                                recieved = read();
                                Thread.Sleep(planet_timeout);
                                break;
                            }
                            catch
                            {
                                planet_timeout += 50;
                                Debug.Log("Failed To Access:" + body + "Attempt:" + attempts + "Retrying:");
                                Thread.Sleep(planet_timeout);
                            }
                        }
                    }

                    break;
                }
                catch
                {
                    Debug.Log("Failed to get orbitalbodies, ending connection");
                    break;
                }
               
            }
            
            write(command);
            Thread.Sleep(300);
            recieved = read();

            Debug.Log(recieved);
        }

        Debug.Log("Disconnected from server");
        networkStream.Close();
        client.Close();
        clientDone = true;
    }


    /// <summary>
    /// Accesses orbital body through horizons and extracts ephemeris into vars and returns those vars into a double[]
    /// </summary>
    /// <param name="id"></param>
    public OrbitalBody AccessBody(string id)
    {
        string[] initCommands = new string[] { "e", "v", "500@0", "y", "eclip", today.ToString(), tomorrow.ToString(), "1d", "y", "1", "n" };
        string[] followUpCommands = new string[] { "e+", "n" };
        double[] bodyChar = new double[3];
        StringBuilder sb = new StringBuilder();
        string m_stringholder;


        Debug.Log("Starting Body" + id);
        //Send body ID
        write(id);
        sb.Append(read());

        if (id == startingBody)
        {
            m_stringholder = SendCommandsForBody(sb, initCommands);
        }
        else
        {
            m_stringholder = SendCommandsForBody(sb, followUpCommands);
        }
        
        //Get empheris
        //$$SOE Start of ephemeris
        int startPos = m_stringholder.LastIndexOf("$$SOE") + "$$SOE".Length + 1;
        //$$EOE End of ephemeris
        int length = m_stringholder.IndexOf("$$EOE") - startPos;
        string sub = m_stringholder.Substring(startPos, length);

        //Split into only today and it X Y Z sections
        m_stringholder = sub.Substring(sub.IndexOf("X"), sub.IndexOf("\r\n VX") - sub.IndexOf("X"));
        Debug.Log("Ephemeris done");

        //Split into Vars
        bodyChar = SplitIntoVars(m_stringholder);

        OrbitalBody orbitalBody = new OrbitalBody(id, bodyChar[0], bodyChar[1], bodyChar[2]);
        Debug.Log(id + " body done");

        return orbitalBody;
    }

    private string SendCommandsForBody(StringBuilder sb, string[] _commands)
    {
        string m_stringholder;
        //Send Command loop
        for (int i = 0; i < _commands.Length; i++)
        {
            Debug.Log("Entered Command Loop");
            Thread.Sleep(10);
            write(_commands[i]);
            Thread.Sleep(10);
            sb.Append(read());
        }
        Debug.Log("Exits Command loop");
        sb.Append(read());
        m_stringholder = sb.ToString();
        return m_stringholder;
    }

    /// <summary>
    /// Splits a string into x y z decimals, these are converted to a double
    /// </summary>
    /// <param name="m_stringholder"></param>
    private double[] SplitIntoVars(string m_stringholder)
    {
        double[] m_bodyChar = new double[3];
        string[] splitString = m_stringholder.Split('=');
        for (int i = 1; i < 4; i++)
        {
            string m_split;
            if (splitString[i].Length > 22)
            {
                m_split = splitString[i].Substring(0, splitString[i].IndexOf(' ', 10));
            }
            else
            {
                m_split = splitString[i];
            }

            if (m_split[0] == ' ')
            {
                m_split = m_split.Substring(1);
            }

            m_bodyChar[i - 1] = (double)Decimal.Parse(m_split, System.Globalization.NumberStyles.Float);
        }
        Debug.Log("Converted to vars");
        return m_bodyChar;
    }

    /// <summary>
    /// Converts message string into bytes and then sends them to JPL
    /// </summary>
    /// <param name="message"></param>
    public void write(string message)
    {
        message += Environment.NewLine;
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        networkStream.Write(messageBytes, 0, messageBytes.Length);
        networkStream.Flush();
    }


    /// <summary>
    /// Reads network stream, has a buffer of 1024 bytes but reads all data through networkStream.DataAvailable
    /// </summary>
    /// <returns></returns>
    public string read()
    {
        byte[] data = new byte[1024];
        StringBuilder recieved = new StringBuilder();

        int numberOfBytesRead = 0;

        // Incoming message may be larger than the buffer size, therefore reads all available data through this loop before stitching it togheter. 
        do
        {
            numberOfBytesRead = networkStream.Read(data, 0, data.Length);
            recieved.AppendFormat("{0}", Encoding.ASCII.GetString(data, 0, numberOfBytesRead));
        }
        while (networkStream.DataAvailable);
        
        
        return recieved.ToString();
    }

}