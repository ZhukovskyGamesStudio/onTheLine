using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Log : MonoBehaviour {
    [SerializeField]
    private TMP_Text logText;
    
    public void Init(string text) {
        gameObject.SetActive(true);
        logText.text = text;
        Destroy(gameObject, 15f);
    }

    public void DestroyImmediate() {
        Destroy(gameObject);
    }
}
