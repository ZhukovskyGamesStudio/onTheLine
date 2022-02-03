using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Palto : MonoBehaviour
{
    public string sceneName = "Menu";

    public void OnMouseDown()
    {
        SceneManager.LoadScene(sceneName);
    }
}
