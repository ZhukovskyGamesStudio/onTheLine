using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogsQueue 
{
    List<QueuePos> timedCallsList;
    List<Dialog> randomCallsList;
    private CallsTimeTable callsTimeTable;

    public DialogsQueue(CallsTimeTable timeTable){
        callsTimeTable = timeTable;
        if (callsTimeTable != null) {
            timedCallsList = new List<QueuePos>(callsTimeTable.callsTimeTable);
            randomCallsList = new List<Dialog>(callsTimeTable.randomDialogs);
        } else {
            timedCallsList = new List<QueuePos>();
            randomCallsList = new List<Dialog>();
        }
    }

    public int GetCallsAmount()
    {
        return DayManager.Day.CallsTimeTable.additionalCallsAmount + DayManager.Day.CallsTimeTable.callsTimeTable.Length;
    }

    public Call GetCall()
    {
        Dialog dialog = GetNextDialog();
        if (dialog == null)
            return null;
        Call call = new Call();
        call.from = dialog.requirementFrom.roomNumber;
        call.to = dialog.requirementTo.roomNumber;
        call.dialog = dialog;
        return call;
    }

    Dialog GetNextDialog()
    {
        float time = Clock.instance.GetTime();
        Dialog dialog = GetTimedDialog(time);

        if (dialog == null)
            dialog = GetRandomDialog();
        if (dialog == null)
            Debug.Log("Нет диалога! Время: " + time);

        return dialog;
    }

    Dialog GetRandomDialog()
    {

        List<Dialog> available = new List<Dialog>();
        int topPriority = 0;
        for (int i = 0; i < randomCallsList.Count; i++)
        {
            if (CheckRequirements(randomCallsList[i]))
            {
                if (randomCallsList[i].priority < topPriority)
                    continue;
                if (randomCallsList[i].priority > topPriority)
                {
                    topPriority = randomCallsList[i].priority;
                    available = new List<Dialog>();
                }
                available.Add(randomCallsList[i]);
            }
        }
        if (available.Count > 0)
        {

            Dialog dialog = available[Random.Range(0, available.Count)];
            randomCallsList.Remove(dialog);
            return dialog;
        }
        return null;

    }

    Dialog GetTimedDialog(float curTime)
    {
        List<QueuePos> available = new List<QueuePos>();
        for (int i = 0; i < timedCallsList.Count; i++)
        {
            if (timedCallsList[i].IsTimeWithinRange(curTime))
            {
                available.Add(timedCallsList[i]);
            }
        }
        if (available.Count > 0)
        {

            QueuePos rnd = available[Random.Range(0, available.Count)];
            timedCallsList.Remove(rnd);
            return rnd.dialog;
        }
        return null;
    }

    bool CheckRequirements(Dialog toCheck)
    {
        if (toCheck.requireTags != null)
            for (int i = 0; i < toCheck.requireTags.Count; i++)
            {
                if (!TagManager.CheckTag(toCheck.requireTags[i]))
                {
                    return false;
                }

            }
        if (toCheck.forbiddenTags != null)
            for (int i = 0; i < toCheck.forbiddenTags.Count; i++)
            {
                if (TagManager.CheckTag(toCheck.forbiddenTags[i]))
                {
                    return false;
                }

            }
        return true;
    }

}

[System.Serializable]
public class QueuePos
{
    public string name;
    public Dialog dialog;
    public float fromTime, toTime;
    public int priority = 0;

    public bool IsTimeWithinRange(float time)
    {

        return fromTime <= time && time <= toTime;
    }
}
