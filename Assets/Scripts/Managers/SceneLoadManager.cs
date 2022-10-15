using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour {
    #region Singleton

    public static SceneLoadManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(instance.gameObject);
    }

    #endregion

    public static void LoadScene(string SceneName) {
        SceneManager.LoadSceneAsync(SceneName);
        TagManager.instance.Clear();
    }
}
