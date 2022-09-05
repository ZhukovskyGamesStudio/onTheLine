using UnityEngine;

[CreateAssetMenu(fileName = "TimeTable", menuName = "ScriptableObjects/TimeTable", order = 3)]
[System.Serializable]
public class CallsTimeTable : ScriptableObject {
    [Min(0)]
    public float timeBetweenCalls;
    [Min(0)]
    public float randomTimeBetweenCalls;
    [Min(0)]
    public int additionalCallsAmount;
    public QueuePos[] callsTimeTable;
    public Dialog[] randomDialogs;
}
