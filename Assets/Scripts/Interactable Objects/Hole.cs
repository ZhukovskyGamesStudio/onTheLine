using UnityEngine;
using UnityEngine.Events;

public class Hole : MonoBehaviour
{
    SettingsConfig settings;
    AudioSource audioSource;

    public TalkingBubble TalkingBubble;
    public PersonBehindHole personBehindHole;
    [HideInInspector] public int number;
    [HideInInspector] public UnityEvent<string> mistakeEvent;
    [HideInInspector] public UnityEvent<Call> endOfCall;
    [HideInInspector] public UnityEvent<Call, int> endOfCallWrongNumber;
    [HideInInspector] public UnityEvent<int, bool> turnMyBulb;
    public Shteker curShteker;
    [HideInInspector] public Hole connectedHole;
    [HideInInspector] public DoorNumber DoorNumber;

    public bool isOpros;
    float ringingTime, timeout, timeOutTime;
    void Start()
    {
        timeout = 5f;
        settings = Settings.config;
        audioSource = GetComponent<AudioSource>();
    }

    public void ShtekerIn(Shteker shteker)
    {
        if (curShteker == null)
        {
            curShteker = shteker;
            bool isRight = true;
            TalkingBubble.ChangeSide(isRight);

            if (curShteker.connectedTo.curHole)
            {
                connectedHole = curShteker.connectedTo.curHole;
                connectedHole.connectedHole = this;

                isRight = connectedHole.transform.position.x > transform.position.x;
                TalkingBubble.ChangeSide(isRight);
                curShteker.connectedTo.curHole.TalkingBubble.ChangeSide(!isRight);
            }
            ChangeOpros(shteker.isOpros);
        }

    }
    public void ShtekerOut(Shteker shteker)
    {
        if (shteker == curShteker)
        {
            curShteker = null;

            if (connectedHole)
            {
                connectedHole.personBehindHole.Hear("/disconnectSound/");
                connectedHole.connectedHole = null;
                connectedHole = null;
            }
            ChangeOpros(false);
            personBehindHole.Hear("/disconnectSound/");
        }
    }

    public void NewCall(Call call)
    {
        personBehindHole.StartNewCall(call);
    }

    public void ChangeOpros(bool isOn)
    {
        isOpros = isOn;
        bool isRight = Random.Range(0, 2) == 1;

        if (connectedHole != null)
        {
            isRight = connectedHole.transform.position.x > transform.position.x;
        }

        TalkingBubble.TurnOn(isOn, isRight);
        if (!Settings.config.isWaitingForOperatorHello && isOn)
            Hear("/hello1/");
    }

    public void Hear(string line)
    {
        personBehindHole.Hear(line);
    }

    public void Hear(Dialog dialog, int lineIndex)
    {
        personBehindHole.Hear(dialog, lineIndex);
    }

    public void PassSound(Dialog dialog, int lineIndex)
    {
        if (curShteker && curShteker.connectedTo.curHole)
            curShteker.connectedTo.curHole.Hear(dialog, lineIndex);
    }
    public void PassSound(string line)
    {
        if (curShteker && curShteker.connectedTo.curHole)
            curShteker.connectedTo.curHole.Hear(line);
    }

    public void SetDoorNumber(bool isOn)
    {
        if (isOn)
            DoorNumber.Open();
        else
            DoorNumber.Close();
    }

    void Update()
    {
        CheckRinging();
    }

    void CheckRinging()
    {
        if (curShteker && curShteker.isRinging)
        {
            ringingTime += Time.deltaTime;
        }
        else
            timeOutTime += Time.deltaTime;

        if (timeOutTime >= timeout)
        {
            ringingTime = 0;
            timeOutTime = 0;
        }

        if (ringingTime >= settings.timeRingingBeforePick + Random.Range(-1, 1f))
        {
            ringingTime = 0;
            personBehindHole.Pick();
        }
    }
}
