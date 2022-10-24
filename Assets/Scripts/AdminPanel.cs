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
    private Slider _mouseSenseSlider;

    private MouseLook _mouseLook;

    private void Awake() {
        _mouseLook = Camera.main?.GetComponent<MouseLook>();
        if(_dayManager!= null) {
            _dayManager.SetAdminOverrideDay(forceThisDay, forceTraining);
        }
    }

    private void Start() {
        float value = PlayerPrefs.GetFloat("mouseSense", 1);

        _mouseSenseSlider.SetValueWithoutNotify(value);
        OnMouseSenseChange(value);
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
        if (Input.GetKeyDown(KeyCode.BackQuote))
            OpenMenu(!_adminMenuPanel.activeSelf);
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
}