using UnityEngine;

public class TrainingManager : MonoBehaviour {
    void Start() {
        SaveManager.sv.isTrainingStarted = true;
        Debug.Log("This is training Day. Press F to instantly finish it!");
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