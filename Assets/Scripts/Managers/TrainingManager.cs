using System.Collections;
using UnityEngine;

public class TrainingManager : MonoBehaviour {
    [SerializeField]
    private GameObject _eyeSightDetector;

    private Hole _hole4;

    private void Start() {
        SaveManager.sv.isTrainingStarted = true;
        Instantiate(_eyeSightDetector, GameObject.Find("DoorNumber 4").transform);

        _hole4 = GameObject.Find("Hole 4").GetComponent<Hole>();
        _hole4.OnShtekerIn += AddPlugInTag;
    }

    private void AddPlugInTag() {
        _hole4.OnShtekerIn -= AddPlugInTag;
        TagManager.AddTag("Plug 1 in");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            FinishTraining();
        }
    }

    public void Finish() {
        StartCoroutine(FinishTrainingCoroutine());
    }

    private IEnumerator FinishTrainingCoroutine() {
        yield return new WaitForSeconds(3);
        FinishTraining();
    }

    private void FinishTraining() {
        SaveManager.sv.isTrainingComplete = true;
        SaveManager.sv.currentDay++;
        SaveManager.Save();
        SceneLoadManager.LoadScene("Menu");
    }
}