using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lamp : InteractableObject
{
    public string SceneToLoad = "Level";
    public float BlackingTime = 3f;
    public Image BlackingImage;
    Coroutine coroutine;

    [HideInInspector]
    public override void OnmouseDown()
    {
        coroutine = StartCoroutine(BlackingCoroutine());
    }

    IEnumerator BlackingCoroutine()
    {
        float time = 0;
        float blackingTimeUntilDark = BlackingTime * 0.8f;
        while (time < blackingTimeUntilDark)
        {
            time += Time.deltaTime;
            BlackingImage.color = new Color(0, 0, 0, time / blackingTimeUntilDark);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(blackingTimeUntilDark * 0.2f);
        LoadLevel();
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    private void OnMouseUp()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        BlackingImage.color = new Color(0, 0, 0, 0);
    }
}
