public class PersonFromBack : PersonBehindHole
{
    protected override void StartStopWaiting(bool isStart) {
        if(_curState == PersonState.WaitingForConnection && isStart)
            Drop(true);
    }
}
