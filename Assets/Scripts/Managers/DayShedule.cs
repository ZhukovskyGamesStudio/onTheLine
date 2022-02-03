using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayShedule : MonoBehaviour
{
    public Clock Clock;
    public List<SheduledEvent> eventsList;

    private void Update()
    {
        if (eventsList != null)
            if (eventsList.Count > 0)
            {
                float time = Clock.GetTime();
                if (time > eventsList[0].time)
                {
                    eventsList[0].eventInvokeMethod.Invoke();
                    eventsList.RemoveAt(0);
                }                       
            }
    }
    public void AddTag(Tags newTag)
    {
        Debug.Log("Added tag " + newTag);
        TagManager.AddTag(newTag);
    }

}
   [System.Serializable]
public class SheduledEvent
{
    public float time;
    public UnityEvent eventInvokeMethod;


}
