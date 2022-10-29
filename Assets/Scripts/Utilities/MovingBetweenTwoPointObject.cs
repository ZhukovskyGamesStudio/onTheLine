using UnityEngine;
using UnityEngine.Events;

public class MovingBetweenTwoPointObject : MonoBehaviour {
    public float moveSpeed = 10;
    private Vector3 _takenPosition, _takenRotation;
    public Vector3 TakenPosition, TakenRotation;
    public Mesh meshForGizmos;
    public UnityEvent OnTaken, OnPut;

    Vector3 startPos, takenPos, targetPos;
    Quaternion startRot, takenRot, targetRot;
    bool isTaken;
    bool isMoving;

    private void Start() {
        _takenPosition = TakenPosition;
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        targetPos = startPos;
    }

    private void Update() {
        if (_takenPosition != TakenPosition || _takenRotation != TakenRotation) {
            _takenPosition = TakenPosition;
            _takenRotation = TakenRotation;
            isMoving = true;
        }

        takenPos = startPos + TakenPosition;
        takenRot = startRot * Quaternion.Euler(TakenRotation);
    }

    private void FixedUpdate() {
        if (!isMoving)
            return;

        if (isTaken) {
            targetPos = takenPos;
            targetRot = takenRot;
        } else {
            targetPos = startPos;
            targetRot = startRot;
        }

        bool isClose = Vector3.Distance(transform.localPosition, targetPos) < 0.001f;
        if (!isClose) {
            Vector3 nextPos = Vector3.Lerp(transform.localPosition, targetPos, moveSpeed * Time.fixedDeltaTime);
            transform.localPosition = nextPos;
        }

        bool isCloseRot = Quaternion.Angle(transform.localRotation, targetRot) < 0.1f;
        if (!isCloseRot) {
            Quaternion nextRot = Quaternion.Lerp(transform.localRotation, targetRot, moveSpeed * Time.fixedDeltaTime);
            transform.localRotation = nextRot;
        }

        if (isClose && isCloseRot) {
            isMoving = false;
            if (isTaken)
                OnTaken?.Invoke();
            else
                OnPut?.Invoke();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireMesh(meshForGizmos, -1, transform.position + TakenPosition,
            transform.rotation * Quaternion.Euler(TakenRotation), Vector3.one);
        //Gizmos.DrawWireSphere(transform.localPosition + TakenPosition, 0.1f);
    }

    public void Take() {
        isTaken = true;
        isMoving = true;
        targetPos = takenPos;
        targetRot = takenRot;
    }

    public void Put() {
        isTaken = false;
        isMoving = true;
        targetPos = startPos;
        targetRot = startRot;
    }
}