using System;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour {
    public static TagManager instance;
    public List<string> tags;

    private void Awake() {
        instance = this;
        if (tags == null)
            tags = new List<string>();
    }

    public static void AddTag(string newTag) {
        if (!instance.tags.Contains(newTag)) {
            TryInvokeEventByTag(newTag);
            instance.tags.Add(newTag);
        }
    }

    public static void RemoveTag(string removeTag) {
        string tag = (string) Enum.Parse(typeof(string), removeTag.Substring(1));
        if (CheckTag(removeTag))
            instance.tags.Remove(tag);
    }

    public static bool CheckTag(string toCheck) {
        return instance.tags.Contains(toCheck);
    }


    private static void TryInvokeEventByTag(string tag) {
        switch (tag) {
            case "SoundStarted":
                Hole hole4 = GameObject.Find("Hole 4").GetComponent<Hole>();
                Hole hole15 = GameObject.Find("Hole 15").GetComponent<Hole>();
                string isOnline = hole4.isOpros ? "Online" : "Offline";
                hole4.Hear(isOnline);
                hole15.Hear(isOnline);
                break;
        }
    }
}