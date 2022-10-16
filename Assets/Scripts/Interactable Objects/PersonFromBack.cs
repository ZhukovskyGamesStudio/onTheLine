public class PersonFromBack : PersonBehindHole
{
    protected override void StartStopWaiting(bool isStart) {
        if(CurState == PersonState.WaitingForConnection && isStart) {
            TalkingBubble.TurnOn(false);
            Drop(true);
        }
    }

    public override void Hear(string line) {
        base.Hear(line);
    }
}
