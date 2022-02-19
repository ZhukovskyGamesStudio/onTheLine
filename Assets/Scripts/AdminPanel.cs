using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdminPanel : MonoBehaviour {
    public DayManager DayManager;
    [SerializeField]
    private GameObject AdminMenuPanel;
    private MouseLook _mouseLook;
    public Slider MouseSenseSlider;

    private void Awake() {
        _mouseLook = Camera.main?.GetComponent<MouseLook>();
    }

    private void Start() {
        float value = PlayerPrefs.GetFloat("mouseSense", 1);
       
        MouseSenseSlider.SetValueWithoutNotify(value);
        OnMouseSenseChange(value);
    }

    public void ReloadDay() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EraseSavedData() {
        SaveManager.sv = new SaveProfile();
        SaveManager.Save();
        
    }

    public void StopCalls() {
        DayManager.StopDayImmedeately();
    }

    public void StopTime() {
        if (Time.timeScale == 1)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void Quit() {
        Application.Quit();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            OpenMenu(!AdminMenuPanel.activeSelf);
    }

    public void OpenMenu(bool isOn) {
        AdminMenuPanel.SetActive(isOn);
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