using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Levitan;
using UnityEngine;
using UnityEngine.Events;

public class PersonBehindHole : MonoBehaviour {
    public Dialog betweenRandomDialog;
    public TalkingBubble TalkingBubble;
    public Hole curHole;

    public Person person;

    private Dialog _dialog;
    protected PersonState CurState;
    private SettingsConfig _settings;
    private Coroutine _coroutine;

    private readonly List<string> _helloStringList = new() {
        "Алло", ". . .", "Здравствуйте", "Aлло, здравствуйте"
    };

    private readonly string _betweenRandomNumbers = "Видимо ошибка связи.";

    private const string DIALOG_END = "/dialogEnd/";

    [NotNull]
    private const string HELLO_1 = "/hello1/";

    private string RandomHelloString => _helloStringList[Random.Range(0, _helloStringList.Count)];

    void Start() {
        _settings = Settings.config;
        CurState = PersonState.Out;
    }

    public void StartNewCall(Call call) {
        _dialog = call.dialog;
        Pick();
    }

    //Поднимает телефонную трубку. 
    public virtual void Pick() {
        if (CurState != PersonState.Out)
            return;

        curHole.SetDoorNumber(true);

        CurState = PersonState.Picked;

        if (!Settings.config.isWaitingForOperatorHello && curHole.isOpros && _dialog != null)
            Hear(HELLO_1);
        else {
            if (curHole.number == 2 - 1) {
                //номер милиции
                _dialog = DayManager.DialogsQueue.PoliceGeneral;
                Hear(HELLO_1);
            } else {
                TalkingBubble.Say(RandomHelloString, delegate { curHole.PassSound("/picked/"); });
                StartStopWaiting(true);
            }
        }
    }

    //Слышит фразу не из диалога
    public virtual void Hear(string line) {
        if (CurState == PersonState.Out)
            return;
        StartStopWaiting(false);
        ChooseAnswer(line);
    }

    //Слышит фразу из диалога
    public void Hear(Dialog dialog, int lineIndex) {
        if (CurState == PersonState.Out)
            return;
        lineIndex++;
        _dialog = dialog;

        StartStopWaiting(false);
        TalkingBubble.StopSaying();

        if (!_dialog.IsCorrectNumber(curHole.number + 1)) {
            Say("Вы не туда попали", delegate {
                curHole.PassSound(DIALOG_END);
                Drop();
            });
        } else if (_dialog.lines.Count == lineIndex) {
            TransitionData autoNext = _dialog.Transitions.FirstOrDefault(t => t.thought == DIALOG_END);
            if (autoNext != null) {
                //авто смена диалога на следующий, если есть тег dialogEnd
                Dialog nextDialog = DialogsManager.Instance.GetDialogById(autoNext.dialog);
                AddEndDialogTags(_dialog);
                if (nextDialog.requirementFrom.roomNumber == curHole.number + 1) {
                    ChooseAnswer(DIALOG_END);
                } else {
                    curHole.PassSound(DIALOG_END);
                }
            } else {
                //окончание диалога, т.к. сказана последняя реплика
                Say(". . .", delegate {
                    curHole.PassSound(DIALOG_END);
                    Drop(true);
                });
            }
        } else {
            CurState = PersonState.DialogStarted;
            Say(_dialog.lines[lineIndex], delegate { curHole.PassSound(_dialog, lineIndex); });
        }
    }

    //Выбирает ответ на услышанную фразу
    bool ChooseAnswer(string line) {
        if (_dialog != null) {
            TransitionData changeDialogTransition =
                _dialog.Transitions.FirstOrDefault(transition => transition.thought == line);
            if (changeDialogTransition != null) {
                TalkingBubble.StopSaying();
                _dialog = DialogsManager.Instance.GetDialogById(changeDialogTransition.dialog);
                if (_dialog.requirementFrom.roomNumber - 1 == curHole.number) {
                    if (!string.IsNullOrEmpty(_dialog.SayToOperator)) {
                        Say(_dialog.SayToOperator, delegate { Drop(true); });
                    } else
                        Say(_dialog.lines[0], delegate { curHole.PassSound(_dialog, 0); });
                }

                return true;
            }
        }

        if (person != null) {
            if (person.HasAnswer(line)) {
                string answer = person.Hear(line);
                Say(answer, delegate { StartStopWaiting(true); });
                return true;
            }
        }

        switch (line) {
            case "/hello/":
                if (_dialog != null) {
                    if (CurState == PersonState.DialogStarted) {
                        if (_dialog != null) {
                            curHole.mistakeEvent.Invoke("Абоненты доложили что разговор подслушан!");
                            Drop();
                            curHole.PassSound(DIALOG_END);
                            return true;
                        }
                    } else if (CurState == PersonState.WaitingForConnection) {
                        //добавить реплику повтора для оператора
                    } else {
                        CurState = PersonState.WaitingForConnection;
                        Say(_dialog.SayToOperator, delegate { StartStopWaiting(true); });
                    }
                } else {
                    //Диалог со случайным номером
                    Say(_betweenRandomNumbers, delegate { Drop(); });
                }

                return true;

            case HELLO_1:
                if (CurState == PersonState.DialogStarted || CurState == PersonState.WaitingForConnection ||
                    _dialog == null)
                    return false;
                CurState = PersonState.WaitingForConnection;
                Say(_dialog.SayToOperator, delegate { EndOfPhrazeToOperator(); });
                return false;

            case "/repeat/":
                if (_dialog != null && CurState == PersonState.WaitingForConnection) {
                    Say(_dialog.SayToOperator, delegate { StartStopWaiting(true); });
                    return false;
                }

                return false;

            case "/disconnectSound/":
                if (CurState == PersonState.DialogStarted) {
                    curHole.mistakeEvent.Invoke("Абонент отключён от звонка во время разговора!");
                    Drop();
                    return true;
                }

                return false;

            case DIALOG_END:
                Drop();
                return true;

            case "/picked/":
                CurState = PersonState.DialogStarted;
                if (_dialog != null)
                    Say(_dialog.lines[0], delegate { curHole.PassSound(_dialog, 0); });
                else
                    // Здесь должен подставляться диалог между незнакомцами
                    Say(betweenRandomDialog.lines[0], delegate { curHole.PassSound(betweenRandomDialog, 0); });
                return true;

            default:
                // const string confusedLine = "Что? Ничего не понятно.";
                //Say(confusedLine, delegate { curHole.PassSound(confusedLine); });
                return false;
        }
    }

    private void Say(string line, UnityAction action) {
        string informativeLine = AddInformationsToPhraze(line);
        TalkingBubble.StopSaying();
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
                    Notebook.instance.AddLine(informationData.thought);
                });
            }
        }

        return lineToSay;
    }

    //Запускает или останавливает корутин ожидания
    protected virtual void StartStopWaiting(bool isStart) {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        if (isStart) {
            //For now people can wait forever
            //_coroutine = StartCoroutine(Waiting());
        }
    }

    protected virtual void EndOfPhrazeToOperator() {
        if (_dialog.lines.Count == 0) {
            Drop(true);
        } else {
            StartStopWaiting(true);
        }
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
        CurState = PersonState.Out;
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