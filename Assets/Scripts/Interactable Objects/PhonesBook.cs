using UnityEngine;
using UnityEngine.UI;

public class PhonesBook : InteractableObject
{
    public GameObject PhonesBookPanel;
    public GameObject PeopleListGrid1, PeopleListGrid2;
    public GameObject RoomWithPeoplePrefab;
    public GameObject bookTop;
    MovingBetweenTwoPointObject _mbp;
    public MovingBetweenTwoPointObject bookTopMbp;

    private bool _isTaken;

    public float MoveSpeed;

    private void Awake()
    {
        _mbp = GetComponent<MovingBetweenTwoPointObject>();
    }

    private void Start()
    {
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
        _isTaken = !_isTaken;
        if (_isTaken)
        {
            _mbp.Take();
            bookTopMbp.Take();
        }
        else
        {
            _mbp.Put();
            bookTopMbp.Put();
        }
    }

    public void OpenedBook()
    {

    }

    public void ClosedBook()
    {

    }


    void FillPeopleList() {
        Day curDay =  SaveManager.GetDay();
        Room[] rooms = curDay.allRooms;
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
    }
}
