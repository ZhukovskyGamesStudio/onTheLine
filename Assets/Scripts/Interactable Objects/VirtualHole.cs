using System.Collections;
using UnityEngine;

public class VirtualHole : Hole {

    public float timeBeforeBubbleClears = 5f;
    private Coroutine _clearBubble;
  
    public override void NewCall(Call call) {
        ChangeOpros(true);
        TalkingBubble.TurnOn(true);
        if (_clearBubble != null) {
            StopCoroutine(_clearBubble);
        }
        base.NewCall(call);
    }

    public override void SetDoorNumber(bool isOn) {
        isOnLine = isOn;
    }
    

    public void ClearBubble() {
        if (_clearBubble != null) {
            StopCoroutine(_clearBubble);
        }
        _clearBubble = StartCoroutine(ClearBubbleCoroutine());
    }

    private IEnumerator ClearBubbleCoroutine() {
        yield return new WaitForSeconds(timeBeforeBubbleClears);
        TalkingBubble.TurnOn(false);
    }
}