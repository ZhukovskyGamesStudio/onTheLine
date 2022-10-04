using System.Collections;
using System.Linq;
using Levitan;
using UnityEngine;

public class PersonBehindHole : MonoBehaviour {
    public Dialog betweenRandomDialog;
    public TalkingBubble TalkingBubble;
    public Hole curHole;
    public Person person;
    private Call _call;
    private Dialog _dialog;
    protected PersonState _curState;
    SettingsConfig _settings;
    Coroutine _coroutine;

    void Start() {
        _settings = Settings.config;
        _curState = PersonState.Out;
    }

    public void StartNewCall(Call call) {
        _call = call;
        _dialog = call.dialog;
        Pick();
    }

    //Поднимает телефонную трубку. 
    public void Pick() {
        if (_curState != PersonState.Out)
            return;

        curHole.SetDoorNumber(true);

        _curState = PersonState.Picked;

        if (!Settings.config.isWaitingForOperatorHello && curHole.isOpros && _call != null)
            Hear("/hello1/");
        else {
            TalkingBubble.Say("Ало", delegate { curHole.PassSound("/picked/"); });
            StartStopWaiting(true);
        }
    }

    //Слышит фразу не из диалога
    public void Hear(string line) {
        if (_curState == PersonState.Out)
            return;
        StartStopWaiting(false);

        ChooseAnswer(line);
    }

    //Слышит фразу из диалога
    public void Hear(Dialog dialog, int lineIndex) {
        if (_curState == PersonState.Out)
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
            _curState = PersonState.DialogStarted;
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
                if (!string.IsNullOrEmpty(_dialog.SayToOperator)) {
                    TalkingBubble.Say(_dialog.SayToOperator, null);
                } else
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
                    if (_curState == PersonState.DialogStarted) {
                        if (_call != null) {
                            curHole.mistakeEvent.Invoke("Абоненты доложили что разговор подслушан!");
                            Drop();
                            curHole.PassSound("/dialogEnd/");
                            return;
                        }
                    } else if (_curState == PersonState.WaitingForConnection) {
                        //добавить реплику повтора для оператора
                    } else {
                        _curState = PersonState.WaitingForConnection;
                        TalkingBubble.Say(AddInformationsToPhraze(_dialog.SayToOperator), delegate { StartStopWaiting(true); });
                    }
                } else {
                    //Диалог со случайным номером
                    TalkingBubble.Say("Я вас не вызывала.", delegate { Drop(); });
                }

                return;

            case "/hello1/":
                if (_curState == PersonState.DialogStarted || _curState == PersonState.WaitingForConnection ||
                    _call == null)
                    return;
                _curState = PersonState.WaitingForConnection;
                TalkingBubble.Say(AddInformationsToPhraze(_dialog.SayToOperator), delegate { StartStopWaiting(true); });
                return;

            case "/repeat/":
                if (_dialog != null && _curState == PersonState.WaitingForConnection)
                    return;
                _curState = PersonState.WaitingForConnection;
                TalkingBubble.Say(AddInformationsToPhraze(_dialog.SayToOperator), delegate { StartStopWaiting(true); });
                return;

            case "/disconnectSound/":
                if (_curState == PersonState.DialogStarted) {
                    curHole.mistakeEvent.Invoke("Абонент отключён от звонка во время разговора!");
                    Drop();
                }

                return;

            case "/dialogEnd/":
                Drop();
                return;

            case "/picked/":
                _curState = PersonState.DialogStarted;
                if (_call != null)
                    TalkingBubble.Say(_dialog.lines[0], delegate { curHole.PassSound(_dialog, 0); });
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
        if (_dialog == null)
            return lineToSay;
        if (_dialog.Informations == null)
            return lineToSay;
        foreach (var informationData in _dialog.Informations) {
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
    protected virtual void StartStopWaiting(bool isStart) {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        if (isStart)
            _coroutine = StartCoroutine(Waiting());
    }

    //Ожидает столько времени, сколько указано в настройках. Если не дожидается - кладёт трубку и отправляется "ошибка оператора"
    IEnumerator Waiting() {
        float timePass = 0;
        while (timePass < _settings.waitForAbonentPick) {
            timePass += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        curHole.mistakeEvent.Invoke("Абонент не дождался обслуживания!");
        Drop();
    }

    //Кладёт трубку. Если разговор закончен корректно - сохраняет обслуженный звонок
    //Обнуляет все значения
    protected void Drop(bool isEndedProperly = false) {
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

        _call = null;
        _dialog = null;
        StartStopWaiting(false);
        _curState = PersonState.Out;
        TalkingBubble.StopSaying();
        curHole.SetDoorNumber(false);
    }
}

public enum PersonState {
    Out = 0,
    Picked,
    DialogStarted,
    WaitingForConnection
}