using UnityEngine;

public class TrainingManager : MonoBehaviour {

    [SerializeField]
    private GameObject TrainingWarning;

    public GameObject EyeSightDetector;

    private Hole hole4;
    void Start() {
        SaveManager.sv.isTrainingStarted = true;
        Instantiate(EyeSightDetector, GameObject.Find("DoorNumber 4").transform);

        hole4 = GameObject.Find("Hole 4").GetComponent<Hole>();
        hole4.OnShtekerIn += AddPlugInTag;
        TrainingWarning.SetActive(true);
    }

    private void AddPlugInTag() {
        hole4.OnShtekerIn -= AddPlugInTag;
        TagManager.AddTag("Plug 1 in");
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