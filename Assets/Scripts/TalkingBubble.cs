using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TalkingBubble : MonoBehaviour {
    public GameObject BubbleRight, BubbleLeft;
    public Text curText, RightText, LeftText;
    public string emptyTextSymbols = " . . . ";

    public UnityEvent onTalkEnds;

    private Coroutine _printingCoroutine;
    private UnityEvent _callback;
    private UnityEvent _listenedImportantCallback;
    private SettingsConfig _settings;
    private bool _isTalkingNow;
    private bool _isOn;

    private List<char> randomSymbol = new() {
        '@', '#', '!', '~', '$', '%', '^', '&', '*', '(', ')', '_', '-', '=', '+'
    };

    private void Start() {
        _settings = Settings.config;
        _callback = new UnityEvent();
        _listenedImportantCallback = new UnityEvent();
        curText = RightText;
        _isTalkingNow = false;
        RightText.text = emptyTextSymbols;
        LeftText.text = emptyTextSymbols;
    }

    public void Say(string toSay, UnityAction callbackAction) {
        _callback.RemoveAllListeners();
        if (callbackAction != null) {
            _callback.AddListener(callbackAction);
        }

        if (_printingCoroutine != null)
            StopCoroutine(_printingCoroutine);
        _printingCoroutine = StartCoroutine(PrintNumerator(toSay));
    }

    public void AddListenedCallback(UnityAction callbackAction) {
        _listenedImportantCallback.RemoveAllListeners();
        _listenedImportantCallback.AddListener(callbackAction);
    }

    public void StopSaying() {
        if (_printingCoroutine != null)
            StopCoroutine(_printingCoroutine);
        Headphones.PlayStopTalking(gameObject, false);
        _callback.RemoveAllListeners();
        curText.text = emptyTextSymbols;
    }

    public void ChangeSide(bool isRight) {
        if (BubbleRight == null) {
            BubbleLeft.SetActive(_isOn);
            curText = LeftText;
            return;
        }

        if (BubbleLeft == null) {
            BubbleRight.SetActive(_isOn);
            curText = RightText;
            return;
        }

        if (isRight) {
            curText = RightText;
            BubbleLeft.SetActive(false);
            BubbleRight.SetActive(_isOn);
        } else {
            curText = LeftText;
            BubbleRight.SetActive(false);
            BubbleLeft.SetActive(_isOn);
        }
    }

    public void TurnOn(bool isOn, bool isRight = false) {
        _isOn = isOn;
        ChangeSide(isRight);
        if (isOn && _isTalkingNow)
            Headphones.PlayStopTalking(gameObject, true);
        else
            Headphones.PlayStopTalking(gameObject, false);

        if (curText.text.Contains("<b><color=#") && !curText.text.Contains("></color></b>"))
            curText.text = "<b><color=#" + ColorUtility.ToHtmlStringRGB(Settings.config.ImportantTextColor) +
                           ">. . .</color></b>";
        else
            curText.text = emptyTextSymbols;
    }

    private IEnumerator PrintNumerator(string toSay) {
        curText.text = "";
        _isTalkingNow = true;
        if (_isOn)
            Headphones.PlayStopTalking(gameObject, true);

        bool isImportant = false;
        bool isListeningImportant = false;

        for (int i = 0; i < toSay.Length; i++) {
            if (toSay[i] == '~') {
                yield return new WaitForSeconds(_settings.TimePauseDoubleDash);
                continue;
            }

            if (toSay[i] == '@') {
                while (true) {
                    yield return new WaitForSeconds(1);
                }
            }

            if (toSay[i] == '$') {
                isImportant = !isImportant;
                if (isImportant) {
                    isListeningImportant = _isOn;
                    curText.text += "<b><color=#" + ColorUtility.ToHtmlStringRGB(Settings.config.ImportantTextColor) +
                                    "></color></b>";
                } else {
                    if (isListeningImportant)
                        _listenedImportantCallback?.Invoke();
                    isListeningImportant = false;
                    _listenedImportantCallback.RemoveAllListeners();
                }

                continue;
            }

            if (toSay[i] == '{') {
                string tagToAdd = "";
                i++;
                do {
                    tagToAdd += toSay[i];
                    i++;
                } while (toSay[i] != '}');

                TagManager.AddTag(tagToAdd);

                continue;
            }

            if (isImportant)
                curText.text = curText.text.Insert(curText.text.Length - 12, toSay[i].ToString());
            else {
                PrintSymbol(toSay[i]);
            }

            yield return new WaitForSeconds(_settings.TimeBetweenLetters);
        }

        _isTalkingNow = false;
        Headphones.PlayStopTalking(gameObject, false);
        onTalkEnds?.Invoke();
        yield return new WaitForSeconds(_settings.timeBetweenTwoPeople);
        _callback?.Invoke();
        yield return new WaitForSeconds(_settings.timeBeforeBubbleDissappears);
        curText.text = emptyTextSymbols;
    }

    protected virtual void PrintSymbol(char symbol) {
        if (Headphones.IsCanHear || symbol == '\n') {
            curText.text += symbol;
        } else {
            curText.text += randomSymbol[Random.Range(0, randomSymbol.Count)];
        }
    }
}