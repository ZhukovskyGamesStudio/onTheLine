using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableItemBehaviour : MonoBehaviour
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

    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.Slerp(transform.position, targetPos, 0.01f * moveSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.01f * moveSpeed);
        }
    }


    protected virtual void OnMouseDown()
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
