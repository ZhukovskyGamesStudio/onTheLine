using System.Collections.Generic;

[System.Serializable]
public class SaveProfile {
    public int currentDay = 0;
    public int profile;
    public int money;
    public int hunger;
    public bool isTrainingStarted = false;
    public bool isTrainingComplete = false;
    public DayResult dayResult;
    public List<string> tags;
}

[System.Serializable]
public class DayResult {
    public bool isWorkedAllDay;
    public int callsServed;
    public int penaltyAmount;
}