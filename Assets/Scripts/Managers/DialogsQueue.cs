using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogsQueue 
{
    List<QueuePos> timedCallsList;
    List<Dialog> randomCallsList;
    private CallsTimeTable callsTimeTable;
    private Commutator _commutator;

    public DialogsQueue(CallsTimeTable timeTable,Commutator commutator ){
        callsTimeTable = timeTable;
        _commutator = commutator;
        if (callsTimeTable != null) {
            timedCallsList = new List<QueuePos>(callsTimeTable.callsTimeTable);
            randomCallsList = new List<Dialog>(callsTimeTable.randomDialogs);
        } else {
            timedCallsList = new List<QueuePos>();
            randomCallsList = new List<Dialog>();
        }
    }

    public Dialog PoliceGeneral => callsTimeTable.PoliceGeneral;

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
        if (dialog == null) {
            //Debug.Log("��� �������! �����: " + time);
        }
        return dialog;
    }

    Dialog GetRandomDialog()
    {

        List<Dialog> available = new();
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

    bool CheckRequirements(Dialog toCheck) {
        if (_commutator.IsNumberCurrentlyInCall(toCheck.requirementFrom.roomNumber))
            return false;
        if (toCheck.requireTags != null)
            foreach (string t in toCheck.requireTags) {
                if (!TagManager.CheckTag(t))
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
public class QueuePos {
    public Dialog dialog;
    public float fromTime, toTime;

    public bool IsTimeWithinRange(float time) => fromTime <= time && time <= toTime;
}

[CustomPropertyDrawer(typeof(QueuePos))]
public class QueuePosDrawer : PropertyDrawer {
    private const int dialogSize = 200;
    private const int fromTimeSize = 50;
    private const int toTimeSize = 50;
    private const int spaceSize = 20;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect dialogRect = new Rect(position.x, position.y, dialogSize, position.height);
        Rect fromTimeRect = new Rect(position.x, position.y, fromTimeSize, position.height);
        fromTimeRect.x += dialogSize + spaceSize;
        Rect toTimeRect = new Rect(position.x, position.y, toTimeSize, position.height);
        toTimeRect.x += fromTimeSize + dialogSize + spaceSize * 2;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        
        EditorGUI.PropertyField(dialogRect, property.FindPropertyRelative("dialog"), GUIContent.none);
        EditorGUI.PropertyField(fromTimeRect, property.FindPropertyRelative("fromTime"), GUIContent.none);
        EditorGUI.PropertyField(toTimeRect, property.FindPropertyRelative("toTime"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
    
