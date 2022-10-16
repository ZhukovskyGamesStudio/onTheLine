using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class DialogsParserWindow : EditorWindow {
    [SerializeField]
    private TextAsset[] jsonDialogs;

    private void OnGUI() {
        ScriptableObject target = this;
        SerializedObject so = new(target);
        SerializedProperty stringsProperty = so.FindProperty("jsonDialogs");
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();

        if (GUI.Button(new Rect(10, position.height - 60, position.width - 20, 50), "Parse"))
            try {
                for (int i = 0; i < stringsProperty.arraySize; i++) {
                    TextAsset jsonDialog = stringsProperty.GetArrayElementAtIndex(i).objectReferenceValue as TextAsset;
                    Parse(jsonDialog, false);
                }
            }
            catch {
                Debug.Log("Parse error");
                throw;
            }
    }

    private void Parse(TextAsset data, bool isFocusingWindow) {
        string text = data.text;
        Dialog dialog = CreateInstance<Dialog>();
        JsonUtility.FromJsonOverwrite(text, dialog);

        string path = "Assets/Dialogs/" + data.name.Substring(0, data.name.IndexOf('_')) + "/" + data.name + ".asset";
        if (!File.Exists(path)) {
            AssetDatabase.CreateAsset(dialog, path);
        } else {
            Dialog old = (Dialog) AssetDatabase.LoadAssetAtPath(path, typeof(Dialog));
            old.Copy(dialog);
            EditorUtility.SetDirty(old);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (isFocusingWindow)
            EditorUtility.FocusProjectWindow();
        MenuTools.RecollectDialogsDatabase();
    }
}
#endif