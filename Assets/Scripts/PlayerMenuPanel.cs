using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuPanel : MonoBehaviour {
    [Header("Other")]
    [SerializeField]
    private GameObject _menuPanel;

    [SerializeField]
    private Slider _mouseSenseSlider, _speechSpeedSlider, _soundsSlider;

    [SerializeField]
    private Image _pauseImage;

    private MouseLook _mouseLook;

    private void Awake() {
        _mouseLook = Camera.main?.GetComponent<MouseLook>();
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

        float soundVolume = PlayerPrefs.GetFloat("soundVolume", 1);
        _soundsSlider.SetValueWithoutNotify(soundVolume);
        SetCurrentSound();
        _soundsSlider.onValueChanged.AddListener(delegate { OnSoundVolumeChange(_soundsSlider.value); });
    }

    public void StopTime() {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        SetCurrentSound();
        _pauseImage.gameObject.SetActive(Time.timeScale != 1);
    }

    private void SetCurrentSound() {
        AudioListener.volume = Time.timeScale == 1 ? (_soundsSlider ? 1 : 0) : 0;
    }

    public void Quit() {
        Application.Quit();
    }

    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q)) {
            OpenMenu(!_menuPanel.activeSelf);
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OpenMenu(!_menuPanel.activeSelf);
        }
#endif

        if (Input.GetKeyDown(KeyCode.P)) {
            StopTime();
        }
    }

    private void OpenMenu(bool isOn) {
        _menuPanel.SetActive(isOn);
        if (isOn)
            CursorManager.ChangeState(true);
        else
            CursorManager.BackToPreviousState();
    }

    private void OnMouseSenseChange(float value) {
        PlayerPrefs.SetFloat("mouseSense", value);
        _mouseLook.SensivityMultiplier = value;
    }

    private void OnSpeechSpeedChange(float value) {
        PlayerPrefs.SetFloat("speechSpeed", value);
        Settings.config.timeScale = value;
    }

    private void OnSoundVolumeChange(float value) {
        PlayerPrefs.SetFloat("soundVolume", value);
    }
}