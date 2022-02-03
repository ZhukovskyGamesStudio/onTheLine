using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public DayManager DayManager;
    public GameObject MenuPanel;
    public MouseLook MouseLook;
    public Slider MouseSenseSlider;
    public GameObject PeopleList;
    public GameObject PeopleListGrid;
    public GameObject RoomWithPeoplePrefab;

    private void Start()
    {
        float value = PlayerPrefs.GetFloat("mouseSense", 1);
        MouseSenseSlider.SetValueWithoutNotify(value);
        OnMouseSenseChange(value);
        FillPeopleList();
    }

    public void ReloadDay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StopCalls()
    {
        DayManager.StopDayImmedeately();
    }

    public void StopTime()
    {
        if (Time.timeScale == 1)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    void FillPeopleList()
    {
        Room[] rooms = DialogsQueue.instance.allRooms;
        for (int i = 0; i < rooms.Length; i++)
        {
            GameObject obj = Instantiate(RoomWithPeoplePrefab, PeopleListGrid.transform);
            obj.SetActive(true);
            obj.GetComponent<PhoneBookElement>().SetUI(rooms[i]);
        }
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

    public void OpenPeopleList()
    {
        PeopleList.SetActive(!PeopleList.activeSelf);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            OpenMenu(!MenuPanel.activeSelf);
    }

    public void OpenMenu(bool isOn)
    {
        MenuPanel.SetActive(isOn);
        if (isOn)
            CursorManager.ChangeState(true);
        else
            CursorManager.BackToPreviousState();
    }

    public void OnMouseSenseChange(float value)
    {
        PlayerPrefs.SetFloat("mouseSense", value);
        MouseLook.SensivityMultiplier = value;
    }

}
