using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    public float handleSpeed;
    public Transform Phonehandle;
    public Transform itemPos;
    Transform PhoneParent;
    Vector3 startPos;
    Quaternion startRot;
    public Transform PhoneFinPos;
    Vector3 targetPos;
    Quaternion targetRot;
    bool isTaken;

    private void Start()
    {
        PhoneParent = Phonehandle.parent;
        startPos = Phonehandle.position;
        startRot = Phonehandle.rotation;
        targetPos = startPos;
        isTaken = false;
    }

    private void Update()
    {            

        if (Vector3.Distance(Phonehandle.position, targetPos) > 0.05f)
        {
            Phonehandle.position = Vector3.Slerp(Phonehandle.position, targetPos, 0.01f * handleSpeed);
            Phonehandle.rotation = Quaternion.Slerp(Phonehandle.rotation, targetRot, 0.01f * handleSpeed);
        }
           
    }

    public void TakeDrop()
    {
        isTaken = !isTaken;
        targetPos = isTaken ? PhoneFinPos.position : startPos;
        targetRot = isTaken ? PhoneFinPos.rotation : startRot;
        Phonehandle.parent = isTaken ? Camera.main.transform : PhoneParent;
    }

    private void OnMouseDown()
    {
        TakeDrop();
    }
}
