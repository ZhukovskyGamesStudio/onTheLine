using UnityEngine;

public class Shteker : InteractableObject
{
    public float moveSpeed;
    public float rotateSpeed;
    public GameObject ShtekerBody;
    AudioSource audioSource;

    [HideInInspector] public GameObject curHoleGameobject;
    [HideInInspector] public Hole curHole;
    [HideInInspector] public Shteker connectedTo;
    [HideInInspector] public bool isOpros = false;
    [HideInInspector] public bool isRinging = false;

    Vector3 targetPos;
    Quaternion targetRot;
    bool isInField;

    Vector3 finPos;
    Vector3 startingPos;
    Quaternion startingRot;
    Rigidbody rb;
    Transform bodyTransform;

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        bodyTransform = transform;

        startingPos = bodyTransform.position;
        startingRot = bodyTransform.rotation;

        targetPos = startingPos;
        targetRot = startingRot;
    }


    protected override void OnmouseDrag()
    {
        base.OnmouseDrag();
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.transform.CompareTag("ShtekerField"))
        {
            EnterShtekerField(hit.transform);
        }
        else if (curHoleGameobject != null)
        {
            ExitShtekerField(curHoleGameobject.transform);
        }

        targetPos = hit.point + hit.normal * 0;

        if (!isInField)
            targetRot = Quaternion.Euler(-45, 0, -25);

    }

    [HideInInspector]
    public override void OnmouseDown()
    {
        base.OnmouseDown();
        ShtekerBody.layer = LayerMask.NameToLayer("Ignore Raycast");
        Commutator.dragWallStatic.gameObject.SetActive(true);
        if (curHole)
            curHole.ShtekerOut(this);
    }

    protected override void OnmouseUp()
    {
        base.OnmouseUp();
        if (isInField)
        {
            bodyTransform.position = finPos;
            transform.rotation = targetRot;
            targetPos = finPos;
            rb.linearVelocity = Vector3.zero;
            curHole = curHoleGameobject.GetComponent<Hole>();
            curHole.ShtekerIn(this);
        }
        else
        {

            curHole = null;
            targetPos = startingPos;
            targetRot = startingRot;
        }
        ShtekerBody.layer = LayerMask.NameToLayer("Default");
        Commutator.dragWallStatic.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Quaternion.Angle(transform.rotation, targetRot) > 1)
            bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, targetPos) > 0.003f)
        {
            Vector3 posAfterLerping = Vector3.Lerp(bodyTransform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            Vector3 forcalc = posAfterLerping;
            forcalc.y = startingPos.y;
            bodyTransform.position = posAfterLerping;
        }

    }

    void EnterShtekerField(Transform other)
    {
        if (other.GetComponent<Hole>().curShteker != null)
            return;
        curHoleGameobject = other.gameObject;
        isInField = true;
        finPos = other.position + other.up * -0.07f;
        targetRot = Quaternion.Euler(-90, 0, 0);//   other.transform.rotation;// * Quaternion.Euler(180,0,0);//  Quaternion.LookRotation(other.transform.forward,other.transform.up * -1);
    }

    void ExitShtekerField(Transform other)
    {
        //Debug.Log("Shteker out of " + other.name + " field.");
        curHole = null;
        isInField = false;
        targetRot = Quaternion.Euler(-30, 0, -25);
    }

    public void ChangeRingingState(bool isOn)
    {
        isRinging = isOn;
    }

    public void ChangeOprosState(bool isOn)
    {
        isOpros = isOn;
        if (curHole)
            curHole.ChangeOpros(isOn);
    }

}
