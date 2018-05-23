using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaveLoad : MonoBehaviour {

    private string gameDataFileName = "data.json";
    private List<BodySaveData> LoadedBodies = new List<BodySaveData>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadOrbitalBodies();
    }

    public List<BodySaveData> GetLoadedBodies()
    {
        return LoadedBodies;
    }

    public void LoadOrbitalBodies()
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

                BodySaveData loadedBody = JsonUtility.FromJson<BodySaveData>(temp);
                LoadedBodies.Add(loadedBody);
            }

            Debug.Log("he");
            //BodySaveData loadedData = JsonUtility.FromJson<BodySaveData>(dataAsJson);

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
            var saveObj = new BodySaveData(item.ID, item.X_value, item.Y_value, item.Z_value);
            sb.Append(JsonUtility.ToJson(saveObj));
        }

        string filePath = Application.dataPath + gameDataFileName;
        File.WriteAllText(filePath, sb.ToString());
    }
}
