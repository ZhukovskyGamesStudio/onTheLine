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

    [SerializeField]
    private Image _soundsToggleImage, _pauseImage;

    [SerializeField]
    private Toggle _soundsToggle;

    [SerializeField]
    private Sprite _soundOn, _soundOff;

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
        OnMouseSenseChange(mouseSense);
        _mouseSenseSlider.onValueChanged.AddListener(delegate { OnMouseSenseChange(_mouseSenseSlider.value); });

        float speechSpeed = PlayerPrefs.GetFloat("speechSpeed", 1);
        _speechSpeedSlider.SetValueWithoutNotify(speechSpeed);
        OnSpeechSpeedChange(speechSpeed);
        _speechSpeedSlider.onValueChanged.AddListener(delegate { OnSpeechSpeedChange(_speechSpeedSlider.value); });

        bool isSound = PlayerPrefs.GetInt("isSound", 1) == 1;
        _soundsToggle.SetIsOnWithoutNotify(isSound);
        SetSoundMuteMode(isSound);
        _soundsToggle.onValueChanged.AddListener(delegate { SetSoundMuteMode(_soundsToggle.isOn); });
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

    private void SetSoundMuteMode(bool isSoundOn) {
        _soundsToggleImage.sprite = isSoundOn ? _soundOn : _soundOff;
        SetCurrentSound();
        PlayerPrefs.SetInt("isSound", isSoundOn ? 1 : 0);
    }

    public void StopCalls() {
        _dayManager.StopDayImmedeately();
    }

    public void StopTime() {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        SetCurrentSound();
        _pauseImage.gameObject.SetActive(Time.timeScale != 1);
    }

    private void SetCurrentSound() {
        AudioListener.volume = Time.timeScale == 1 ? (_soundsToggle.isOn ? 1 : 0) : 0;
    }

    public void Quit() {
        Application.Quit();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            OpenMenu(!_adminMenuPanel.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.P)) {
            StopTime();
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

    private void OnSpeechSpeedChange(float value) {
        PlayerPrefs.SetFloat("speechSpeed", value);
        Settings.config.timeScale = value;
    }
}