using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehindHole : MonoBehaviour
{
    
    public Dialog betweenRandomDialog;
    public TalkingBubble TalkingBubble;
    public Hole curHole;
    public Person person;
    Call Call;
    Dialog dialog;
    PersonState curState;
    SettingsConfig settings;
    Coroutine coroutine;

    void Start()
    {
        settings = Settings.config;
        curState = PersonState.Out;
    }
    public void StartNewCall(Call call)
    {
        Call = call;
        if (call != null)
            dialog = call.dialog;
        Pick();
    }
    public void Pick()
    {
        if (curState != PersonState.Out)
            return;

        curHole.SetDoorNumber(true);

        curState = PersonState.Picked;

        if (!Settings.config.isWaitingForOperatorHello && curHole.isOpros && Call != null)
            Hear("/hello1/");
        else
            TalkingBubble.Say("Ало", delegate { curHole.PassSound("/picked/"); });

        StartStopWaiting(true);
    }
    public void Hear(string line)
    {
     //   Debug.Log("Hear " + line);
        if (curState == PersonState.Out)
            return;
        StartStopWaiting(false);

        ChooseAnswer(line);
    }
    public void Hear(Dialog _dialog, int lineIndex)
    {
        if (curState == PersonState.Out)
            return;
        lineIndex++;
        dialog = _dialog;

        StartStopWaiting(false);
        TalkingBubble.StopSaying();

        if (_dialog.requirementFrom.roomNumber != curHole.number && _dialog.requirementTo.roomNumber != curHole.number)
            TalkingBubble.Say("Вы не туда попали", delegate { curHole.PassSound("/dialogEnd/"); Drop(); });

        if (_dialog.lines.Count == lineIndex)
            TalkingBubble.Say(". . .", delegate { curHole.PassSound("/dialogEnd/"); Drop(true); });
        else
        {
            curState = PersonState.DialogStarted;
            if (dialog.bubbleLines.Count != 0)
            {
                for (int i = 0; i < dialog.bubbleLines.Count; i++)
                {
                    if (dialog.bubbleLines[i].lineIndex == lineIndex)
                    {
                        //Debug.Log(dialog.bubbleLines[i].bubble);
                        string line = dialog.bubbleLines[i].bubble;
                        TalkingBubble.AddListenedCallback(delegate { CharacterTalking.instance.AddBubble(line); });
                    }                         
                  
                }
            }

            TalkingBubble.Say(_dialog.lines[lineIndex], delegate { curHole.PassSound(_dialog, lineIndex);  });               
        }

       
    }
    void ChooseAnswer(string line)
    {
        if(person != null)
        {
            if (person.HasAnswer(line))
            {
                Debug.Log("answer exists");
                string answer = person.Hear(line);
                TalkingBubble.Say(answer, delegate { StartStopWaiting(true); });
                return;
            }
            else
                Debug.Log("answer not exist");
        }
       

        switch (line)
        {
            case "/hello/":
                if (dialog != null)
                {

                    if (curState == PersonState.DialogStarted)
                    {
                        if (Call != null)
                        {
                            curHole.mistakeEvent.Invoke("Абоненты доложили что разговор подслушан!");
                            Drop();
                            curHole.PassSound("/dialogEnd/");
                            return;
                        }
                    }
                    else if (curState == PersonState.WaitingForConnection)
                    {
                        //добавить реплику повтора для оператора
                    }
                    else
                    {
                        curState = PersonState.WaitingForConnection;
                        TalkingBubble.Say(Call.dialog.SayToOperator, delegate { StartStopWaiting(true); });
                    }
                }
                else
                {
                    //Диалог со случайным номером
                    TalkingBubble.Say("Я вас не вызывала.", delegate { Drop(); });
                }
                return;
            case "/hello1/":
                if (curState == PersonState.DialogStarted || curState == PersonState.WaitingForConnection)
                    return;
                curState = PersonState.WaitingForConnection;
                TalkingBubble.Say(Call.dialog.SayToOperator, delegate { StartStopWaiting(true); });
                return;
            case "/disconnectSound/":
                Debug.Log("curstate " + curState);
                if (curState == PersonState.DialogStarted)
                {
                    curHole.mistakeEvent.Invoke("Абонент отключён от звонка во время разговора!");
                    Drop();
                }
                  
                return;
            case "/dialogEnd/":
                Drop();
                return;
            case "/picked/":
                curState = PersonState.DialogStarted;
                if (Call != null)
                    TalkingBubble.Say(Call.dialog.lines[0], delegate { curHole.PassSound(Call.dialog, 0); });
                else
                    // Здесь должен подставляться диалог между незнакомцами
                    TalkingBubble.Say(betweenRandomDialog.lines[0], delegate { curHole.PassSound(betweenRandomDialog, 0); });
                break;
            default:
                TalkingBubble.Say("Что? Ничего не понятно.", delegate { curHole.PassSound("Что? Ничего не понятно."); });
                break;
        }
    }
    void StartStopWaiting(bool isOn)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        if (isOn)
            coroutine = StartCoroutine(Waiting());
    }

    IEnumerator Waiting()
    {
        float timePass = 0;
        while (timePass < settings.waitForAbonentPick)
        {
            timePass += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        curHole.mistakeEvent.Invoke("Абонент не дождался обслуживания!");
        Drop();
    }

    void Drop(bool isEndenProperly = false)
    {
        if (isEndenProperly)
        {
            SaveManager.AddServedCall();
        }
           
        dialog = null;
        curState = PersonState.Out;
        TalkingBubble.StopSaying();
        curHole.SetDoorNumber(false);
    }
}

enum PersonState
{
    Out = 0,
    Picked,
    DialogStarted,
    WaitingForConnection
}
