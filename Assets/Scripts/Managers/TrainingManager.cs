using UnityEngine;

public class TrainingManager : MonoBehaviour {

    [SerializeField]
    private GameObject TrainingWarning;
    
    void Start() {
        SaveManager.sv.isTrainingStarted = true;
        ShowIncompleteWarning();
    }

    private void ShowIncompleteWarning() {
        TrainingWarning.SetActive(true);
        Debug.Log("This is training Day. It is not complete! Press F to instantly finish it!");  
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