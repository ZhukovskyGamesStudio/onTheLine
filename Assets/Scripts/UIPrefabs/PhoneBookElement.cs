using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhoneBookElement : MonoBehaviour {
    public Text RoomNumberText;
    public GameObject HumanLinePrefab;

    public void SetUI(Room room) {
        string number = (room.number < 10 ? "0" : "") + room.number.ToString();
        RoomNumberText.text = room.number.ToString() + " " + room.description;
        foreach (Person person in room.roomMembers) {
            if (string.IsNullOrEmpty(person.Surname))
                continue;
            GameObject obj = Instantiate(HumanLinePrefab, this.transform);
            obj.SetActive(true);
            Text a = obj.GetComponent<Text>();

            a.text = string.Format("    {0} {1} ",
                person.Surname, person.Name
            );
            if (person.isDead)
                a.text = StrikeThrough(a.text);
        }
    }

    private string StrikeThrough(string s) => s.Aggregate("", (current, c) => current + c + '\u0336');
}