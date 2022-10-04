using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TalkingBubble : MonoBehaviour
{
    public GameObject BubbleRight, BubbleLeft;
    public Text curText, RightText, LeftText;
    public string emptyTextSymbols = " . . . ";

    public UnityEvent onTalkEnds;
    
    Coroutine printingCoroutine;
    UnityEvent callback;
    UnityEvent listenedImportantCallback;
    SettingsConfig settings;
    bool isTalkingNow;
    bool isOn;

    void Start()
    {
        settings = Settings.config;
        callback = new UnityEvent();
        listenedImportantCallback = new UnityEvent();
        curText = RightText;
        isTalkingNow = false;
        RightText.text = emptyTextSymbols;
        LeftText.text = emptyTextSymbols;
    }

    public void Say(string toSay, UnityAction callbackAction)
    {
        callback.RemoveAllListeners();
        if (callbackAction != null) {
            callback.AddListener(callbackAction);
        }
        if (printingCoroutine != null)
            StopCoroutine(printingCoroutine);
        printingCoroutine = StartCoroutine(PrintNumerator(toSay));
    }

    public void AddListenedCallback(UnityAction callbackAction)
    {
        listenedImportantCallback.RemoveAllListeners();
        listenedImportantCallback.AddListener(callbackAction);
    }

    public void StopSaying()
    {
        if (printingCoroutine != null)
            StopCoroutine(printingCoroutine);
        Headphones.PlayStopTalking(gameObject, false);
        curText.text = emptyTextSymbols;
    }

    public void ChangeSide(bool isRight) {
        if (BubbleRight == null) {
            BubbleLeft.SetActive(isOn);
            curText = LeftText;
            return;
        }

        if (BubbleLeft == null) {
            BubbleRight.SetActive(isOn);
            curText = RightText;
            return;
        }

        if (isRight) {
            curText = RightText;
            BubbleLeft.SetActive(false);
            BubbleRight.SetActive(isOn);
        } else {
            curText = LeftText;
            BubbleRight.SetActive(false);
            BubbleLeft.SetActive(isOn);
        }
    }

    public void TurnOn(bool _isOn, bool isRight = false)
    {

        isOn = _isOn;
        ChangeSide(isRight);
        if (_isOn && isTalkingNow)
            Headphones.PlayStopTalking(gameObject, true);
        else
            Headphones.PlayStopTalking(gameObject, false);

        if (curText.text.Contains("<b><color=#") && !curText.text.Contains("></color></b>"))
            curText.text = "<b><color=#" + ColorUtility.ToHtmlStringRGB(Settings.config.ImportantTextColor) + ">. . .</color></b>";
        else
            curText.text = emptyTextSymbols;
        
    }


    IEnumerator PrintNumerator(string toSay)
    {

        curText.text = "";
        isTalkingNow = true;
        if (isOn)
            Headphones.PlayStopTalking(gameObject, true);

        bool isImportant = false;
        bool isListeningImportant = false;

        for (int i = 0; i < toSay.Length; i++)
        {
            if (toSay[i] == '~')
            {
                yield return new WaitForSeconds(settings.timePauseDoubleDash);
                continue;
            }

            if (toSay[i] == '$')
            {
                isImportant = !isImportant;
                if (isImportant)
                {
                    isListeningImportant = isOn;
                    curText.text += "<b><color=#" + ColorUtility.ToHtmlStringRGB(Settings.config.ImportantTextColor) + "></color></b>";
                }
                else
                {
                    if (isListeningImportant)
                        listenedImportantCallback?.Invoke();
                    isListeningImportant = false;
                    listenedImportantCallback.RemoveAllListeners();
                }

                continue;
            }

            if (isImportant)
                curText.text = curText.text.Insert(curText.text.Length - 12, toSay[i].ToString());
            else
                curText.text += toSay[i];
            yield return new WaitForSeconds(settings.timeBetweenLetters);
        }
        isTalkingNow = false;
        Headphones.PlayStopTalking(gameObject, false);
        onTalkEnds?.Invoke();
        yield return new WaitForSeconds(settings.timeBetweenTwoPeople);
        callback?.Invoke();
        yield return new WaitForSeconds(settings.timeBeforeBubbleDissappears);
        curText.text = emptyTextSymbols;
        yield return new WaitForSeconds(settings.timeBeforeBubbleDissappears);
    }
}
