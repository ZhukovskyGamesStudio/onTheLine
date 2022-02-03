using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    bool isTaken;
    MovingBetweenTwoPointObject mbtp;

    private void Awake()
    {
        mbtp = GetComponent<MovingBetweenTwoPointObject>();
    }

    private void OnMouseDown()
    {
        isTaken = !isTaken;
        if (isTaken) 
            mbtp.Take();
        else
            mbtp.Put();
    }

}