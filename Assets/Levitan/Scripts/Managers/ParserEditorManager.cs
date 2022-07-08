using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class ParserEditorManager : EditorWindow {
    public void CreateGUI() {
        rootVisualElement.Add(new Label("Select Dialog to Parse"));
    }

    [MenuItem("Tools/Dialogs Parser")]
    public static void ShowMyEditor() {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<ParserEditorManager>();
        wnd.titleContent = new GUIContent("Dialogs Parser");
    }
}
#endif