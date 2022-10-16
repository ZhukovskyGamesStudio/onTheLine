using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour {
    [SerializeField]
    private float _minLoadTime = 2;

    private void Start() {
        StartCoroutine(MinLoad());
    }

    private IEnumerator MinLoad() {
        yield return new WaitForSeconds(_minLoadTime);
        SceneLoadManager.LoadScene("Menu");
    }
}