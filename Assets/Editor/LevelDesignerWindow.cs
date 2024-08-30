using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class LevelDesignerWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private GameObject parentObject;
    private List<GameObject> prefabs = new List<GameObject>();
    private float gridSize = 1f;

    private ThemeSettings currentTheme;
    private LevelDesignerConfig currentConfig;

    [MenuItem("Window/Level Designer")]
    public static void ShowWindow()
    {
        GetWindow<LevelDesignerWindow>("Level Designer");
    }

    private void OnEnable()
    {
        LoadPrefabSet();
    }

    private void OnDisable()
    {
        SavePrefabSet();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        currentTheme = (ThemeSettings)EditorGUILayout.ObjectField("Select Theme", currentTheme, typeof(ThemeSettings), false);

        EditorGUILayout.BeginVertical(EditorStyles.toolbar);

        currentConfig = (LevelDesignerConfig)EditorGUILayout.ObjectField("Select Config", currentConfig, typeof(LevelDesignerConfig), false);

        EditorGUILayout.EndVertical();


        if (currentTheme != null)
        {
            ApplyTheme(currentTheme);
        }

        if (currentConfig != null)
        {
            ApplyConfig(currentConfig);
        }

        EditorGUILayout.EndHorizontal();

        if (currentTheme != null)
        {
            // Apply the theme's background color and text color to this window
            GUI.backgroundColor = currentTheme.backgroundColor;
            GUI.contentColor = currentTheme.textColor;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = currentTheme.textColor;

            EditorGUILayout.LabelField("Level Designer", labelStyle);
        }
        else
        {
            EditorGUILayout.LabelField("Level Designer", EditorStyles.boldLabel);
        }

        if (currentConfig != null)
        {
            gridSize = currentConfig.gridSize;
            prefabs = currentConfig.defaultPrefabs;
        }

        // Draw the rest of the UI
        DrawToolbar();
        DrawMainContent();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Create a new GUIStyle for the toolbar buttons based on the current theme
        GUIStyle toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
        if (currentTheme != null)
        {
            toolbarButtonStyle.normal.textColor = currentTheme.textColor;
        }

        if (GUILayout.Button("Add New Room", toolbarButtonStyle))
        {
            AddNewRoom();
        }
        if (GUILayout.Button("Group Selected", toolbarButtonStyle))
        {
            GroupSelectedObjects();
        }
        if (GUILayout.Button("Align Selected", toolbarButtonStyle))
        {
            AlignSelectedObjects();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Shortcuts: Ctrl+N (New Room), Ctrl+G (Group), Ctrl+A (Align)", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void ApplyTheme(ThemeSettings theme)
    {
        currentTheme = theme;
        Repaint();
    }

    private void ApplyConfig(LevelDesignerConfig config)
    {
        currentConfig = config;
        Repaint();
    }


    private void DrawMainContent()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("Level Designer", EditorStyles.boldLabel);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("Create", GUILayout.Width(60)))
            {
                CreatePrefabInstance(prefabs[i]);
            }
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                prefabs.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Prefab"))
        {
            prefabs.Add(null);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Click 'Create' to add a prefab to the scene, or drag and drop prefabs into the scene to place them.", MessageType.Info);

        EditorGUILayout.EndScrollView();
    }

    private void CreatePrefabInstance(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            //Vector3 position = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            Vector3 position = parentObject.transform.position;
            instance.transform.position = SnapToGrid(position);
            if (parentObject != null)
            {
                instance.transform.SetParent(parentObject.transform);
            }
            Undo.RegisterCreatedObjectUndo(instance, "Create Prefab Instance");
            Selection.activeGameObject = instance;
            SceneView.lastActiveSceneView.FrameSelected();
        }
        else
        {
            Debug.LogWarning("Attempted to create a null prefab.");
        }
    }

    private void AddNewRoom()
    {
        GameObject newRoom = new GameObject("New Room");
        if (parentObject != null)
        {
            newRoom.transform.SetParent(parentObject.transform);
        }
        else
        {
            GameObject levelRoot = GameObject.Find("Level Root");
            if (levelRoot == null)
            {
                levelRoot = new GameObject("Level Root");
                Undo.RegisterCreatedObjectUndo(levelRoot, "Create Level Root");
            }
            newRoom.transform.SetParent(levelRoot.transform);
        }
        newRoom.transform.position = SnapToGrid(newRoom.transform.position);
        Selection.activeGameObject = newRoom;
        parentObject = newRoom;
        Undo.RegisterCreatedObjectUndo(newRoom, "Create New Room");
    }

    private void GroupSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length > 0)
        {
            GameObject groupParent = new GameObject("Grouped Objects");
            Undo.RegisterCreatedObjectUndo(groupParent, "Group Objects");

            foreach (GameObject obj in selectedObjects)
            {
                Undo.SetTransformParent(obj.transform, groupParent.transform, "Group Objects");
            }

            Undo.SetTransformParent(groupParent.transform, parentObject.transform, "Group Objects");

            Selection.activeGameObject = groupParent;
        }
        else
        {
            Debug.Log("No objects selected to group.");
        }
    }

    private void AlignSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length > 1)
        {
            Vector3 averagePosition = selectedObjects.Aggregate(Vector3.zero, (acc, obj) => acc + obj.transform.position) / selectedObjects.Length;
            foreach (GameObject obj in selectedObjects)
            {
                Undo.RecordObject(obj.transform, "Align Objects");
                obj.transform.position = new Vector3(averagePosition.x, obj.transform.position.y, averagePosition.z);
            }
        }
        else
        {
            Debug.Log("Select at least two objects to align.");
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }

    private void SavePrefabSet()
    {
        string prefabSetString = string.Join(";", prefabs.Select(p => p != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(p)) : "null"));
        EditorPrefs.SetString("LevelDesignerPrefabSet", prefabSetString);
    }

    private void LoadPrefabSet()
    {
        string prefabSetString = EditorPrefs.GetString("LevelDesignerPrefabSet", "");
        if (!string.IsNullOrEmpty(prefabSetString))
        {
            prefabs = prefabSetString.Split(';')
                .Select(guid => guid != "null" ? AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)) : null)
                .ToList();
        }
    }
}