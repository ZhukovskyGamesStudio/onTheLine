using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeTable", menuName = "ScriptableObjects/TimeTable", order = 3)]
[System.Serializable]
public class CallsTimeTable : ScriptableObject
{
    public int additionalCallsAmount;
    public QueuePos[] callsTimeTable;
    public Dialog[] randomDialogs;
}
