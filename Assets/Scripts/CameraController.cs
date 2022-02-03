using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 minAngles, maxAngles;
    public float maxAngle = 50;
    public float rotationSpeed;
    public Rect blindMiddle;


    [SerializeField] Vector3 curPointerOffset;
    [SerializeField] bool isNearEdge;

    // Start is called before the first frame update
    void Start()
    {
        blindMiddle.y = Screen.height * (1 - blindMiddle.height) / 2;
        blindMiddle.x = Screen.width * (1 - blindMiddle.width) / 2;
        blindMiddle.height *= Screen.height;
        blindMiddle.width *= Screen.width;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousepos = Input.mousePosition;
        Quaternion target = Quaternion.LookRotation(Camera.main.ScreenPointToRay(mousepos).direction);


        float curAngle = Quaternion.Angle(target, Quaternion.Euler(0, 180, 0));
        float angleBetween = Quaternion.Angle(target, transform.rotation);
        if (curAngle < maxAngle && angleBetween > 3)
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * 1);// (maxAngle - curAngle) / maxAngle);

        /*
        isNearEdge = !blindMiddle.Contains(mousepos);
        if (isNearEdge)
        {
            curPointerOffset = mousepos - new Vector2(Screen.width / 2, Screen.height / 2);
            curPointerOffset = curPointerOffset.normalized;
            Vector3 euler = transform.rotation.eulerAngles;

            euler += Vector3.up * curPointerOffset.x;
            euler += Vector3.left * curPointerOffset.y;

            float curAngle = Quaternion.Angle(Quaternion.Euler(euler), Quaternion.Euler(0, 180, 0));
            if (curAngle < 30)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), rotationSpeed * (30 - curAngle) / 30);

        }  */
    }
}
