using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaveLoad : MonoBehaviour {

    private string gameDataFileName = "data.json";
    private List<OrbitalBody> LoadedBodies = new List<OrbitalBody>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadOrbitalBodies();
    }

    public List<OrbitalBody> GetLoadedBodies()
    {
        return LoadedBodies;
    }

    private void LoadOrbitalBodies()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Application.dataPath + gameDataFileName;

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            string[] jsonBodies = dataAsJson.Split(new[] { '{' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in jsonBodies)
            {
                string temp = "{"+item;

                OrbitalBody loadedBody = JsonUtility.FromJson<OrbitalBody>(temp);
                LoadedBodies.Add(loadedBody);
            }

            Debug.Log("he");
            //OrbitalBody loadedData = JsonUtility.FromJson<OrbitalBody>(dataAsJson);

            // Retrieve the allRoundData property of loadedData
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    // This function could be extended easily to handle any additional data we wanted to store in our PlayerProgress object
    public void SaveOrbitalBodies(List<OrbitalBody> objToSave)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in objToSave)
        {
            sb.Append(JsonUtility.ToJson(item));
        }

        string filePath = Application.dataPath + gameDataFileName;
        File.WriteAllText(filePath, sb.ToString());
    }
}
