using System.Collections.Generic;
using System.IO;
using UnityEditor;

#if UNITY_EDITOR
public class MenuTools : EditorWindow {
    [MenuItem("Tools/Dialogs Parser")]
    public static void ShowMyEditor() {
        GetWindow(typeof(DialogsParserWindow));
    }

    [MenuItem("Tools/Recollect Dialogs Database")]
    public static void RecollectDialogsDatabase() {
        DialogsCollection collection = CreateInstance<DialogsCollection>();

        string name = "Assets/Prefabs/ScriptableObjects/DialogsCollection.asset";
        if (!File.Exists(name))
            AssetDatabase.CreateAsset(collection, name);
        else
            collection = (DialogsCollection) AssetDatabase.LoadAssetAtPath(name, typeof(DialogsCollection));

        collection.allDialogs = new List<Dialog>();

        string[] guids = AssetDatabase.FindAssets("t:Dialog", null);
        foreach (string guid in guids) {
            Dialog dialog = (Dialog) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Dialog));
            collection.allDialogs.Add(dialog);
        }
    }
}
#endif