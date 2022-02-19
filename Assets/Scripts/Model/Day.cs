using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Day", menuName = "ScriptableObjects/Day", order = 5)]
[System.Serializable]
public class Day : ScriptableObject
{
    public Room[] allRooms;
    public CallsTimeTable CallsTimeTable;
    public List<SheduledEvent> eventsList;
}


[System.Serializable]
public class SheduledEvent
{
    public float time;
    public UnityEvent eventInvokeMethod;
    [CanBeNull] public string tagToAdd;

}
