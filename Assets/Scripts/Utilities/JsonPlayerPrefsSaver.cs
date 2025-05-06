using System.IO;
using UnityEngine;

public static class JsonPlayerPrefsSaver {
    public static void Save(SaveProfile sv, int profile = 1) {
        string saveJson = JsonUtility.ToJson(sv);
        PlayerPrefs.SetString($"profile_{profile}", saveJson);
    }

    public static SaveProfile Load(int profile = 1) {
        string path = $"profile_{profile}";

        if (!PlayerPrefs.HasKey(path)) {
            return CreateNewSave();
        }

        string json = PlayerPrefs.GetString(path);
        try {
            SaveProfile sv = JsonUtility.FromJson<SaveProfile>(json);
            return sv;
        } catch {
            Debug.LogError("Json parse failed.");
            return CreateNewSave();
        }
    }

    static SaveProfile CreateNewSave() {
        SaveProfile sv = new() {
            dayResult = new DayResult(),
            money = 30,
            hunger = 0
        };
        Save(sv);
        return sv;
    }
}