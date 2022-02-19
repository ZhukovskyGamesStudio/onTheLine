using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowableObject : InteractableObject {
    [SerializeField]
    private bool isTakenOnAwake;
    private bool _isTaken;
    MovingBetweenTwoPointObject _mbp;
    
    private void Awake()
    {
        _mbp = GetComponent<MovingBetweenTwoPointObject>();
        if (isTakenOnAwake) {
            _isTaken = true;
            _mbp.Take();
        }
    }
    
    
    [HideInInspector]
    public override void OnmouseDown()
    {
        ShowClose();
    }

    private void ShowClose()
    {
        _isTaken = !_isTaken;
        if (_isTaken)
        {
            _mbp.Take();
        }
        else
        {
            _mbp.Put();
        }
    }
   
}
