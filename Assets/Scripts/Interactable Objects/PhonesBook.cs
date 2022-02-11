using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhonesBook : InteractableObject
{
    public GameObject PhonesBookPanel;
    DialogsQueue DialogsQueue;
    public GameObject PeopleListGrid1, PeopleListGrid2;
    public GameObject RoomWithPeoplePrefab;
    public GameObject bookTop;
    MovingBetweenTwoPointObject mbp;
    public MovingBetweenTwoPointObject bookTopMbp;


    bool isTaken;

    public float MoveSpeed;

    private void Awake()
    {
        mbp = GetComponent<MovingBetweenTwoPointObject>();
    }

    private void Start()
    {

        DialogsQueue = DialogsQueue.instance;
        FillPeopleList();
    }

    [HideInInspector]
    public override void OnmouseDown()
    {
        ShowClose();
    }


    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Tab))
            ShowClose();
    }

    public void ShowClose()
    {
        isTaken = !isTaken;
        if (isTaken)
        {
            mbp.Take();
            bookTopMbp.Take();
        }
        else
        {
            mbp.Put();
            bookTopMbp.Put();
        }
    }

    public void OpenedBook()
    {

    }

    public void ClosedBook()
    {

    }


    void FillPeopleList()
    {
        //Переписать под большое количество страниц
        Room[] rooms = DialogsQueue.allRooms;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (i < rooms.Length / 2)
            {
                GameObject obj = Instantiate(RoomWithPeoplePrefab, PeopleListGrid1.transform);
                obj.SetActive(true);
                obj.GetComponent<PhoneBookElement>().SetUI(rooms[i]);
            }
            else
            {
                GameObject obj = Instantiate(RoomWithPeoplePrefab, PeopleListGrid2.transform);
                obj.SetActive(true);
                obj.GetComponent<PhoneBookElement>().SetUI(rooms[i]);
            }

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(PeopleListGrid1.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(PeopleListGrid2.GetComponent<RectTransform>());

        /*
            SaveProfile saveProfile = Building.Load();
            for (int i = 0; i < saveProfile.rooms.Length; i++)
            {
                GameObject obj = Instantiate(RoomWithPeoplePrefab, PeopleListGrid.transform);
                obj.SetActive(true);
                obj.GetComponent<PhoneBookElement>().SetUI(saveProfile.rooms[i]);
            }  
        */
    }
}
