using UnityEngine;
using System.IO;

[DefaultExecutionOrder(0)]
public class JSONsaving : MonoBehaviour
{
    // Start is called before the first frame update
    public SaveData _saveData;
    private string path = "";

    void Awake()
    {
        path = SetPaths();

        if (File.Exists(path))
        {
            LoadData();
        }
        else
        {
            CreateData();
            SaveTheData();
        }
    }


    private void CreateData()
    {
        _saveData = new SaveData();
    }

    private string SetPaths()
    {
        string localPath = Application.dataPath + Path.AltDirectorySeparatorChar + "Savedata.json";
        string persistentPath =
            Application.persistentDataPath + Path.AltDirectorySeparatorChar +
            "Savedata.json"; //here is where you actually store the info

        return persistentPath;
    }

    public void SaveTheData()
    {
        string savePath = path;
        // Debug.Log("Saving Data at " + savePath);
        string json = JsonUtility.ToJson(_saveData);
        Debug.Log(json);
        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        _saveData = data;
        Debug.Log(data.ToString());
        if (SaveData.CheckVersionObsolet(_saveData.GetVersion()))
        {
            dumpData();
            LoadData();
        }
    }

    public void dumpData()
    {
        string savePath = path;
        File.Delete(savePath);
    }
}