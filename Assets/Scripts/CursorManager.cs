using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;
    public GameObject CenterPoint;
    public MouseLook MouseLook;

    Stack< bool> PreviousState;
    bool CurrentState;

    private void Awake()
    {
        instance = this;
        PreviousState = new Stack<bool>();
        CurrentState = false;
    }

    public static void ChangeState(bool InMenu)
    {
        instance.PreviousState.Push(instance.CurrentState);       
        ChangeVisuals(InMenu);
    }

    static void ChangeVisuals(bool InMenu)
    {
        instance.CurrentState = InMenu;
        instance.MouseLook.FixCamera(InMenu);
        instance.CenterPoint.SetActive(!InMenu);              
        Cursor.visible = InMenu;
        Cursor.lockState = InMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static void BackToPreviousState()
    {
        bool state = instance.PreviousState.Pop();
        ChangeVisuals(state);
    }

}
