using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableItemBehaviour : InteractableObject
{
    public float moveSpeed = 10;
    public Transform ItemTakenPos;

    Vector3 startPos;
    Quaternion startRot;
    Vector3 targetPos;
    Quaternion targetRot;
    protected bool isTaken;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        targetPos = startPos;
    }

    protected override void Update()
    {
        base.Update();
        if (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.Slerp(transform.position, targetPos, 0.01f * moveSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.01f * moveSpeed);
        }
    }

    [HideInInspector]
    public override void OnmouseDown()
    {
        TakeDrop();
    }


    public void TakeDrop()
    {
        isTaken = !isTaken;
        targetPos = isTaken ? ItemTakenPos.position : startPos;
        targetRot = isTaken ? ItemTakenPos.rotation : startRot;
    }
}
