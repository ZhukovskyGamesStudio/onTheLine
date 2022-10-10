using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(QueuePos))]
public class QueuePosDrawer : PropertyDrawer {
    private const int dialogSize = 300;
    private const int fromTimeSize = 50;
    private const int toTimeSize = 50;
    private const int spaceSize = 20;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect dialogRect = new Rect(position.x, position.y, dialogSize, position.height);
        Rect fromTimeRect = new Rect(position.x, position.y, fromTimeSize, position.height);
        fromTimeRect.x += dialogSize + spaceSize;
        Rect toTimeRect = new Rect(position.x, position.y, toTimeSize, position.height);
        toTimeRect.x += fromTimeSize + dialogSize + spaceSize * 2;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels

        EditorGUI.PropertyField(dialogRect, property.FindPropertyRelative("dialog"), GUIContent.none);
        EditorGUI.PropertyField(fromTimeRect, property.FindPropertyRelative("fromTime"), GUIContent.none);
        EditorGUI.PropertyField(toTimeRect, property.FindPropertyRelative("toTime"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif