using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagManager : MonoBehaviour {
    private static TagManager Instance;
    public List<string> tags;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            tags = new List<string>();
            Clear();
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    public static List<string> Tags => Instance.tags;

    public static void SetLoadedTags(List<string> loadedTags) {
        Instance.tags = loadedTags.Intersect(Instance._overdayTags).ToList();
    }

    public static void AddTag(string newTag) {
        if (!Instance.tags.Contains(newTag)) {
            TryInvokeEventByTag(newTag);
            Instance.tags.Add(newTag);
        }
    }

    public static void RemoveTag(string removeTag) {
        if (removeTag[0] == '!') {
            removeTag = removeTag.Substring(1);
        }

        if (CheckTag(removeTag)) {
            Instance.tags.Remove(removeTag);
        }
    }

    public static bool CheckTag(string toCheck) {
        return Instance.tags.Contains(toCheck);
    }

    public static void Clear() {
        Instance.tags = Instance.tags.Where(tag => Instance._overdayTags.Contains(tag)).ToList();
    }

    public static void ClearAll() {
        Instance.tags = new List<string>();
    }

    private static void TryInvokeEventByTag(string tag) {
        switch (tag) {
            
            case "OpenDoor":
                DoorNumber door4 = GameObject.Find("DoorNumber 4").GetComponent<DoorNumber>();
                door4.Open();
                GameObject.Find("Hole 4").GetComponent<Hole>().enabled = true;
                GameObject.Find("Hole 15").GetComponent<Hole>().enabled = true;
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
                HowToExplainText.instance.ShowText("Перед тем как уйти, не забудьте повесить наушники на крючок");
                break;

            case "EnableExplainText":
                HowToExplainText.instance.ShowText("Выберите фразу из блокнота, чтобы зачитать её вслух");
                break;

            case "To call Police":
                GameObject.Find("Hole 2").GetComponent<Hole>().enabled = true;
                break;

            case "Headphone on":
            case "Number seen":
            case "Plug 1 in":
                if (TrainingManager.Instance != null) {
                    TrainingManager.Instance.StartTooLongWaiting();
                }

                break;

            case "Button pressed":
                GameObject.Find("Hole 15").GetComponent<Hole>().enabled = true;
                if (TrainingManager.Instance != null) {
                    TrainingManager.Instance.StopTooLongWaiting();
                }

                break;
        }
    }

    private readonly List<string> _overdayTags = new() {
        "Attacked",
        "Police called"
    };
}