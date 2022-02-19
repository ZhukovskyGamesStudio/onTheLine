using UnityEngine;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour
{
    #region Singleton

    public static SaveManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(instance);
            sv = JsonUtil.Load(_profile);
        }
    }

    #endregion

    [SerializeField]
    private Day[] days;
    public static SaveProfile sv;
    public UnityAction OnUnloadScene;

    private int _profile = 1;
    

    public static  void Save()
    {
        JsonUtil.Save(sv, instance._profile);
    }

    public static void StartNewDay()
    {
        sv.dayResult = new DayResult();
    }

    public static Day GetDay() {
        return instance.days[sv.currentDay];
    }
    
    public static void ChangeMoney(int delta)
    {
        sv.money += delta;
        if (sv.money < 0)
            sv.money = 0;
    }

    public static SaveProfile LoadSave(int profile) {
        instance._profile = profile;
        sv = JsonUtil.Load(profile);
        return sv;
    }
    
    public static int LoadDay(){
        if (!sv.isTrainingComplete)
            return 0;
        return sv.currentDay;
    }
    
    public static void SetTrainingComplete()
    {
        Debug.Log("Обучение завершено.");
        sv.isTrainingComplete = true;
    }

    public static void AddServedCall()
    {
        Debug.Log("Звонок обслужен. Вам начислен бонус.");
        sv.dayResult.callsServed++;
    }

    public static void AddPenalty()
    {
        Debug.Log("Вам начислен денежный штраф.");
        sv.dayResult.penaltyAmount++;
    }


    private void OnDestroy()
    {
        OnUnloadScene?.Invoke();
        JsonUtil.Save(sv, _profile);
    }
}
