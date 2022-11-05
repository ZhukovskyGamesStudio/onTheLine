using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackingOutLevelChanger : MonoBehaviour {
    public static BlackingOutLevelChanger instance;
    
    [SerializeField]
    private string SceneToLoad = "Level";

    [SerializeField]
    private float BlackingTime = 3f;

    [SerializeField]
    private Image BlackingImage;

    private Coroutine _blackingCoroutine;
    private Action onEnded;

    private void Awake() {
        instance = this;
    }

    public void StartBlackingOut(Action callback = null) {
        Stop();

        _blackingCoroutine = StartCoroutine(BlackingCoroutine(callback));
    }

    public void Stop() {
        BlackingImage.color = new Color(0, 0, 0, 0);
        if (_blackingCoroutine != null) {
            StopCoroutine(_blackingCoroutine);
        }
    }

    public IEnumerator BlackingCoroutine(Action callback = null) {
        float time = 0;
        float blackingTimeUntilDark = BlackingTime * 0.8f;
        while (time < blackingTimeUntilDark)
        {
            time += Time.deltaTime;
            BlackingImage.color = new Color(0, 0, 0, time / blackingTimeUntilDark);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(blackingTimeUntilDark * 0.2f);
        callback?.Invoke();

        SceneManager.LoadScene(SceneToLoad);
    }
}
