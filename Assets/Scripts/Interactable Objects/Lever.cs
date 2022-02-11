using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractableObject
{
    public int alphaNumber = 0;
    public MovingBetweenTwoPointObject mbp;
    AudioSource audioSource;

    [HideInInspector] public Shteker Shteker1, Shteker2;
    [HideInInspector] public bool isRinging;

    SettingsConfig settings;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        settings = Settings.config;
    }

    [HideInInspector]
    public override void OnmouseDown()
    {
        base.OnmouseDown();
        ChangeState(true);
    }

    protected override void OnmouseDrag()
    {
        base.OnmouseDrag();
    }

    protected override void OnmouseUp()
    {
        base.OnmouseUp();
        ChangeState(false);
    }


    public void ChangeState(bool nextState, bool isSilent = false) // 0 - middle, 1 - right, 2 - left
    {
        if (isRinging == nextState)
            return;
        if (!isSilent)
            audioSource.Play();

        isRinging = nextState;

        if (isRinging)
            mbp.Take();
        else
            mbp.Put();
    }

    public void SendChangedSignal(bool isRinging)
    {
        Shteker1.ChangeRingingState(isRinging);
        Shteker2.ChangeRingingState(isRinging);
    }


    protected override void Update()
    {
        base.Update();
        if (settings.isAlphaNumericTumblers)
        {
            if (!(settings.isSpaceAlphaButtons && Input.GetKey(KeyCode.Space)))
            {
                if (Input.GetKeyDown((KeyCode)(48 + alphaNumber)))
                    ChangeState(true);
                if (Input.GetKeyUp((KeyCode)(48 + alphaNumber)))
                    ChangeState(false);
            }
        }
    }
}
