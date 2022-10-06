using System;
using System.Collections.Generic;
using Levitan;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/Dialog", order = 1)]
[Serializable]
public class Dialog : ScriptableObject {
    public string Id;
    public PersonShablon requirementFrom, requirementTo;
    public string SayToOperator;
    public List<string> lines;
    public List<BubbleLine> bubbleLines;
    public List<string> requireTags;
    public List<string> produceTags;
    public List<string> forbiddenTags;
    public List<TransitionData> Transitions;
    public List<NewInformationData> Informations;

    public int priority; //Сделать парсинг из текста

    public void Copy(Dialog dialog) {
        requirementFrom = dialog.requirementFrom;
        requirementTo = dialog.requirementTo;
        SayToOperator = dialog.SayToOperator;
        lines = dialog.lines;
        bubbleLines = dialog.bubbleLines;
        requireTags = dialog.requireTags;
        forbiddenTags = dialog.forbiddenTags;
        Transitions = dialog.Transitions;
        Informations = dialog.Informations;
    }

    public void FillGaps(int number, string Name) {
        SayToOperator = string.Format(SayToOperator, number);
        for (int i = 0; i < lines.Count; i++) {
            lines[i] = string.Format(lines[i], number, Name);
        }
    }

    public bool Match(Person from, Person to) {
        return from.CheckShablon(requirementFrom) && to.CheckShablon(requirementTo);
    }

    public bool IsCorrectNumber(int number) {
        return requirementFrom.roomNumber == number || requirementTo.roomNumber == number;
    }
}

[System.Serializable]
public class BubbleLine {
    public int lineIndex;
    public string bubble;
}