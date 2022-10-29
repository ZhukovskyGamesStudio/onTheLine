using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdminPanel : MonoBehaviour {
    [SerializeField]
    private Day forceThisDay;

    [SerializeField]
    private bool forceTraining;

    [Header("Other")]
    [SerializeField]
    private DayManager _dayManager;

    [SerializeField]
    private GameObject _adminMenuPanel;

    [SerializeField]
    private Slider _mouseSenseSlider, _speechSpeedSlider;

    private MouseLook _mouseLook;

    private void Awake() {
        _mouseLook = Camera.main?.GetComponent<MouseLook>();
        if (_dayManager != null) {
            _dayManager.SetAdminOverrideDay(forceThisDay, forceTraining);
        }
    }

    private void Start() {
        float mouseSense = PlayerPrefs.GetFloat("mouseSense", 0.7f);
        _mouseSenseSlider.SetValueWithoutNotify(mouseSense);
        _speechSpeedSlider.onValueChanged.AddListener(delegate { OnMouseSenseChange(_mouseSenseSlider.value); });
        OnMouseSenseChange(mouseSense);

        float speechSpeed = PlayerPrefs.GetFloat("speechSpeed", 1);
        _speechSpeedSlider.SetValueWithoutNotify(speechSpeed);
        OnSpeechSpeedChange(speechSpeed);
        _speechSpeedSlider.onValueChanged.AddListener(delegate { OnSpeechSpeedChange(_speechSpeedSlider.value); });
    }

    public void ReloadDay() {
        TagManager.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EraseSavedData() {
        SaveManager.sv = new SaveProfile();
        TagManager.ClearAll();

        SaveManager.Save();
        SceneManager.LoadScene("Menu");
    }

    public void StopCalls() {
        _dayManager.StopDayImmedeately();
    }

    public void StopTime() {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
    }

    public void Quit() {
        Application.Quit();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            OpenMenu(!_adminMenuPanel.activeSelf);
        }
    }

    private void OpenMenu(bool isOn) {
        _adminMenuPanel.SetActive(isOn);
        if (isOn)
            CursorManager.ChangeState(true);
        else
            CursorManager.BackToPreviousState();
    }

    public void OnMouseSenseChange(float value) {
        PlayerPrefs.SetFloat("mouseSense", value);
        _mouseLook.SensivityMultiplier = value;
    }

    public void OnSpeechSpeedChange(float value) {
        PlayerPrefs.SetFloat("speechSpeed", value);
        Settings.config.timeScale = value;
    }
}