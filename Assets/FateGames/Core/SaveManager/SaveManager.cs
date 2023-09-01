using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FateGames.Core;
using System.Text;
using UnityEditor;

public class SaveManager : Singleton<SaveManager>
{
    private SaveData data;
    [SerializeField] private string fileName = "SaveFile.fate";
    [SerializeField] private int encryptionKey = 813836;
    List<Saveable> references = new();
    bool isLoaded = false;

    public static int Level { get => Instance.GetInt("level", 1); set => Instance.SetInt("level", value); }
    public static int Money { get => Instance.GetInt("money", 0); set => Instance.SetInt("money", value); }
    public static int Gem { get => Instance.GetInt("gem", 0); set => Instance.SetInt("gem", value); }
    private static float savedTotalPlaytime { get => Instance.GetFloat("totalPlaytime", 0); set => Instance.SetFloat("totalPlaytime", value); }
    public static float TotalPlaytime { get => savedTotalPlaytime + Time.time - lastTotalPlaytimeSaveTime; }
    private static float lastTotalPlaytimeSaveTime = 0;

    public void SaveTotalPlaytime()
    {
        if (!isLoaded) return;
        savedTotalPlaytime = TotalPlaytime;
        lastTotalPlaytimeSaveTime = Time.time;
    }

    public SaveData Data => data;

    public void Register(Saveable reference)
    {
        if (!references.Contains(reference))
            references.Add(reference);
    }
    public bool TryGetInt(string key, out int value)
    {
        value = 0;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.intFields.ContainsKey(key))
        {
            value = data.intFields[key];
            return true;
        }
        return false;
    }
    public int GetInt(string key, int defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        int value = defaultValue;
        if (data.intFields.ContainsKey(key))
            value = data.intFields[key];
        return value;
    }

    public void SetInt(string key, int value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.intFields.ContainsKey(key))
        {
            data.intFields[key] = value;
        }
        else
        {
            data.intFields.Add(key, value);
        }
    }
    public bool TryGetFloat(string key, out float value)
    {
        value = 0;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.floatFields.ContainsKey(key))
        {
            value = data.floatFields[key];
            return true;
        }
        return false;
    }
    public float GetFloat(string key, float defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        float value = defaultValue;
        if (data.floatFields.ContainsKey(key))
            value = data.floatFields[key];
        return value;
    }

    public void SetFloat(string key, float value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.floatFields.ContainsKey(key))
        {
            data.floatFields[key] = value;
        }
        else
        {
            data.floatFields.Add(key, value);
        }
    }
    public bool TryGetString(string key, out string value)
    {
        value = "";
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.stringFields.ContainsKey(key))
        {
            value = data.stringFields[key];
            return true;
        }
        return false;
    }
    public string GetString(string key, string defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        string value = defaultValue;
        if (data.stringFields.ContainsKey(key))
            value = data.stringFields[key];
        return value;
    }

    public void SetString(string key, string value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.stringFields.ContainsKey(key))
        {
            data.stringFields[key] = value;
        }
        else
        {
            data.stringFields.Add(key, value);
        }
    }
    public bool TryGetBool(string key, out bool value)
    {
        value = false;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.boolFields.ContainsKey(key))
        {
            value = data.boolFields[key];
            return true;
        }
        return false;
    }
    public bool GetBool(string key, bool defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        bool value = defaultValue;
        if (data.boolFields.ContainsKey(key))
            value = data.boolFields[key];
        return value;
    }

    public void SetBool(string key, bool value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.boolFields.ContainsKey(key))
        {
            data.boolFields[key] = value;
        }
        else
        {
            data.boolFields.Add(key, value);
        }
    }
    public bool TryGetIntList(string key, out List<int> value)
    {
        value = null;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.intListFields.ContainsKey(key))
        {
            value = data.intListFields[key];
            return true;
        }
        return false;
    }
    public List<int> GetIntList(string key, List<int> defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        List<int> value = defaultValue;
        if (data.intListFields.ContainsKey(key))
            value = data.intListFields[key];
        return value;
    }

    public void SetIntList(string key, List<int> value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.intListFields.ContainsKey(key))
        {
            data.intListFields[key] = value;
        }
        else
        {
            data.intListFields.Add(key, value);
        }
    }
    public bool TryGetFloatList(string key, out List<float> value)
    {
        value = null;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.floatListFields.ContainsKey(key))
        {
            value = data.floatListFields[key];
            return true;
        }
        return false;
    }
    public List<float> GetFloatList(string key, List<float> defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        List<float> value = defaultValue;
        if (data.floatListFields.ContainsKey(key))
            value = data.floatListFields[key];
        return value;
    }

    public void SetFloatList(string key, List<float> value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.floatListFields.ContainsKey(key))
        {
            data.floatListFields[key] = value;
        }
        else
        {
            data.floatListFields.Add(key, value);
        }
    }
    public bool TryGetStringList(string key, out List<string> value)
    {
        value = null;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.stringListFields.ContainsKey(key))
        {
            value = data.stringListFields[key];
            return true;
        }
        return false;
    }
    public List<string> GetStringList(string key, List<string> defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        List<string> value = defaultValue;
        if (data.stringListFields.ContainsKey(key))
            value = data.stringListFields[key];
        return value;
    }

    public void SetStringList(string key, List<string> value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.stringListFields.ContainsKey(key))
        {
            data.stringListFields[key] = value;
        }
        else
        {
            data.stringListFields.Add(key, value);
        }
    }
    public bool TryGetBoolList(string key, out List<bool> value)
    {
        value = null;
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.boolListFields.ContainsKey(key))
        {
            value = data.boolListFields[key];
            return true;
        }
        return false;
    }
    public List<bool> GetBoolList(string key, List<bool> defaultValue)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        List<bool> value = defaultValue;
        if (data.boolListFields.ContainsKey(key))
            value = data.boolListFields[key];
        return value;
    }

    public void SetBoolList(string key, List<bool> value)
    {
        if (!isLoaded)
        {
            Debug.Log("Not loaded!");
            Load();
        }
        if (data.boolListFields.ContainsKey(key))
        {
            data.boolListFields[key] = value;
        }
        else
        {
            data.boolListFields.Add(key, value);
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }

    }
#if UNITY_EDITOR
    private void OnApplicationQuit()
    {

        Save();
    }
#endif
    public void Load()
    {
        if (isLoaded) return;

        if (TryReadFromFile(fileName, out string json))
        {
            string decryptedJson = EncryptDecrypt(json, encryptionKey);
            data = JsonConvert.DeserializeObject<SaveData>(decryptedJson);
            //JsonUtility.FromJsonOverwrite(json, data);
        }
        else
        {
            data = new SaveData();
            Save();
        }
        isLoaded = true;
    }
    public void Save()
    {
        Save(data);
    }
    public void SaveReferenceFields()
    {
        if (!isLoaded) return;
        for (int i = 0; i < references.Count; i++)
        {
            references[i].SaveFields();
        }
    }

    public void Save(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("Cannot save, data is null");
            return;
        }
        SaveReferenceFields();
        SaveTotalPlaytime();
        string json = JsonConvert.SerializeObject(data);
        string encryptedJson = EncryptDecrypt(json, encryptionKey);
        WriteToFile(fileName, encryptedJson);
        Debug.Log("Saved");
    }

    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    private bool TryReadFromFile(string fileName, out string json)
    {
        json = "";
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
                return true;
            }
        }
        else
        {
            Debug.LogWarning("File not found");
        }

        return false;
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    public string EncryptDecrypt(string szPlainText, int szEncryptionKey)
    {
        StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
        StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
        char Textch;
        for (int iCount = 0; iCount < szPlainText.Length; iCount++)
        {
            Textch = szInputStringBuild[iCount];
            Textch = (char)(Textch ^ szEncryptionKey);
            szOutStringBuild.Append(Textch);
        }
        return szOutStringBuild.ToString();
    }

#if UNITY_EDITOR
    [MenuItem("Fate/Delete Save Data")]
    private static void DeleteSaveData()
    {
        string persistentDataPath = Application.persistentDataPath;

        // Delete all files and subdirectories in the persistent data path
        if (Directory.Exists(persistentDataPath))
        {
            Directory.Delete(persistentDataPath, true);
            Debug.Log("Persistent data path deleted.");
        }
        else
        {
            Debug.LogWarning("Persistent data path does not exist.");
        }
    }

#endif
}
[System.Serializable]
public class SaveData
{
    public Dictionary<string, int> intFields = new();
    public Dictionary<string, float> floatFields = new();
    public Dictionary<string, string> stringFields = new();
    public Dictionary<string, bool> boolFields = new();
    public Dictionary<string, List<int>> intListFields = new();
    public Dictionary<string, List<float>> floatListFields = new();
    public Dictionary<string, List<string>> stringListFields = new();
    public Dictionary<string, List<bool>> boolListFields = new();
}

[System.Serializable]
public class FateFieldKey
{
    public string fieldName;
    public string key;

    public FateFieldKey(string fieldName, string key)
    {
        this.fieldName = fieldName;
        this.key = key;
    }
}
