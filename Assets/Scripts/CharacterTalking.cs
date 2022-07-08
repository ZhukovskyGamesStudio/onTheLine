using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterTalking : MonoBehaviour
{
    public static CharacterTalking instance;
    public Commutator Commutator;
    public GameObject bubblePrefab;
    public GameObject GreyPanel;
    public GameObject Nothing;
    bool isVisible;
    Dictionary<GameObject, string> bubblesD;
    List<string> phrazes;
    GameObject selectedButton;


    private void Awake()
    {
        instance = this;
        bubblesD = new Dictionary<GameObject, string>();
        phrazes = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (Settings.instance.isWaitingForOperatorHello)
        //AddBubble("Повторите пожалуйста.", "/hello/");
        AddBubble("Повторите пожалуйста.", "/repeat/");
        //AddBubble("� ����� �������� ��������� ����������");

        OpenClose(false);
    }

    // Update is called once per frame
    void Update()
    {
        OpenClose(Input.GetKey(KeyCode.T));
    }

    void OpenClose(bool isOpen)
    {
        if (isVisible == isOpen)
            return;

        isVisible = isOpen;
        GreyPanel.SetActive(isVisible);
        if (isVisible)
        {
            Nothing.SetActive(bubblesD.Count == 0);
            selectedButton = null;
        }
        else
        {
            if (selectedButton)
                SayBubble(selectedButton);
        }

        CursorManager.ChangeState(isVisible);
    }

    public void AddBubble(string text, string specialText = "")
    {
        if (specialText == "")
            specialText = text;

        if (phrazes.Contains(specialText))
            return;
        GameObject bubl = Instantiate(bubblePrefab, GreyPanel.transform);
        Vector3 rndV = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), 0);

        bubl.transform.position = transform.position + rndV + rndV.normalized * 100;
        bubl.transform.GetChild(0).GetComponent<Text>().text = text;

        AddTriggersToButton(bubl);
        bubl.SetActive(true);

        phrazes.Add(specialText);
        bubblesD.Add(bubl, specialText);
    }

    void AddTriggersToButton(GameObject bubl)
    {
        EventTrigger bublTrigger = bubl.GetComponent<EventTrigger>();
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => SelectButton(bubl));

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => DeselectButton(bubl));

        bublTrigger.triggers.Add(pointerEnter);
        bublTrigger.triggers.Add(pointerExit);
    }



    public void RemoveBubble(GameObject bubl)
    {
        phrazes.Remove(bubblesD[bubl]);
        bubblesD.Remove(bubl);
        Destroy(bubl);
    }

    void SelectButton(GameObject bubl)
    {
        selectedButton = bubl;
    }

    void DeselectButton(GameObject bubl)
    {
        selectedButton = null;
    }

    void SayBubble(GameObject bubl)
    {
        string txt = bubblesD[bubl];
        Commutator.PassSoundFromOperator(txt);
    }
}
