using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Dialogue))]
public class DialoguePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Find the properties
        var isEventProperty = property.FindPropertyRelative("isEvent");
        var textProperty = property.FindPropertyRelative("text");
        var portraitProperty = property.FindPropertyRelative("portrait");
        var hintTextProperty = property.FindPropertyRelative("hintText");
        var cmdProperty = property.FindPropertyRelative("cmd");
        var tgtPathProperty = property.FindPropertyRelative("tgtPath");
        var startingPathProperty = property.FindPropertyRelative("startingPath");
        var tgtINodeName = property.FindPropertyRelative("tgtINodeName");
        var redirectTgt = property.FindPropertyRelative("redirectTgt");


        // Calculate rects
        var lineHeight = EditorGUIUtility.singleLineHeight;
        var textAreaHeight = EditorGUIUtility.singleLineHeight * 4; // Adjust this for your desired height
        var currentPos = position;
        currentPos.height = lineHeight;

        // Draw isEvent field
        EditorGUI.PropertyField(currentPos, isEventProperty);
        currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw TextArea with adjusted width
        currentPos.height = textAreaHeight;
        EditorGUI.PropertyField(new Rect(currentPos.x, currentPos.y, position.width, textAreaHeight), textProperty, new GUIContent("Text"));
        currentPos.y += textAreaHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw portrait field
        currentPos.height = lineHeight;
        EditorGUI.PropertyField(currentPos, portraitProperty);
        currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Conditionally show cmd, hintText, tgtPath, and startingPath based on isEvent value
        if (isEventProperty.boolValue)
        {
            EditorGUI.PropertyField(currentPos, cmdProperty);
            currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, hintTextProperty);
            currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, tgtPathProperty);
            currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, startingPathProperty);
            currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, tgtINodeName);
            currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(currentPos, redirectTgt);
            currentPos.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var isEventProperty = property.FindPropertyRelative("isEvent");
        var height = EditorGUIUtility.singleLineHeight * 3; // Base height for isEvent, text, and portrait
        height += EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing; // Text area height

        if (isEventProperty.boolValue)
        {
            height += EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 3; // cmd, hintText, tgtPath, startingPath
        }

        return height;
    }
}
