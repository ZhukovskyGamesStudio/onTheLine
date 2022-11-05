public class Lamp : InteractableObject {
    public override void OnmouseDown() {
        base.OnmouseDown();
        BlackingOutLevelChanger.instance.StartBlackingOut();
    }

    protected override void OnmouseUp() {
        base.OnmouseUp();
        BlackingOutLevelChanger.instance.Stop();
    }
}