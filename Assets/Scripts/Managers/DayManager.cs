using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DayManager : MonoBehaviour {
    public static DialogsQueue DialogsQueue;
    DayShedule DayShedule;

    public Day forceThisDay;
    public bool forceTraining;

    public static Day Day;
    public TrainingManager TrainingManager;

    [Header("Other")]
    public Commutator Commutator;

    private Clock Clock;

    void Init() {
        if (forceThisDay != null) {
            Day = forceThisDay;
        } else {
            Day = SaveManager.GetDay();
        }

        if (!SaveManager.sv.isTrainingComplete || forceTraining)
            Instantiate(TrainingManager);
        DialogsQueue = new DialogsQueue(Day.CallsTimeTable);
    }

    private void Start() {
        Init();

        Clock = Clock.instance;
        DayShedule = new DayShedule(Day.eventsList, Clock);
        Clock.onStartDay += StartDay;
    }

    public void StartDay() {
        SaveManager.StartNewDay();
        StartCoroutine(CallsCoroutine());
    }

    private void Update() {
        DayShedule.CheckEvent();
    }

    public void StopDayImmedeately() {
        StopAllCoroutines();
        Commutator.EndAllCalls();
        Clock.EndDay();
    }

    public void NewCall() {
        Call newCall = DialogsQueue.GetCall();
        if (newCall == null) return;
        if (newCall.dialog == null) return;
        Commutator.NewCall(newCall);
    }

    IEnumerator CallsCoroutine() {
        yield return new WaitForSeconds(Random.Range(3, 5));
        while (Clock.IsWorkTime()) {
            NewCall();
            float waitTime = Day.CallsTimeTable.timeBetweenCalls + Day.CallsTimeTable.randomTimeBetweenCalls;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnDestroy() {
        Clock.onStartDay -= StartDay;
    }
}