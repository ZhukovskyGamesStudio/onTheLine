using System;
using Levitan;
using UnityEngine;

public class WindowsFileOpener : MonoBehaviour {
    void Start() {
#if UNITY_STANDALONE_WIN
        string[] args = Environment.GetCommandLineArgs();
        LogManager logManager = AppManager.Instance._LogManager;
        if (args.Length > 1) {
            string path = args[1];
            try {
                AppManager.Instance._saveManager.LoadProject(path);
            }
            catch {
                logManager.Log(path);
            }
        }
#endif
    }
}