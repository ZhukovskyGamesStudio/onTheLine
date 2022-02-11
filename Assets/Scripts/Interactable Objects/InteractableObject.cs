using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractableObject : MonoBehaviour
{
    bool isDragging;

    [HideInInspector]
    public virtual void OnmouseDown()
    {
        isDragging = true;
    }

    protected virtual void Update()
    {
        if (isDragging)
        {
            OnmouseDrag();

            if (Input.GetMouseButtonUp(0))
                OnmouseUp();
        }
    }

    protected virtual void OnmouseUp()
    {
        isDragging = false;
    }

    protected virtual void OnmouseDrag()
    {
    }
}
