using UnityEngine;

public class TrainingManager : MonoBehaviour {

    [SerializeField]
    private GameObject TrainingWarning;
    
    void Start() {
        SaveManager.sv.isTrainingStarted = true;
        TrainingWarning.SetActive(true);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            FinishTraining();
        }
    }

    void FinishTraining() {
        SaveManager.sv.isTrainingComplete = true;
        SaveManager.sv.currentDay++;
        SaveManager.Save();
        SceneLoadManager.LoadScene("Menu");
    }
}