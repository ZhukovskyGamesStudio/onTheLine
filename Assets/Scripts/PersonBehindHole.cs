using System.Collections;
using System.Linq;
using Levitan;
using UnityEngine;

public class PersonBehindHole : MonoBehaviour {
    public Dialog betweenRandomDialog;
    public TalkingBubble TalkingBubble;
    public Hole curHole;
    public Person person;
    Call Call;
    Dialog _dialog;
    PersonState curState;
    SettingsConfig settings;
    Coroutine coroutine;

    void Start() {
        settings = Settings.config;
        curState = PersonState.Out;
    }

    public void StartNewCall(Call call) {
        Call = call;
        _dialog = call.dialog;
        Pick();
    }

    //Поднимает телефонную трубку. 
    public void Pick() {
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
    public void Hear(string line) {
        if (curState == PersonState.Out)
            return;
        StartStopWaiting(false);

        ChooseAnswer(line);
    }

    //Слышит фразу из диалога
    public void Hear(Dialog dialog, int lineIndex) {
        if (curState == PersonState.Out)
            return;
        lineIndex++;
        _dialog = dialog;

        StartStopWaiting(false);
        TalkingBubble.StopSaying();

        if (!_dialog.IsCorrectNumber(curHole.number))
            TalkingBubble.Say("Вы не туда попали", delegate {
                curHole.PassSound("/dialogEnd/");
                Drop();
            });

        if (_dialog.lines.Count == lineIndex) {
            TalkingBubble.Say(". . .", delegate {
                curHole.PassSound("/dialogEnd/");
                Drop(true);
            });
        } else {
            curState = PersonState.DialogStarted;
            TalkingBubble.Say(AddInformationsToPhraze(_dialog.lines[lineIndex]), delegate { curHole.PassSound(_dialog, lineIndex); });
        }
    }

    //Выбирает ответ на услышанную фразу
    void ChooseAnswer(string line) {
        TransitionData changeDialogTransition =
            _dialog.Transitions.FirstOrDefault(transition => transition.thought == line);
        if (changeDialogTransition != null) {
            _dialog = DialogsManager.instance.GetDialogById(changeDialogTransition.dialog);
            if (_dialog.requirementFrom.roomNumber == curHole.number) {
                TalkingBubble.Say(_dialog.lines[0], delegate { curHole.PassSound(_dialog, 0); });
            }

            return;
        }

        if (person != null) {
            if (person.HasAnswer(line)) {
                string answer = person.Hear(line);
                TalkingBubble.Say(answer, delegate { StartStopWaiting(true); });
                return;
            }
        }

        switch (line) {
            case "/hello/":
                if (_dialog != null) {
                    if (curState == PersonState.DialogStarted) {
                        if (Call != null) {
                            curHole.mistakeEvent.Invoke("Абоненты доложили что разговор подслушан!");
                            Drop();
                            curHole.PassSound("/dialogEnd/");
                            return;
                        }
                    } else if (curState == PersonState.WaitingForConnection) {
                        //добавить реплику повтора для оператора
                    } else {
                        curState = PersonState.WaitingForConnection;
                        TalkingBubble.Say(AddInformationsToPhraze(Call.dialog.SayToOperator), delegate { StartStopWaiting(true); });
                    }
                } else {
                    //Диалог со случайным номером
                    TalkingBubble.Say("Я вас не вызывала.", delegate { Drop(); });
                }

                return;

            case "/hello1/":
                if (curState == PersonState.DialogStarted || curState == PersonState.WaitingForConnection ||
                    Call == null)
                    return;
                curState = PersonState.WaitingForConnection;
                TalkingBubble.Say(AddInformationsToPhraze(Call.dialog.SayToOperator), delegate { StartStopWaiting(true); });
                return;

            case "/repeat/":
                if (_dialog != null && curState == PersonState.WaitingForConnection)
                    return;
                curState = PersonState.WaitingForConnection;
                TalkingBubble.Say(AddInformationsToPhraze(Call.dialog.SayToOperator), delegate { StartStopWaiting(true); });
                return;

            case "/disconnectSound/":
                if (curState == PersonState.DialogStarted) {
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
                    TalkingBubble.Say(betweenRandomDialog.lines[0],
                        delegate { curHole.PassSound(betweenRandomDialog, 0); });
                break;

            default:
                TalkingBubble.Say("Что? Ничего не понятно.",
                    delegate { curHole.PassSound("Что? Ничего не понятно."); });
                break;
        }
    }

    private string AddInformationsToPhraze(string lineToSay) {
        foreach (var informationData in Call.dialog.Informations) {
            if (lineToSay.Contains(informationData.line)) {
                lineToSay = lineToSay.Replace(informationData.line, "$" + informationData.line + "$");
                TalkingBubble.AddListenedCallback(delegate {
                    CharacterTalking.instance.AddBubble(informationData.thought);
                });
            }
        }

        return lineToSay;
    }

    //Запускает или останавливает корутин ожидания
    void StartStopWaiting(bool isOn) {
        if (coroutine != null)
            StopCoroutine(coroutine);

        if (isOn)
            coroutine = StartCoroutine(Waiting());
    }

    //Ожидает столько времени, сколько указано в настройках. Если не дожидается - кладёт трубку и отправляется "ошибка оператора"
    IEnumerator Waiting() {
        float timePass = 0;
        while (timePass < settings.waitForAbonentPick) {
            timePass += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        curHole.mistakeEvent.Invoke("Абонент не дождался обслуживания!");
        Drop();
    }

    //Кладёт трубку. Если разговор закончен корректно - сохраняет обслуженный звонок
    //Обнуляет все значения
    void Drop(bool isEndedProperly = false) {
        if (isEndedProperly) {
            SaveManager.AddServedCall();
            foreach (string tag in _dialog.produceTags) {
                if (tag.StartsWith("!")) {
                    TagManager.RemoveTag(tag);
                } else {
                    TagManager.AddTag(tag);
                }
            }
        }

        Call = null;
        _dialog = null;
        curState = PersonState.Out;
        TalkingBubble.StopSaying();
        curHole.SetDoorNumber(false);
    }
}

enum PersonState {
    Out = 0,
    Picked,
    DialogStarted,
    WaitingForConnection
}