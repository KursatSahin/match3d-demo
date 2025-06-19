using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Match3d.Gameplay.Level;
using Match3d.Gameplay.Item;
using UnityEditorInternal;

public class LevelManagerWindow : EditorWindow
{
    // Window state
    private Vector2 _levelListScrollPosition;
    private Vector2 _levelDetailsScrollPosition;
    private LevelData _selectedLevel;
    private List<LevelData> _allLevels = new List<LevelData>();
    private SerializedObject _serializedLevel;
    private int _selectedLevelIndex = -1;
    private string _searchFilter = "";
    
    // Serialized properties
    private SerializedProperty _dataProp;
    private SerializedProperty _secondsProp;
    private SerializedProperty _goalItemsProp;
    private SerializedProperty _layoutItemsProp;
    
    // ReorderableLists
    private ReorderableList _goalList;
    private ReorderableList _layoutList;
    
    
    [MenuItem("Tools/Match 3D/Level Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<LevelManagerWindow>("Level Manager");
        window.minSize = new Vector2(800, 600);
        window.Show();
    }
    
    private void OnEnable()
    {
        RefreshLevelList();
    }
    
    private void RefreshLevelList()
    {
        // Find all level assets in the project
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        _allLevels.Clear();
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (level != null)
            {
                _allLevels.Add(level);
            }
        }
        
        // Sort levels by name or ID if applicable
        _allLevels = _allLevels.OrderBy(l => l.name).ToList();
    }
    
    private void OnGUI()
    {
        DrawToolbar();
        
        EditorGUILayout.BeginHorizontal();
        
        // Left panel - Level list
        DrawLevelList();
        
        // Right panel - Level details
        DrawLevelDetails();
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            RefreshLevelList();
        }
        
        if (GUILayout.Button("Create New Level", EditorStyles.toolbarButton, GUILayout.Width(120)))
        {
            CreateNewLevel();
        }
        
        if (_selectedLevel != null)
        {
            if (GUILayout.Button("Clone Level", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                DuplicateSelectedLevel();
            }
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Delete Level", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Delete Level", 
                    $"Are you sure you want to delete level '{_selectedLevel.name}'?", 
                    "Delete", "Cancel"))
                {
                    DeleteSelectedLevel();
                }
            }
        }
        else
        {
            GUILayout.FlexibleSpace();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawLevelList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        
        // Search field
        _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
        
        // Level list with scrolling
        _levelListScrollPosition = EditorGUILayout.BeginScrollView(
            _levelListScrollPosition, 
            GUILayout.Width(250), 
            GUILayout.ExpandHeight(true));
        
        // Filter and display levels
        for (int i = 0; i < _allLevels.Count; i++)
        {
            LevelData level = _allLevels[i];
            
            // Skip if doesn't match filter
            if (!string.IsNullOrEmpty(_searchFilter) &&!level.name.ToLower().Contains(_searchFilter.ToLower())) 
                continue;
            
            // Button style changes when selected
            GUIStyle style = new GUIStyle(GUI.skin.button);
            
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            
            if (i == _selectedLevelIndex)
            {
                style.normal.background = EditorGUIUtility.isProSkin 
                    ? Texture2D.grayTexture 
                    : Texture2D.whiteTexture;
                
                style.normal.textColor = EditorGUIUtility.isProSkin 
                    ? Color.white 
                    : Color.blue;
            }
            
            if (GUILayout.Button(level.name, style, GUILayout.Height(30))) SelectLevel(i);
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawLevelDetails()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        
        if (_selectedLevel == null)
        {
            EditorGUILayout.HelpBox("Select a level from the list to edit", MessageType.Info);
        }
        else
        {
            // Level name field
            EditorGUI.BeginChangeCheck();
            
            string newName = EditorGUILayout.TextField("Level Name", _selectedLevel.name);
            
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(newName))
            {
                RenameSelectedLevel(newName);
            }
            
            EditorGUILayout.Space();
            
            // Level details scroll view
            _levelDetailsScrollPosition = EditorGUILayout.BeginScrollView(
                _levelDetailsScrollPosition, 
                GUILayout.ExpandHeight(true), 
                GUILayout.ExpandWidth(true));
            
            if (_serializedLevel != null)
            {
                _serializedLevel.Update();
                
                EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_secondsProp, new GUIContent("Seconds"));
                EditorGUILayout.Space();
                
                // Draw Goal Items list
                if (_goalList != null)
                {
                    _goalList.DoLayoutList();
                }
                
                EditorGUILayout.Space();
                
                // Draw Layout Items list
                if (_layoutList != null)
                {
                    _layoutList.DoLayoutList();
                }
                
                _serializedLevel.ApplyModifiedProperties();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void SelectLevel(int index)
    {
        if (index >= 0 && index < _allLevels.Count)
        {
            _selectedLevelIndex = index;
            _selectedLevel = _allLevels[index];
            
            // Set up serialized object and property
            _serializedLevel = new SerializedObject(_selectedLevel);
            
            // Find properties
            _dataProp = _serializedLevel.FindProperty("_data");
            _secondsProp = _dataProp.FindPropertyRelative("seconds");
            _goalItemsProp = _dataProp.FindPropertyRelative("goalItems");
            _layoutItemsProp = _dataProp.FindPropertyRelative("layoutItems");
            
            // Set up reorderable lists
            SetupReorderableLists();
        }
    }
    
    private void SetupReorderableLists()
    {
        // Goal Items list
        _goalList = new ReorderableList(_serializedLevel, _goalItemsProp, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Goal Items"),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _goalItemsProp.GetArrayElementAtIndex(index);
                rect.y += 2;
                float w = rect.width / 2;
                
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, w - 5, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("type"), GUIContent.none);
                
                EditorGUI.PropertyField(new Rect(rect.x + w, rect.y, w - 5, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("count"), GUIContent.none);
            }
        };
        
        // Layout Items list with column headers
        _layoutList = new ReorderableList(_serializedLevel, _layoutItemsProp, true, true, true, true)
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
                
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, w - 2, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("type"), GUIContent.none);
                
                EditorGUI.PropertyField(new Rect(rect.x + w, rect.y, w - 2, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("minVolume"), GUIContent.none);
                
                EditorGUI.PropertyField(new Rect(rect.x + 2 * w, rect.y, w - 2, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("maxVolume"), GUIContent.none);
                
                EditorGUI.PropertyField(new Rect(rect.x + 3 * w, rect.y, w - 2, EditorGUIUtility.singleLineHeight), 
                    element.FindPropertyRelative("count"), GUIContent.none);
            },
            headerHeight = (EditorGUIUtility.singleLineHeight * 2) + 10
        };
    }
    
    private void CreateNewLevel()
    {
        // Create save dialog
        string path = EditorUtility.SaveFilePanelInProject(
            "Create New Level",
            "Level_New",
            "asset",
            "Create a new level asset");
            
        if (string.IsNullOrEmpty(path)) return;
            
        // Create the level data asset
        LevelData newLevel = CreateInstance<LevelData>();
        
        AssetDatabase.CreateAsset(newLevel, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Update the list and select the new level
        RefreshLevelList();
        
        int newIndex = _allLevels.FindIndex(l => l == newLevel);
        
        if (newIndex >= 0) SelectLevel(newIndex);
    }
    
    private void DuplicateSelectedLevel()
    {
        if (_selectedLevel == null) return;
            
        // Get the path of the original asset
        string originalPath = AssetDatabase.GetAssetPath(_selectedLevel);
        string directory = Path.GetDirectoryName(originalPath);
        string fileName = Path.GetFileNameWithoutExtension(originalPath);
        string extension = Path.GetExtension(originalPath);
        
        // Create a new path
        string newPath = $"{directory}/{fileName}_Copy{extension}";
        
        // Ensure the path is unique
        int copyNum = 1;
        
        while (File.Exists(newPath))
        {
            newPath = $"{directory}/{fileName}_Copy_{copyNum}{extension}";
            copyNum++;
        }
        
        // Duplicate the asset
        bool success = AssetDatabase.CopyAsset(originalPath, newPath);
        if (success)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Update the list and select the duplicated level
            RefreshLevelList();
            
            LevelData duplicatedLevel = AssetDatabase.LoadAssetAtPath<LevelData>(newPath);
            
            int newIndex = _allLevels.FindIndex(l => l == duplicatedLevel);
            
            if (newIndex >= 0) SelectLevel(newIndex);
        }
        else
        {
            Debug.LogError($"Failed to duplicate level: {originalPath}");
        }
    }
    
    private void DeleteSelectedLevel()
    {
        if (_selectedLevel == null)
            return;
            
        string path = AssetDatabase.GetAssetPath(_selectedLevel);
        bool success = AssetDatabase.DeleteAsset(path);
        
        if (success)
        {
            _selectedLevel = null;
            _selectedLevelIndex = -1;
            _serializedLevel = null;
            RefreshLevelList();
        }
        else
        {
            Debug.LogError($"Failed to delete level: {path}");
        }
    }
    
    private void RenameSelectedLevel(string newName)
    {
        if (_selectedLevel == null || string.IsNullOrEmpty(newName))
            return;
            
        string path = AssetDatabase.GetAssetPath(_selectedLevel);
        
        AssetDatabase.RenameAsset(path, newName);
        AssetDatabase.SaveAssets();
    }
}