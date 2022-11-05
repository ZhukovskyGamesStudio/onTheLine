using UnityEngine;
using UnityEngine.UI;

public class HowToExplainText : MonoBehaviour {
    public static HowToExplainText instance;

    [SerializeField]
    private Text _text;

    private void Awake() {
        instance = this;
    }

    public void ShowText(string text) {
        _text.text = text;
        _text.enabled = true;
    }

    public void Hide() {
        _text.enabled = false;
    }
}