public class VirtualHole : Hole {
    public override void NewCall(Call call) {
        ChangeOpros(true);
        TalkingBubble.TurnOn(true);

        base.NewCall(call);
    }

    public override void SetDoorNumber(bool isOn) {
        isOnLine = isOn;
    }

    public void ClearBubble() {
        TalkingBubble.TurnOn(false);
    }
}