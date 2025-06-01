using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Match3d.Gameplay.Level;
using Match3d.Gameplay.Item;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private SerializedProperty _dataProp;
    private SerializedProperty _secondsProp;
    private SerializedProperty _goalItemsProp;
    private SerializedProperty _layoutItemsProp;

    private ReorderableList _goalList;
    private ReorderableList _layoutList;

    private void OnEnable()
    {
        _dataProp = serializedObject.FindProperty("_data");
        _secondsProp = _dataProp.FindPropertyRelative("seconds");
        _goalItemsProp = _dataProp.FindPropertyRelative("goalItems");
        _layoutItemsProp = _dataProp.FindPropertyRelative("layoutItems");

        _goalList = new ReorderableList(serializedObject, _goalItemsProp, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Goal Items"),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _goalItemsProp.GetArrayElementAtIndex(index);
                rect.y += 2;
                float w = rect.width / 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, w - 5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("type"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + w, rect.y, w - 5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("count"), GUIContent.none);
            }
        };

        _layoutList = new ReorderableList(serializedObject, _layoutItemsProp, true, true, true, true)
        {
            drawHeaderCallback = rect => 
            {
                // Main header
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Layout Items", 
                    EditorStyles.boldLabel);
                
                // Define spacing for column headers
                float columnHeaderYOffset = EditorGUIUtility.singleLineHeight + 2; // Space after main header
                float w = rect.width / 5;
                
                // Column headers with clear separation from main header
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y + columnHeaderYOffset, w - 2, EditorGUIUtility.singleLineHeight),
                    "Item Type", 
                    EditorStyles.miniLabel);
                
                EditorGUI.LabelField(
                    new Rect(rect.x + w, rect.y + columnHeaderYOffset, w - 2, EditorGUIUtility.singleLineHeight),
                    "Min Volume", 
                    EditorStyles.miniLabel);
                
                EditorGUI.LabelField(
                    new Rect(rect.x + 2 * w, rect.y + columnHeaderYOffset, w - 2, EditorGUIUtility.singleLineHeight),
                    "Max Volume", 
                    EditorStyles.miniLabel);
                
                EditorGUI.LabelField(
                    new Rect(rect.x + 3 * w, rect.y + columnHeaderYOffset, w - 2, EditorGUIUtility.singleLineHeight),
                    "Count", 
                    EditorStyles.miniLabel);
            },
            elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 4,
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _layoutItemsProp.GetArrayElementAtIndex(index);
                rect.y += 2;
                float w = rect.width / 5;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, w - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("type"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + w, rect.y, w - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("minVolume"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 2 * w, rect.y, w - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("maxVolume"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 3 * w, rect.y, w - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("count"), GUIContent.none);
            },
            // Increased header height to fit both main header and column headers with proper spacing
            headerHeight = (EditorGUIUtility.singleLineHeight * 2) + 8
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_secondsProp, new GUIContent("Seconds"));
        EditorGUILayout.Space();

        _goalList.DoLayoutList();
        EditorGUILayout.Space();
        _layoutList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}