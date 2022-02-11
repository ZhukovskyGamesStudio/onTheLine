using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Palto : InteractableObject
{
    public string sceneName = "Menu";

    [HideInInspector]
    public override void OnmouseDown()
    {
        SceneManager.LoadScene(sceneName);
    }
}
