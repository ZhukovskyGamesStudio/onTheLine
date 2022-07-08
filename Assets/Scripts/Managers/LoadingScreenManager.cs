using UnityEngine;

public class LoadingScreenManager : MonoBehaviour {
    void Start() {
        SceneLoadManager.LoadScene("Menu");
    }
}