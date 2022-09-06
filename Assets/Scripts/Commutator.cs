using System.Collections.Generic;
using UnityEngine;

public class Commutator : MonoBehaviour
{
    [Min(0)] public int minHoleNumber = 0, maxHoleNumber;

    [Header("Interactive Parts")]
    public Hole[] holes;
    public Hole virtualHole;
    public Shteker[] Shtekers1, Shtekers2;
    public Lever[] Levers;
    public Tumbler[] Tumblers;
    public DoorNumber[] DoorNumbers;
    public Facs Facs;
    public Transform DragWall;
    public static Transform dragWallStatic;

    [HideInInspector] public List<Call> Calls;

    void Awake()
    {
        dragWallStatic = DragWall;
        for (int i = 0; i < holes.Length; i++)
        {
            holes[i].number = i;
            holes[i].DoorNumber = DoorNumbers[i];
            holes[i].mistakeEvent.AddListener(OnPlayerMistake);
            holes[i].endOfCall.AddListener(EndOfCall);
            holes[i].endOfCallWrongNumber.AddListener(EndOfCallWrongNumber);
            holes[i].turnMyBulb.AddListener(TurnBulb);
        }
        virtualHole.number = -1;
        virtualHole.mistakeEvent.AddListener(OnPlayerMistake);
        virtualHole.endOfCall.AddListener(EndOfCall);
        
        for (int i = 0; i < Shtekers1.Length; i++)
        {
            Shtekers1[i].connectedTo = Shtekers2[i];
            Shtekers2[i].connectedTo = Shtekers1[i];
        }


        for (int i = 0; i < Levers.Length; i++)
        {
            Levers[i].Shteker1 = Shtekers1[i];
            Levers[i].Shteker2 = Shtekers2[i];
            Tumblers[i].Shteker1 = Shtekers1[i];
            Tumblers[i].Shteker2 = Shtekers2[i];
        }
    }

    public Call NewCall(Call newCall) {
        if (newCall.from == -1) {
            virtualHole.NewCall(newCall);
        } else {
            //Building.instance.GetNewCall
            TurnBulb(newCall.from - 1, true);
            //bulbs[newCall.from].ChangeState(1);   
            holes[newCall.from].NewCall(newCall);
        }

        Calls.Add(newCall);
        return newCall;
    }

    public void TurnBulb(int number, bool isOn)
    {
        if (isOn)
            DoorNumbers[number].Open();
        else
            DoorNumbers[number].Close();
        //bulbs[number].ChangeState(isOn ? 1 : 0);
    }

    public void EndOfCallWrongNumber(Call call, int wrongHoleNumber)
    {
        Calls.Remove(call);
        if (call.from > 0)
            TurnBulb(call.from - 1, false);
        TurnBulb(wrongHoleNumber - 1, false);
    }

    public void EndOfCall(Call call)
    {
        Calls.Remove(call);
        if (call.from > 0)
            TurnBulb(call.from - 1, false);
        if (call.to > 0 && call.state > CallState.WaitingforConnection)
            TurnBulb(call.to - 1, false);
    }

    public void EndAllCalls()
    {
        for (int i = 0; i < holes.Length; i++)
        {
            holes[i].StopAllCoroutines();
        }
        virtualHole.StopAllCoroutines();
        for (int i = 0; i < Calls.Count; i++)
        {
            EndOfCall(Calls[i]);
        }
        Debug.Log("Ended all calls ");
    }

    public void PassSoundFromOperator(string sound)
    {
        for (int i = 0; i < holes.Length; i++)
        {
            if (holes[i].isOpros)
            {
                holes[i].Hear(sound);
            }
            virtualHole.Hear(sound);
        }
    }
    public void PassSoundFromOperator(Dialog dialog, int lineIndex)
    {
        for (int i = 0; i < holes.Length; i++)
        {
            if (holes[i].isOpros)
                holes[i].Hear(dialog, lineIndex);
        }
    }


    public void OnPlayerMistake(string message)
    {
        Facs.NewPenalty(message + "\n" + message + "!\n" + message.ToUpper() + "!!!\n");
        SaveManager.AddPenalty();
    }
}

