using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Facs : MonoBehaviour
{
    public float WaitTimeUntilExtinct = 10;
    public float timeBetweenLetters = 10;
    public GameObject PenaltyList;
    public Text ExplanationText;

    Coroutine Coroutine;
    
    void Start()
    {
        PenaltyList.SetActive(false);
    }

    public void NewPenalty(string exp)
    {
        Debug.Log("Вам начислен денежный штраф.");
        if (Coroutine != null)
            StopCoroutine(Coroutine);
        Coroutine = StartCoroutine(PenaltyCoroutine(exp));
    }

    

    IEnumerator PenaltyCoroutine(string exp)
    {
        PenaltyList.SetActive(true);
        ExplanationText.text = "";

        for (int i = 0; i < exp.Length; i++)
        {
            ExplanationText.text += exp[i];
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        yield return new WaitForSeconds(WaitTimeUntilExtinct);

        PenaltyList.SetActive(false);
    }                    


}
