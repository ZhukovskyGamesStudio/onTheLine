using System;
using System.Collections;
using System.Linq;
using Levitan;
using UnityEngine;
using UnityEngine.Events;

public class PersonBehindHole : MonoBehaviour {
    public Dialog betweenRandomDialog;
    public TalkingBubble TalkingBubble;
    public Hole curHole;
    public Person person;
    //private Call _call;
    private Dialog _dialog;
    protected PersonState _curState;
    SettingsConfig _settings;
    Coroutine _coroutine;

    void Start() {
        _settings = Settings.config;
        _curState = PersonState.Out;
    }

    public void StartNewCall(Call call) {
      
        _dialog = call.dialog;
        Pick();
    }

    //Поднимает телефонную трубку. 
    public virtual void Pick() {
        if (_curState != PersonState.Out)
            return;

        curHole.SetDoorNumber(true);

        _curState = PersonState.Picked;

        if (!Settings.config.isWaitingForOperatorHello && curHole.isOpros && _dialog != null)
            Hear("/hello1/");
        else {

            if (curHole.number == 2-1) { //номер милиции
                _dialog = DayManager.DialogsQueue.PoliceGeneral;
                Hear("/hello1/");
            } else {
                TalkingBubble.Say("Ало", delegate { curHole.PassSound("/picked/"); });
                StartStopWaiting(true);
            }
        }
    }

    //Слышит фразу не из диалога
    public virtual void Hear(string line) {
        if (_curState == PersonState.Out)
            return;
        StartStopWaiting(false);
        TalkingBubble.StopSaying();
        
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

        if (!_dialog.IsCorrectNumber(curHole.number + 1)) {
            Say("Вы не туда попали", delegate {
                curHole.PassSound("/dialogEnd/");
                Drop();
            });
        } else if (_dialog.lines.Count == lineIndex) {
            TransitionData autoNext = _dialog.Transitions.FirstOrDefault(t => t.thought == "/dialogEnd/");
            if (autoNext != null) {
                //авто смена диалога на следующий, если есть тег dialogEnd
                Dialog newxtDialog = DialogsManager.instance.GetDialogById(autoNext.dialog);
                AddEndDialogTags(_dialog);
                if (newxtDialog.requirementFrom.roomNumber == curHole.number + 1) {
                    ChooseAnswer("/dialogEnd/");
                } else {
                    curHole.PassSound("/dialogEnd/");
                }
            } else {
                //окончание диалога, т.к. сказана последняя реплика
                Say(". . .", delegate {
                    curHole.PassSound("/dialogEnd/");
                    Drop(true);
                });
            }
        } else {
            _curState = PersonState.DialogStarted;
            Say(_dialog.lines[lineIndex], delegate { curHole.PassSound(_dialog, lineIndex); });
        }
    }

    //Выбирает ответ на услышанную фразу
    void ChooseAnswer(string line) {
        if (_dialog != null) {
            TransitionData changeDialogTransition =
                _dialog.Transitions.FirstOrDefault(transition => transition.thought == line);
            if (changeDialogTransition != null) {
                _dialog = DialogsManager.instance.GetDialogById(changeDialogTransition.dialog);
                if (_dialog.requirementFrom.roomNumber -1 == curHole.number) {
                    if (!string.IsNullOrEmpty(_dialog.SayToOperator)) {
                        Say(_dialog.SayToOperator, delegate { Drop(true) ; });
                    } else
                        Say(_dialog.lines[0], delegate { curHole.PassSound(_dialog, 0); });
                }

                return;
            }
        }

        if (person != null) {
            if (person.HasAnswer(line)) {
                string answer = person.Hear(line);
                Say(answer, delegate { StartStopWaiting(true); });
                return;
            }
        }

        switch (line) {
            case "/hello/":
                if (_dialog != null) {
                    if (_curState == PersonState.DialogStarted) {
                        if (_dialog != null) {
                            curHole.mistakeEvent.Invoke("Абоненты доложили что разговор подслушан!");
                            Drop();
                            curHole.PassSound("/dialogEnd/");
                            return;
                        }
                    } else if (_curState == PersonState.WaitingForConnection) {
                        //добавить реплику повтора для оператора
                    } else {
                        _curState = PersonState.WaitingForConnection;
                        Say(_dialog.SayToOperator,delegate { StartStopWaiting(true); } );
                    }
                } else {
                    //Диалог со случайным номером
                    Say("Я вас не вызывала.",delegate { Drop(); } );
                }

                return;

            case "/hello1/":
                if (_curState == PersonState.DialogStarted || _curState == PersonState.WaitingForConnection ||
                    _dialog == null)
                    return;
                _curState = PersonState.WaitingForConnection;
                Say(_dialog.SayToOperator, delegate { StartStopWaiting(true); });
                return;

            case "/repeat/":
                if (_dialog != null && _curState == PersonState.WaitingForConnection)
                    Say(_dialog.SayToOperator, delegate { StartStopWaiting(true); });
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
                if (_dialog != null)
                    Say(_dialog.lines[0],delegate { curHole.PassSound(_dialog, 0); } );
                else
                    // Здесь должен подставляться диалог между незнакомцами
                    Say(betweenRandomDialog.lines[0], delegate { curHole.PassSound(betweenRandomDialog, 0); });
                break;

            default:
                const string confusedLine = "Что? Ничего не понятно.";
                Say(confusedLine, delegate { curHole.PassSound(confusedLine); });
                break;
        }
    }

    private void Say(string line, UnityAction action) {
        string informativeLine = AddInformationsToPhraze(line);
        TalkingBubble.Say(informativeLine, action);
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
            AddEndDialogTags(_dialog);
        }
        
        _dialog = null;
        StartStopWaiting(false);
        _curState = PersonState.Out;
        TalkingBubble.StopSaying();
        curHole.SetDoorNumber(false);
    }

    private void AddEndDialogTags(Dialog dialog) {
        foreach (string tag in dialog.produceTags) {
            if (tag.StartsWith("!")) {
                TagManager.RemoveTag(tag);
            } else {
                TagManager.AddTag(tag);
            }
        }
    }
}

public enum PersonState {
    Out = 0,
    Picked,
    DialogStarted,
    WaitingForConnection
}