using System.Collections;
using UnityEngine;

public class TrainingManager : MonoBehaviour {
    public static TrainingManager Instance;

    [SerializeField]
    private GameObject _eyeSightDetector;

    [SerializeField]
    private GameObject _headphonesTrainingStartingPos;

    private Hole _hole4;

    private const float WAIT_BEFORE_TOO_LONG = 45;
    private const string TOO_LONG_TAG = "Too long";

    private Coroutine _coroutine;

    private void Awake() {
        Instance = this;
        Instantiate(_headphonesTrainingStartingPos);
    }

    private void Start() {
        Headphones.instance.ChangeStartingPlace(_headphonesTrainingStartingPos.transform);
        SaveManager.sv.isTrainingStarted = true;
        Instantiate(_eyeSightDetector, GameObject.Find("DoorNumber 4").transform);
        _hole4 = GameObject.Find("Hole 4").GetComponent<Hole>();

        DisableOtherHoles();
        _hole4.OnShtekerIn += AddPlugInTag;
        StartTooLongWaiting();
    }

    private void DisableOtherHoles() {
        Transform baseTransform = FindObjectOfType<Commutator>().transform.Find("Holes");
        baseTransform.Find("Hole 1").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 2").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 3").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 4").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 5").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 6").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 7").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 8").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 9").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 10").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 11").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 12").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 13").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 14").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 15").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 16").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 17").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 18").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 19").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 20").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 21").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 22").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 23").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 24").GetComponent<Hole>().enabled = false;
        baseTransform.Find("Hole 25").GetComponent<Hole>().enabled = false;
    }

    private void AddPlugInTag() {
        _hole4.OnShtekerIn -= AddPlugInTag;
        TagManager.AddTag("Plug 1 in");
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
}