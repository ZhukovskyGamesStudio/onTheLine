using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public static SaveProfile sv;
    public UnityAction OnUnloadScene;

    public int profile = 1;

    private void Awake()
    {
        instance = this;
        sv = JsonUtil.Load(profile);
    }

    public void Save()
    {
        JsonUtil.Save(sv, profile);
    }

    public static void StartNewDay()
    {
        sv.dayResult = new DayResult();
    }

    public static void ChangeMoney(int delta)
    {
        sv.money += delta;
        if (sv.money < 0)
            sv.money = 0;
    }

    public static void AddServedCall()
    {
        Debug.Log("Звонок обслужен. Вам начислен бонус.");
        sv.dayResult.callsServed++;
    }

    public static void AddPenalty()
    {
        Debug.Log("Вам начислен денежный штраф.");
        sv.dayResult.penaltyAmount++;
    }


    private void OnDestroy()
    {
        if(OnUnloadScene != null)
            OnUnloadScene.Invoke();
        JsonUtil.Save(sv, profile);
    }
}
