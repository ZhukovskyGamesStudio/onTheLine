using UnityEngine;

public class TableItemBehaviour : InteractableObject {
    public float moveSpeed = 10;
    public Transform ItemTakenPos;

    Vector3 startPos;
    Quaternion startRot;
    Vector3 targetPos => isTaken ? ItemTakenPos.position : startPos;
    Quaternion targetRot => isTaken ? ItemTakenPos.rotation : startRot;
    protected bool isTaken;

    protected virtual void Awake() {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    protected override void Update() {
        base.Update();
        if (Vector3.Distance(transform.position, targetPos) > 0.05f) {
            transform.position = Vector3.Slerp(transform.position, targetPos, 0.01f * moveSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.01f * moveSpeed);
        }
    }

    [HideInInspector]
    public override void OnmouseDown() {
        TakeDrop();
    }

    private void TakeDrop() {
        isTaken = !isTaken;
    }
}