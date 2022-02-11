using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tumbler : InteractableObject
{
    public int alphaNumber;
    public MovingBetweenTwoPointObject mbp;

    [HideInInspector] public Shteker Shteker1, Shteker2;

    bool isPressed;
    SettingsConfig settings;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        settings = Settings.config;
    }

    [HideInInspector]
    public override void OnmouseDown()
    {
        base.OnmouseDown();
        isPressed = !isPressed;
        MoveButtonHead();
    }


    void MoveButtonHead(bool isSilent = false) //  0 - right, 1 - left
    {
        if (!isSilent)
            audioSource.Play();
        if (isPressed)
            mbp.Take();
        else
            mbp.Put();
    }

    public void SendChangedSignal(bool isOpros)
    {
        Shteker1.ChangeOprosState(isOpros);
        Shteker2.ChangeOprosState(isOpros);
    }


    protected override void  Update()
    {
        base.Update();
        if (settings.isSpaceAlphaButtons)
        {
            if (Input.GetKey(KeyCode.Space))
                if (Input.GetKeyDown((KeyCode)(48 + alphaNumber)))
                {
                    isPressed = !isPressed;
                    MoveButtonHead();
                }
        }
    }
}
