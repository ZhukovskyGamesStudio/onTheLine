using UnityEngine;

public class LogManager : MonoBehaviour {
    [SerializeField]
    private Log logPrefab;

    public void Log(string text) {
        Log log = Instantiate(logPrefab, logPrefab.transform.parent);
        log.Init(text);
    }
}