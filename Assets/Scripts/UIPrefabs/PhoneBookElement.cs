using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneBookElement : MonoBehaviour
{
    public Text RoomNumberText;
    public GameObject HumanLinePrefab;

    public void SetUI(Room room)
    {
        string number = (room.number < 10 ? "0" : "") + room.number.ToString();
        RoomNumberText.text = room.number.ToString() + " " + room.description;
        for (int i = 0; i < room.roomMembers.Count; i++)
        {
            GameObject obj = Instantiate(HumanLinePrefab, this.transform);
            obj.SetActive(true);
            Text a = obj.GetComponent<Text>();

            a.text = string.Format("    {0} {1} ",
               room.roomMembers[i].Surname,  room.roomMembers[i].Name
              );
            if (room.roomMembers[i].isDead)
                a.text = StrikeThrough(a.text);

            /*
            a.text = string.Format("    {0} {1} {2} {3} {4} {5}",
                room.roomMembers[i].Name, room.roomMembers[i].Surname,
                room.roomMembers[i].Sex.ToString(), room.roomMembers[i].Age.ToString(), room.roomMembers[i].Work.ToString(), room.roomMembers[i].Temperament.ToString());
            if (room.roomMembers[i].isDead)
                a.text = StrikeThrough(a.text);    */
        }
    }

    public string StrikeThrough(string s)
    {
        string strikethrough = "";
        foreach (char c in s)
        {
            strikethrough = strikethrough + c + '\u0336';
        }
        return strikethrough;
    }
}
