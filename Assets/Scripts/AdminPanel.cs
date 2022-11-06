using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdminPanel : MonoBehaviour {
    [SerializeField]
    private DayManager _dayManager;

    [SerializeField]
    private Day forceThisDay;

    [SerializeField]
    private GameObject _adminMenuPanel;

    [SerializeField]
    private Text tagsText;

    private void Awake() {
        if (_dayManager != null) {
            _dayManager.SetAdminOverrideDay(forceThisDay);
        }

        _adminMenuPanel.SetActive(PlayerPrefs.GetInt("adminPanel") == 1);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            _adminMenuPanel.SetActive(!_adminMenuPanel.activeSelf);
            PlayerPrefs.SetInt("adminPanel", _adminMenuPanel.activeSelf ? 1 : 0);
        }

        if (_adminMenuPanel.activeSelf) {
            UpdateTags();
        }
    }

    public void AddTag(string tagValue) {
        if (tagValue.Length <= 0) {
            return;
        }

        if (tagValue.StartsWith("!")) {
            TagManager.RemoveTag(tagValue);
        } else {
            TagManager.AddTag(tagValue);
        }
    }

    private void UpdateTags() {
        tagsText.text = "";
        foreach (var variable in TagManager.Tags) {
            tagsText.text += variable + "\n";
        }
    }

    public void StopCalls() {
        _dayManager.StopDayImmedeately();
    }

    public void ReloadDay() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EraseSavedData() {
        SaveManager.sv = new SaveProfile();
        TagManager.ClearAll();

        SaveManager.Save();
        SceneManager.LoadScene("Menu");
    }

    public void SetDay(int day) {
        SaveManager.sv.currentDay = day;
        SaveManager.Save();
        SceneManager.LoadScene("Level");
    }
}