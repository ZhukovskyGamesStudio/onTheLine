using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    [Header("Other")]
    public Commutator Commutator;
    public Clock Clock;

    private void Awake()
    {
        Clock.onStartDay += StartDay;
    }

    public void StartDay()
    {
        SaveManager.StartNewDay();
        StartCoroutine(CallsCoroutine());
    }

    public void StopDayImmedeately()
    {
        StopAllCoroutines();
        Commutator.EndAllCalls();
        Clock.EndDay();
    }


    public void NewCall()
    {
        Commutator.NewCall();
    }

    IEnumerator CallsCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(3, 5));
        int callsAmount = DialogsQueue.instance.GetCallsAmount();
        for (int i = 0; i < callsAmount; i++)
        {
            if (Clock.IsWorkTime())
                NewCall();

            float waitTime = Settings.config.minTimeBetweenCalls + Random.Range(0, Settings.config.randomTimeBetweenCalls);
            yield return new WaitForSeconds(waitTime);
        }
        Debug.Log("Звонки закончились");
    }


    private void OnDestroy()
    {
        Clock.onStartDay -= StartDay;
    }
}
