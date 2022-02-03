[System.Serializable]
public class SaveProfile
{
    public int profile;
    public int money;
    public int hunger;
    public DayResult dayResult;

}
[System.Serializable]
public class DayResult
{
    public bool isWorkedAllDay;
    public int callsServed;
    public int penaltyAmount;
}
