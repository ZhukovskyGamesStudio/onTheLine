using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plug : MonoBehaviour
{
    [Range(0.1f, 15f)]
    public float holdDistance;
    [Range(00f, 1)]
    public float slerpPercent;

    bool isDragging;

    private void OnMouseDrag()
    {
        isDragging = true;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
        Vector3 finPos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(holdDistance);

        transform.position = Vector3.Slerp(transform.position, finPos, slerpPercent);
        gameObject.layer = 2;
    }


    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            gameObject.layer = 0;
        }
    }
}
