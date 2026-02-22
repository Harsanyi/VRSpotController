using System;
using System.IO;
using UnityEngine;

public class Config
{
    private const string FILE_NAME = "config";

    private static string fullFilePath => Path.Combine(Application.dataPath, "_App/Resources", FILE_NAME+".json");
    private static Config _loaded = null;

    public static Config loaded
    {
        get
        {
            if (_loaded == null)Load();
            return _loaded;
        }
    }

    public string serverIP = "127.0.0.1";
    public int serverPort = 5000;
    public string serverAddress => $"http://{serverIP}:{serverPort}";

    private static void Load()
    {
        try
        {
            TextAsset file = Resources.Load<TextAsset>(FILE_NAME);
            _loaded = JsonUtility.FromJson<Config>(file.text);
            Debug.Log("Config file loaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            _loaded = new Config();
            _loaded.Save();
        }
    }

    public static void Delete()
    {
        if (!File.Exists(fullFilePath))
        {
            Debug.LogError("Config file not found. Cannot be deleted.");
            return;
        }
        
        File.Delete(fullFilePath);
        Debug.Log("Config file deleted successfully.");
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this,true);
        using (StreamWriter sw = new StreamWriter(fullFilePath))
        {
            sw.Write(json);
        }
        
        Debug.Log("New Config file created!");
    }

}
