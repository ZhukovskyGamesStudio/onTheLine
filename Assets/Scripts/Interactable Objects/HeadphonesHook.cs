using UnityEngine;

public class HeadphonesHook : InteractableObject {
    [SerializeField]
    private float _timeToEndLevel = 3;

    private float _curHoldingTime;
    private Headphones _headphones;

    [SerializeField]
    private Transform _phonesTargetPos, _movingTransform;

    private bool _isEnded;

    private void Start() {
        Reset();
        _headphones = Headphones.instance;
    }

    protected override void OnmouseDrag() {
        if (_curHoldingTime >= 0) {
            _curHoldingTime -= Time.deltaTime;
            _headphones.MoveBetween(_phonesTargetPos, _movingTransform,
                (_timeToEndLevel - _curHoldingTime) / _timeToEndLevel);
        } else {
            EndLevel();
        }
    }

    protected override void OnmouseUp() {
        base.OnmouseUp();
        if (_isEnded) {
            return;
        }

        Reset();
        _headphones.MoveBetween(_phonesTargetPos, _movingTransform, 0);
    }

    private void EndLevel() {
        _isEnded = true;
        _curHoldingTime = 0;
        _headphones.PutOnHook(_phonesTargetPos);
        BlackingOutLevelChanger.instance.StartBlackingOut();
        SaveManager.sv.currentDay++;
        SaveManager.Save();
    }

    private void Reset() {
        _curHoldingTime = _timeToEndLevel;
    }
}