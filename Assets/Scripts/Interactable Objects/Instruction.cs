using UnityEngine;

public class Instruction : InteractableObject
{
    bool isTaken;
    MovingBetweenTwoPointObject mbtp;

    private void Awake()
    {
        mbtp = GetComponent<MovingBetweenTwoPointObject>();
    }
    [HideInInspector]
    public override void OnmouseDown()
    {
        isTaken = !isTaken;
        if (isTaken) 
            mbtp.Take();
        else
            mbtp.Put();
    }

}