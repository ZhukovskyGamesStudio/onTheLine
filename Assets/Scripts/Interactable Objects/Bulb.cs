using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulb : MonoBehaviour
{
    [Min(0.1f)] public float blinkingTime;
    public MeshRenderer MeshRenderer;
    public Material OnMaterial, OffMaterial;

    [HideInInspector] public bool isOn;
    [HideInInspector] public bool isBlinking;

    Coroutine blinkingCoroutine;
    int curState = 0;

    public void OnMouseDown()
    {
        curState++;
        if (curState > 2)
            curState = 0;
        ChangeState(curState);
    }


    public void ChangeState(int stateIndex) // 0 - выкл, 1 - вкл, 2 - мигание
    {
       
        if (stateIndex == 0)
        {
            isOn = false;
            Turn();
            if (isBlinking)
            {
                isBlinking = false;
                StopCoroutine(blinkingCoroutine);
            }
        }
        else if (stateIndex == 1)
        {
            isOn = true;
          
            Turn();
            if(isBlinking)
            {
                isBlinking = false;
                StopCoroutine(blinkingCoroutine);
            }
           
        } 
        else if(stateIndex == 2 && !isBlinking)
        {
            isOn = false;               
            isBlinking = true;
            blinkingCoroutine = StartCoroutine(Blinking());
        }
                   
    }

    void Turn()
    {
        MeshRenderer.material = isOn ? OnMaterial : OffMaterial;
    }


    IEnumerator Blinking()
    {
        while (true)
        {
            isOn = !isOn;
            Turn();
            yield return new WaitForSeconds(blinkingTime);
        }

    }
}
