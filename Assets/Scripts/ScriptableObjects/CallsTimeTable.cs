using UnityEngine;

[CreateAssetMenu(fileName = "TimeTable", menuName = "ScriptableObjects/TimeTable", order = 3)]
[System.Serializable]
public class CallsTimeTable : ScriptableObject {
    [Min(0)]
    public float timeBetweenCalls;
    [Min(0)]
    public float randomTimeBetweenCalls;
    public QueuePos[] callsTimeTable;
    public Dialog[] randomDialogs;

    public Dialog PoliceGeneral;
}
