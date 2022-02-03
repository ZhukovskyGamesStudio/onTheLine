using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsConfig", menuName = "ScriptableObjects/SettingsConfig", order = 3)]
public class SettingsConfig : ScriptableObject
{
    [Header("General")]
    public float SecondsInOneMinute = 2;
    public float arriveHour = 7.5f;
    public float leaveHour = 18.5f;
    public float startDayHour = 8;
    public float endDayHour = 18;
    public float minTimeBetweenCalls = 30;
    public float randomTimeBetweenCalls = 10;
    public bool isInstaExitOnEndOfDay = true;

    [Header("Controls")]
    public bool isAlphaNumericTumblers = true;
    public bool isSpaceAlphaButtons = true;

    [Header("Survival")]
    public int payForDay = 10;
    public int payForCall = 5;
    public int penaltyIncrease = 5;
    public int freePenalties = 2;
    public int foodPrice = 25;

    [Header("Talking")]
    public bool isWaitingForOperatorHello = true;
    public float ringingTimeUntilMistake = 2f;
    public float timeBetweenLetters = 0.033f;
    public float timePauseDoubleDash = 1f;
    public float timeBetweenTwoPeople = 1f;
    public float timeBeforeBubbleDissappears = 5f;
    public float timeRingingBeforePick = 3f;
    public float timeRingingBeforePickDelta = 1f;
    public float waitForOperatorAnswer = 10f;
    public float waitForOperatorRing = 10f;
    public float waitForAbonentPick = 7f;

    [Header("Physics")]
    [Min(0)] public float dragDistance;
    public static float dragDistanceStatic;

    [Header("Graphics")]
    public Color ImportantTextColor;

}
