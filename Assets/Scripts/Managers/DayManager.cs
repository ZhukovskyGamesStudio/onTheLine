using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DayManager : MonoBehaviour {
    public static DialogsQueue DialogsQueue;

    public TrainingManager TrainingManager;

    [Header("Other")]
    public Commutator Commutator;

    private Clock _clock;
    private Day _forceThisDay;
    private Day _currentDay;
    private bool _forceTraining;
    private DayShedule _dayShedule;

    public void SetAdminOverrideDay(Day forceThisDay, bool forceTraining) {
        _forceThisDay = forceThisDay;
        _forceTraining = forceTraining;
    }

    void Init() {
        if (_forceThisDay != null) {
            _currentDay = _forceThisDay;
        } else {
            _currentDay = SaveManager.GetDay();
        }

        if (SaveManager.sv.currentDay == 0)
            Instantiate(TrainingManager);
        DialogsQueue = new DialogsQueue(_currentDay.CallsTimeTable, Commutator);
    }

    private void Start() {
        Init();

        _clock = Clock.instance;
        _dayShedule = new DayShedule(_currentDay.eventsList, _clock);
        _clock.onStartDay += StartDay;
    }

    public void StartDay() {
        SaveManager.StartNewDay();
        StartCoroutine(CallsCoroutine());
    }

    private void Update() {
        _dayShedule.CheckEvent();
    }

    public void StopDayImmedeately() {
        StopAllCoroutines();
        Commutator.EndAllCalls();
        _clock.EndDay();
    }

    public void NewCall() {
        Call newCall = DialogsQueue.GetCall();
        if (newCall == null) return;
        if (newCall.dialog == null) return;
        Commutator.NewCall(newCall);
    }

    IEnumerator CallsCoroutine() {
        yield return new WaitForSeconds(Random.Range(3, 5));
        while (_clock.IsWorkTime()) {
            NewCall();
            float waitTime = _currentDay.CallsTimeTable.timeBetweenCalls +
                             _currentDay.CallsTimeTable.randomTimeBetweenCalls;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnDestroy() {
        _clock.onStartDay -= StartDay;
    }
}