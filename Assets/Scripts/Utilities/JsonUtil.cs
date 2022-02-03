using System.IO;
using UnityEngine;

public class JsonUtil
{
    const string saveName = "saveData";

    static string GetPath(int profile = 1)
    {
        return Application.persistentDataPath + "/" + saveName + profile.ToString()+ ".txt"; // путь к файлу сохранения
    }

    public static void Save(SaveProfile sv,int profile = 1)
    {
        string path = GetPath(profile);

        string tosave = JsonUtility.ToJson(sv);

        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(tosave);
        writer.Close();
    }


    public static SaveProfile Load(int profile = 1)
    {
        string path = GetPath(profile);

        if (!File.Exists(path))
        {
            Debug.LogWarning("File does not exists. Generating new saveData");
            return GenerateNewSaveData();
        }
        StreamReader reader = new StreamReader(path);
        string savedData = reader.ReadToEnd();
        reader.Close();
        try
        {
            SaveProfile sv = JsonUtility.FromJson<SaveProfile>(savedData);
            return sv;
        }
        catch
        {
            Debug.LogError("Json parse failed.");
            return GenerateNewSaveData();
        }
        
    }


    static SaveProfile GenerateNewSaveData()
    {
        SaveProfile sv = new SaveProfile();
        sv.dayResult = new DayResult();
        sv.money = 30;
        sv.hunger = 0;
        Save(sv);
        return sv;
    }
}
