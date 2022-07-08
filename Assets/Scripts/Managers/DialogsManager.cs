using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogsManager : MonoBehaviour {
    #region Singleton

    public static DialogsManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    #endregion
    
    
    
    [SerializeField]
    private DialogsCollection dialogsCollection;

    public Dialog GetDialogById(string id) => dialogsCollection.allDialogs.FirstOrDefault(dialog => dialog.Id == id);

}
