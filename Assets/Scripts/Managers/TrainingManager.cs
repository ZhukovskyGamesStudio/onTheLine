using System;
using System.Collections;
using UnityEngine;

public class TrainingManager : MonoBehaviour {

    public static TrainingManager instance;
    
    
    [SerializeField]
    private GameObject _eyeSightDetector;

    private Hole _hole4;

    private const float WAIT_BEFORE_TOO_LONG = 30;
    private const float WAIT_END_TRAINING = 10;
    private const string TOO_LONG_TAG = "Too long";

    private Coroutine _coroutine;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        SaveManager.sv.isTrainingStarted = true;
        Instantiate(_eyeSightDetector, GameObject.Find("DoorNumber 4").transform);

        _hole4 = GameObject.Find("Hole 4").GetComponent<Hole>();
        _hole4.OnShtekerIn += AddPlugInTag;
        StartTooLongWaiting();
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

    public void StartTooLongWaiting() {
        StopTooLongWaiting();
        _coroutine = StartCoroutine(TooLongCoroutine());
    }
    
    public void StopTooLongWaiting() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
    }

    private IEnumerator TooLongCoroutine() {
        TagManager.RemoveTag(TOO_LONG_TAG);
        yield return new WaitForSeconds(WAIT_BEFORE_TOO_LONG);
        TagManager.AddTag(TOO_LONG_TAG);
    }

    private IEnumerator FinishTrainingCoroutine() {
        yield return new WaitForSeconds(WAIT_END_TRAINING);
        FinishTraining();
    }

    private void FinishTraining() {
        SaveManager.sv.isTrainingComplete = true;
        SaveManager.sv.currentDay++;
        SaveManager.Save();
        SceneLoadManager.LoadScene("Menu");
    }
}