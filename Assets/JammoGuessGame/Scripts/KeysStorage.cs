using System.IO;
using UnityEngine;

public static class KeysStorage
{
    [System.Serializable]
    public class ConfigData
    {
        public string openai_key;
        public string elevenlabs;
    }


    public static ConfigData Data
    {
        get
        {
            var filePath = Path.Combine(Application.dataPath, "../keys.json");
            var jsonContent = File.ReadAllText(filePath);
            var configData = JsonUtility.FromJson<ConfigData>(jsonContent);
            return configData;
        }
    }
}
