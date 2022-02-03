using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRope : MonoBehaviour
{
    public Transform rope;
    public Transform point1,point2;
    public FixedJoint[] joints;
    public float maxJOintForce;

    public FixedJoint joint;
    public float curdistance, startingDistance;
    public float curScale, startingScale;
    public float point1startY;
    // Start is called before the first frame update
    void Start()
    {
        if (point2)
        {
            point1startY = point1.position.y;
            startingDistance = point2.position.y;//   (point2.position - point1.position).magnitude;
            startingScale = rope.localScale.y;
        }
          
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // Debug.Log(joint.);
        
        if (point2)
        {
            Vector3 newPose = point2.position;
            newPose.y = startingDistance * (point1.position.y / point1startY);
            point2.position = newPose;
             /*
             curdistance = (point2.position - point1.position).magnitude;
             Vector3 newScale = rope.localScale;
            float multiplier = (curdistance / startingDistance);
            curScale = startingScale * multiplier;
             newScale = Vector3.one *  curScale;
             rope.localScale = newScale;  */
             
        }
       
    }
}
