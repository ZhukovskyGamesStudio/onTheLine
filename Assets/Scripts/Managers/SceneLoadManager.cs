using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour {
    #region Singleton

    private static SceneLoadManager _instance;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(_instance.gameObject);
    }

    #endregion

    public static void LoadScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName);
    }
}