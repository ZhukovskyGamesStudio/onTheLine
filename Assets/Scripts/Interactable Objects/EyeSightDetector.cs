using UnityEngine;

public class EyeSightDetector : MonoBehaviour {
    public string tagToWait;
    public string tagToAdd;
    private bool _isTryingToAdd = true;

    private void OnBecameVisible() {
        _isTryingToAdd = true;
    }

    private void OnBecameInvisible() {
        _isTryingToAdd = false;
    }

    private void Update() {
        if(!_isTryingToAdd)
            return;
        if (!TagManager.CheckTag(tagToWait) || TagManager.CheckTag(tagToAdd)) {
            return;
        }

        TagManager.AddTag(tagToAdd);
        gameObject.SetActive(false);
        _isTryingToAdd = false;
    }
}