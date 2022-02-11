using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Denunciation : TableItemBehaviour
{
    public GameObject DenunciationPanel;

    [HideInInspector]
    public override void OnmouseDown()
    {
        ShowClose();
    }


    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.End) && DenunciationPanel.activeSelf)
            DenunciationPanel.SetActive(false);

    }
    public void ShowClose()
    {
        isTaken = !isTaken;
        DenunciationPanel.SetActive(isTaken);
        Cursor.visible = isTaken;
        if (isTaken)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}
