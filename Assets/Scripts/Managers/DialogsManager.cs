using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogsManager : MonoBehaviour {
    #region Singleton

    public static DialogsManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
        } else {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    #endregion
    
    
    
    [SerializeField]
    private DialogsCollection dialogsCollection;

    public Dialog GetDialogById(string id) => dialogsCollection.allDialogs.FirstOrDefault(dialog => dialog.Id == id);

}
