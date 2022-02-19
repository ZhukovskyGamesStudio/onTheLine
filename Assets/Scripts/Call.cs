[System.Serializable]
public class Call
{
    public int from, to;
    public CallState state;
    public Dialog dialog;

    public Call()
    {

    }

    public Call(int _from, int _to)
    {
        state = CallState.WaitingforOperator;
        from = _from;
        to = _to;
    }
}

public enum CallState
{
    WaitingforOperator = 0,
    TalkingtoOperator,
    WaitingforConnection,
    WaitingforOtherAbonent,
    Talking,
    Finished
}

