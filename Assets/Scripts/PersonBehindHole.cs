using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Levitan;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PersonBehindHole : MonoBehaviour {
    public Dialog betweenRandomDialog;
    public TalkingBubble TalkingBubble;
    public Hole curHole;

    public Person person;

    private Dialog _dialog;
    protected PersonState CurState;
    protected WaitingReason CurWaitingReason;
    private SettingsConfig _settings;
    private Coroutine _waitingCoroutine;
    private Coroutine _listeningCoroutine;
    private Coroutine _speakingCoroutine;

    private readonly List<string> _helloStringList = new() {
        "Алло", ". . .", "Здравствуйте", "Aлло, здравствуйте"
    };

    private readonly string _betweenRandomNumbers = "Видимо ошибка связи.";

    public const int POLICE_NUMBER = 2;

    public const string DIALOG_END = "/dialogEnd/";
    public const string SPEAKING_SOUND = "/SPEAKING_SOUND/";

    [NotNull]
    public const string HELLO_1 = "/hello1/";

    public const string OPERATOR_HELLO = "/OPERATOR_HELLO/";
    public const string OPERATOR_CONNECTION_OK = "/OPERATOR_CONNECTION_OK/";

    public const string PERSON_PAUSED = ".~ ?~ .~";
    public const string PERSON_WAITING = "Долго ещё?";
    public const string PERSON_RESTORE_CONNECTION = "Ты прерывался, повтори";
    public const string PERSON_ANGRY = "Я очень недоволен таким обслуживанием!";

    private float LISTENING_COOLDOWN = 1f;
    private float SPEAKING_COOLDOWN = 0.5f;
    private float WAITING_TIME = 15f;
    private const int WAITING_STRESS = 35;
    private const int PAUSED_STRESS = 35;

    private int _currentStress = 0;
    private int _dialogLineIndex = 0;

    private const int MAXSTRESS = 100;

    private string RandomHelloString => _helloStringList[Random.Range(0, _helloStringList.Count)];

    private int OtherNumber => (curHole.Number == _dialog.requirementFrom.roomNumber
        ? _dialog.requirementTo.roomNumber
        : _dialog.requirementFrom.roomNumber);

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
        _currentStress = 0;
        CurState = PersonState.Waiting;
        CurWaitingReason = WaitingReason.OperatorHello;

        //номер милиции

        if (curHole.Number == POLICE_NUMBER) {
            _dialog = DayManager.DialogsQueue.PoliceGeneral;
            Hear(OPERATOR_HELLO);
            return;
        }

        if (!Settings.config.isWaitingForOperatorHello && curHole.isOpros && _dialog != null) {
            Hear(OPERATOR_HELLO);
            return;
        }

        TalkingBubble.Say(RandomHelloString, delegate { StartStopWaiting(true); });
    }

    //Слышит фразу не из диалога
    public virtual void Hear(string line) {
        if (CurState == PersonState.Out)
            return;

        ChooseAnswer(line);
    }

    //Слышит фразу из диалога
    public void Hear(Dialog dialog, int lineIndex) {
        if (CurState == PersonState.Out)
            return;
        lineIndex++;
        _dialogLineIndex = lineIndex;
        _dialog = dialog;

        //StartStopWaiting(false);
        TalkingBubble.StopSaying();

        if (!_dialog.IsCorrectNumber(curHole.Number)) {
            Say("Вы не туда попали", delegate {
                curHole.PassSound(DIALOG_END);
                Drop();
            });
        } else if (_dialog.lines.Count == lineIndex) {
            EndDialogByLastLine();
        } else {
            SayDialogLine(lineIndex);
        }
    }

    private void EndDialogByLastLine() {
        TransitionData autoNext = _dialog.Transitions.FirstOrDefault(t => t.thought == DIALOG_END);
        if (autoNext != null) {
            //авто смена диалога на следующий, если есть тег dialogEnd
            Dialog nextDialog = DialogsManager.Instance.GetDialogById(autoNext.dialog);
            AddEndDialogTags(_dialog);
            if (nextDialog.requirementFrom.roomNumber == curHole.Number) {
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
    }

    private void SayDialogLine(int lineIndex) {
        StartStopWaiting(false);
        StartStopListening(false);
        CurState = PersonState.Speaking;
        CurWaitingReason = WaitingReason.None;
        //TODO Possible dialog to operator, no waiting for her answer

        Say(_dialog.lines[lineIndex], delegate {
            curHole.PassSound(_dialog, lineIndex);
            CurState = PersonState.Waiting;
            CurWaitingReason = WaitingReason.WaitingForOtherStartSpeak;
            StartStopWaiting(true);
            //TODO If dialog ended - next state should be OUT
        });
    }

    //Выбирает ответ на услышанную фразу
    void ChooseAnswer(string line) {
        if (_dialog != null) {
            TransitionData changeDialogTransition =
                _dialog.Transitions.FirstOrDefault(transition => transition.thought == line);
            if (changeDialogTransition != null) {
                TalkingBubble.StopSaying();
                _dialog = DialogsManager.Instance.GetDialogById(changeDialogTransition.dialog);
                if (_dialog.requirementFrom.roomNumber == curHole.Number) {
                    if (!string.IsNullOrEmpty(_dialog.SayToOperator)) {
                        Say(_dialog.SayToOperator, delegate { Drop(true); });
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

        switch (CurState) {
            case PersonState.Out:
                return;

            case PersonState.Waiting:
                switch (CurWaitingReason) {
                    case WaitingReason.None:
                        break;

                    case WaitingReason.OperatorHello:
                        if (line == OPERATOR_HELLO) {
                            SayLineToOperator();
                        }

                        break;

                    case WaitingReason.Connection:
                        if (line == OPERATOR_CONNECTION_OK) {
                            SayDialogLine(0);
                        }

                        break;

                    case WaitingReason.WaitingForRestoreConnection:
                        if (line == PERSON_RESTORE_CONNECTION) {
                            _currentStress += PAUSED_STRESS;
                            Say(PERSON_PAUSED, delegate { SayDialogLine(_dialogLineIndex); });
                        } else if (line == SPEAKING_SOUND) {
                            if (curHole.connectedHole.Number == OtherNumber) {
                                SayRestoreConnectionLine();
                            } else {
                                SayAngryLine();
                            }
                        }

                        break;
                    case  WaitingReason.WaitingForOtherStartSpeak:
                        if (line == PERSON_RESTORE_CONNECTION) {
                            _currentStress += PAUSED_STRESS;
                            Say(PERSON_PAUSED, delegate { SayDialogLine(_dialogLineIndex); });
                        }else if (line == PERSON_WAITING) {
                            Say(PERSON_PAUSED, delegate {
                                CurWaitingReason = WaitingReason.WaitingForRestoreConnection;
                                StartStopWaiting(true);
                            });
                        } else if (line == SPEAKING_SOUND) {
                            StartStopWaiting(false);
                            CurState = PersonState.Listening;
                            StartStopListening(true);
                        }

                        break;
                }

                break;

            case PersonState.Speaking:
                if (line == PERSON_RESTORE_CONNECTION) {
                    _currentStress += PAUSED_STRESS;
                    Say(PERSON_PAUSED, delegate { SayDialogLine(_dialogLineIndex); });
                }

                break;

            case PersonState.Listening:
                if (line == SPEAKING_SOUND) {
                    StartStopListening(true);
                }

                if (line == PERSON_RESTORE_CONNECTION) {
                    _currentStress += PAUSED_STRESS;
                    Say(PERSON_PAUSED, delegate { SayDialogLine(_dialogLineIndex); });
                }

                break;
        }

        switch (line) {
            /*case "/hello/":
                if (_dialog != null) {
                    if (CurState == PersonState.DialogStarted) {
                        if (_dialog != null) {
                            curHole.mistakeEvent.Invoke("Абоненты доложили что разговор подслушан!");
                            Drop();
                            curHole.PassSound(DIALOG_END);
                            return;
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

                return;*/

            /*
            case HELLO_1:
                if (CurState == PersonState.DialogStarted || CurState == PersonState.WaitingForConnection ||
                    _dialog == null)
                    return;
                CurState = PersonState.WaitingForConnection;
                Say(_dialog.SayToOperator, delegate { EndOfPhrazeToOperator(); });
                return;*/

            case "/repeat/":
                if (_dialog != null && CurState == PersonState.WaitingForConnection) {
                    Say(_dialog.SayToOperator, delegate { StartStopWaiting(true); });
                    return;
                }

                return;

            /*
            case "/disconnectSound/":
                if (CurState == PersonState.DialogStarted) {
                    curHole.mistakeEvent.Invoke("Абонент отключён от звонка во время разговора!");
                    Drop();
                    return;
                }

                return;*/

            case DIALOG_END:
                Drop();
                return;

            /*
            case "/picked/":
                CurState = PersonState.DialogStarted;
                if (_dialog != null)
                    Say(_dialog.lines[0], delegate { curHole.PassSound(_dialog, 0); });
                else
                    // Здесь должен подставляться диалог между незнакомцами
                    Say(betweenRandomDialog.lines[0], delegate { curHole.PassSound(betweenRandomDialog, 0); });
                return;*/

            default:
                // const string confusedLine = "Что? Ничего не понятно.";
                //Say(confusedLine, delegate { curHole.PassSound(confusedLine); });
                return;
        }
    }

    private void SayWaitingLine() {
        Say(PERSON_WAITING, delegate { StartStopWaiting(true); });
    }

    private void SayAngryLine() {
        Say(PERSON_ANGRY, delegate { Drop(); });
    }

    private void SayRestoreConnectionLine() {
       
        Say(PERSON_RESTORE_CONNECTION, delegate {
            StartStopWaiting(false);
            CurState = PersonState.Waiting;
            CurWaitingReason = WaitingReason.WaitingForOtherStartSpeak;
            StartStopWaiting(true);
        });
    }

    private void SayLineToOperator() {
        if (_dialog.lines.Count > 0) {
            Say(_dialog.SayToOperator, delegate {
                StartStopWaiting(true);
                CurWaitingReason = WaitingReason.Connection;
            });
        } else {
            Say(_dialog.SayToOperator, delegate {
                if (true) {
                    Drop(true);
                } else {
                    StartStopWaiting(true);
                    //TODO ADD Dialog choose if dialogs waits for answer or not
                    CurWaitingReason = WaitingReason.OperatorAnswer;
                }
            });
        }
    }

    private void Say(string line, UnityAction action) {
        StartStopWaiting(false);
        StartStopSpeaking(true);
        string informativeLine = AddInformationsToPhraze(line);
        TalkingBubble.StopSaying();
        TalkingBubble.Say(informativeLine, delegate {
            action?.Invoke();
            StartStopSpeaking(false);
        });
    }

    private string AddInformationsToPhraze(string lineToSay) {
        if (_dialog == null)
            return lineToSay;
        if (_dialog.Informations == null)
            return lineToSay;
        foreach (var informationData in _dialog.Informations) {
            if (lineToSay.Contains(informationData.line)) {
                lineToSay = lineToSay.Replace(informationData.line, "$" + informationData.line + "$");
                TalkingBubble.AddListenedCallback(delegate { Notebook.instance.AddLine(informationData.thought); });
            }
        }

        return lineToSay;
    }

    //Запускает или останавливает корутин ожидания
    protected virtual void StartStopWaiting(bool isStart) {
        if (_waitingCoroutine != null)
            StopCoroutine(_waitingCoroutine);

        if (isStart) {
            _waitingCoroutine = StartCoroutine(Waiting());
        }
    }

    protected virtual void StartStopListening(bool isStart) {
        if (_listeningCoroutine != null)
            StopCoroutine(_listeningCoroutine);

        if (isStart) {
            _listeningCoroutine = StartCoroutine(Listening());
        }
    }

    protected virtual void StartStopSpeaking(bool isStart) {
        if (_speakingCoroutine != null)
            StopCoroutine(_speakingCoroutine);

        if (isStart) {
            _speakingCoroutine = StartCoroutine(Speaking());
        }
    }

    private IEnumerator Speaking() {
        while (true) {
            yield return new WaitForSeconds(SPEAKING_COOLDOWN);
            curHole.PassSound(SPEAKING_SOUND);
        }
        //curHole.mistakeEvent.Invoke("Абонент не дождался обслуживания!");
    }

    //Ожидает столько времени, сколько указано в настройках. Если не дожидается - кладёт трубку и отправляется "ошибка оператора"
    private IEnumerator Waiting() {
        yield return new WaitForSeconds(WAITING_TIME);
        _currentStress += WAITING_STRESS;
        if (_currentStress < MAXSTRESS) {
            SayWaitingLine();
        } else {
            SayAngryLine();
        }
        //curHole.mistakeEvent.Invoke("Абонент не дождался обслуживания!");
    }

    private IEnumerator Listening() {
        yield return new WaitForSeconds(LISTENING_COOLDOWN);
        CurState = PersonState.Waiting;
        CurWaitingReason = WaitingReason.WaitingForRestoreConnection;
        StartStopWaiting(true);
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
    DialogStarted,
    WaitingForConnection,
    Waiting,
    Speaking,
    Listening
}

public enum WaitingReason {
    None = -1,
    OperatorHello = 0,
    Connection,
    OperatorAnswer,
    WaitingForOtherStartSpeak,
    WaitingForRestoreConnection,
}