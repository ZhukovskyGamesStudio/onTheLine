using UnityEngine;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour {
    #region Singleton

    public static SaveManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(this);
        } else {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    #endregion

    private void Start() {
        LoadSave(_profile);
    }

    [SerializeField]
    private Day[] days;

    public static SaveProfile sv;
    public UnityAction OnUnloadScene;

    private int _profile = 1;

    public static void Save() {
        sv.tags = TagManager.Tags;
        JsonUtil.Save(sv, instance._profile);
    }

    public static void StartNewDay() {
        sv.dayResult = new DayResult();
    }

    public static Day GetDay() => instance.days[Mathf.Min(sv.currentDay, instance.days.Length - 1)];

    public static void ChangeMoney(int delta) {
        sv.money += delta;
        if (sv.money < 0)
            sv.money = 0;
    }

    public static SaveProfile LoadSave(int profile) {
        instance._profile = profile;
        sv = JsonUtil.Load(profile);
        TagManager.SetLoadedTags(sv.tags);
        return sv;
    }

    public static int LoadDay() {
        if (!sv.isTrainingComplete)
            return 0;
        return sv.currentDay;
    }

    public static void SetTrainingComplete() {
        Debug.Log("Обучение завершено.");
        sv.isTrainingComplete = true;
    }

    public static void AddServedCall() {
        Debug.Log("Звонок обслужен. Вам начислен бонус.");
        sv.dayResult.callsServed++;
    }

    public static void AddPenalty() {
        Debug.Log("Вам начислен денежный штраф.");
        sv.dayResult.penaltyAmount++;
    }

    private void OnDestroy() {
        OnUnloadScene?.Invoke();
        JsonUtil.Save(sv, _profile);
    }
}