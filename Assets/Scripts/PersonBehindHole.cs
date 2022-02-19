using System.Collections;
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
        dialog = call.dialog;
        Pick();
    }

    //Поднимает телефонную трубку. 
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

    //Слышит фразу не из диалога
    public void Hear(string line)
    {
     
        if (curState == PersonState.Out)
            return;
        StartStopWaiting(false);

        ChooseAnswer(line);
    }

    //Слышит фразу из диалога
    public void Hear(Dialog _dialog, int lineIndex)
    {
        if (curState == PersonState.Out)
            return;
        lineIndex++;
        dialog = _dialog;

        StartStopWaiting(false);
        TalkingBubble.StopSaying();

        if (!dialog.IsCorrectNumber(curHole.number))
            TalkingBubble.Say("Вы не туда попали", delegate { curHole.PassSound("/dialogEnd/"); Drop(); });

        if (dialog.lines.Count == lineIndex)
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
                        string line = dialog.bubbleLines[i].bubble;
                        TalkingBubble.AddListenedCallback(delegate { CharacterTalking.instance.AddBubble(line); });
                    }                         
                  
                }
            }

            TalkingBubble.Say(dialog.lines[lineIndex], delegate { curHole.PassSound(dialog, lineIndex);  });               
        }

       
    }

    //Выбирает ответ на услышанную фразу
    void ChooseAnswer(string line)
    {
        if(person != null)
        {
            if (person.HasAnswer(line))
            {                     
                string answer = person.Hear(line);
                TalkingBubble.Say(answer, delegate { StartStopWaiting(true); });
                return;
            }
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
                if (curState == PersonState.DialogStarted || curState == PersonState.WaitingForConnection || Call == null)
                    return;
                curState = PersonState.WaitingForConnection;
                TalkingBubble.Say(Call.dialog.SayToOperator, delegate { StartStopWaiting(true); });
                return;
            case "/disconnectSound/":
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

    //Запускает или останавливает корутин ожидания
    void StartStopWaiting(bool isOn)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        if (isOn)
            coroutine = StartCoroutine(Waiting());
    }


    //Ожидает столько времени, сколько указано в настройках. Если не дожидается - кладёт трубку и отправляется "ошибка оператора"
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

    //Кладёт трубку. Если разговор закончен корректно - сохраняет обслуженный звонок
    //Обнуляет все значения
    void Drop(bool isEndenProperly = false)
    {
        if (isEndenProperly)
        {
            SaveManager.AddServedCall();
        }
        Call = null;   
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
