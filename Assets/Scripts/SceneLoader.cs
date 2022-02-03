using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public System.Action beforeLoadingAnotherScene;


    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(string sceneName)
    {
        if(beforeLoadingAnotherScene != null)
            beforeLoadingAnotherScene.Invoke();
        SceneManager.LoadScene(sceneName);
    }
}
