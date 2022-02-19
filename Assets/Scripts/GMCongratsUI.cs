using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GMCongratsUI : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Button Button;
    public GameObject WinPanel;
    Clock Clock;


    private void Start()
    {
        Clock = Clock.instance;
        Clock.onEndDay += CheckWin;
    }


    void CheckWin()
    {
        if (TagManager.CheckTag(Tags.girlFoundWrong))
        {
            WinPanel.SetActive(true);
            Text.text = "Дежурные уехали по неправильному адресу. И девочки там не было." +
                "\nБыли только женщины и мужчины и пустота. Как у Пелевина. Только без Чапаева.";
        }
        else if (TagManager.CheckTag(Tags.girlFound))
        {
            WinPanel.SetActive(true);
            Text.text = "Вы нашли потерянную девочку! Вы молодец!" +
                "\nРодина вас не забудет! Мы тоже! Вы тоже! Долой Альгеймер!";
        }
    }

    private void OnDestroy()
    {
        Clock.onEndDay -= CheckWin;
    }
}
