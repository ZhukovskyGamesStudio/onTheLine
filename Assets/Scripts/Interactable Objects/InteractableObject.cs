using UnityEngine;


public class InteractableObject : MonoBehaviour
{
    bool _isDragging;

    [HideInInspector]
    public virtual void OnmouseDown()
    {
        _isDragging = true;
    }

    protected virtual void Update()
    {
        if (_isDragging)
        {
            OnmouseDrag();

            if (Input.GetMouseButtonUp(0))
                OnmouseUp();
        }
    }

    protected virtual void OnmouseUp()
    {
        _isDragging = false;
    }

    protected virtual void OnmouseDrag()
    {
    }
}
