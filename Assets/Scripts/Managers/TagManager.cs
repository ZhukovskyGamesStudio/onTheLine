using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagManager : MonoBehaviour {
    public static TagManager instance;
    public List<string> tags;

    private void Awake() {
        if (instance == null) {
            instance = this;
            tags = new List<string>();

            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
        }
    }

    public static void AddTag(string newTag) {
        if (!instance.tags.Contains(newTag)) {
            TryInvokeEventByTag(newTag);
            instance.tags.Add(newTag);
        }
    }

    public static void RemoveTag(string removeTag) {
        string tag = removeTag.Substring(1);
        if (CheckTag(removeTag))
            instance.tags.Remove(tag);
    }

    public static bool CheckTag(string toCheck) {
        return instance.tags.Contains(toCheck);
    }

    public void Clear() {
        tags = tags.Where(tag => overdayTags.Contains(tag)).ToList();
    }

    private static void TryInvokeEventByTag(string tag) {
        switch (tag) {
            case "OpenDoor":
                DoorNumber door4 = GameObject.Find("DoorNumber 4").GetComponent<DoorNumber>();
                door4.isOpen = true;
                break;

            case "SoundStarted":
                Hole hole4 = GameObject.Find("Hole 4").GetComponent<Hole>();
                Hole hole15 = GameObject.Find("Hole 15").GetComponent<Hole>();
                string isOnline = hole4.isOpros ? "Online" : "Offline";
                hole4.Hear(isOnline);
                hole15.Hear(isOnline);
                Clock.instance.RingClock();
                break;

            case "EndTraining":
                FindObjectOfType<TrainingManager>().Finish();
                break;
        }
    }

    private readonly List<string> overdayTags = new() {
        "Attacked",
        "Police called"
    };
}