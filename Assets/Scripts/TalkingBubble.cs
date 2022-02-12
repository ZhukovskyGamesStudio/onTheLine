using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TalkingBubble : MonoBehaviour
{
    public GameObject BubbleRight, BubbleLeft;
    public Text curText, RightText, LeftText;

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
        RightText.text = " . . . ";
        LeftText.text = " . . . ";
    }

    public void Say(string toSay, UnityAction callbackAction)
    {
        callback.RemoveAllListeners();
        callback.AddListener(callbackAction);
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
        curText.text = " . . . ";
    }

    public void ChangeSide(bool isRight)
    {
        if (isRight)
        {
            curText = RightText;
            BubbleRight.SetActive(isOn);
            BubbleLeft.SetActive(false);
        }
        else
        {
            curText = LeftText;
            BubbleLeft.SetActive(isOn);
            BubbleRight.SetActive(false);
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
            curText.text = " . . . ";
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
        yield return new WaitForSeconds(settings.timeBetweenTwoPeople);
        callback?.Invoke();
        yield return new WaitForSeconds(settings.timeBeforeBubbleDissappears);
        curText.text = " . . . ";
        yield return new WaitForSeconds(settings.timeBeforeBubbleDissappears);
    }
}
