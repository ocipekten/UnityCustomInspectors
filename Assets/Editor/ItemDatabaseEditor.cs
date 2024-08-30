using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    private ItemDatabase itemDatabase;
    private Vector2 scrollPosition;
    private Dictionary<int, Editor> itemEditors = new Dictionary<int, Editor>();

    private GUIStyle headerStyle;
    private GUIStyle itemStyle;

    private void DrawItemProperties(Item item)
    {
        SerializedObject serializedItem = new SerializedObject(item);
        serializedItem.Update();

        DrawPropertyField(serializedItem, "itemName");
        DrawPropertyField(serializedItem, "itemType");
        DrawPropertyField(serializedItem, "icon");
        DrawPropertyField(serializedItem, "description");
        DrawPropertyField(serializedItem, "rarity");

        EditorGUILayout.Space(10);

        switch (item.itemType)
        {
            case ItemType.Weapon:
                DrawWeaponFields(serializedItem);
                break;
            case ItemType.Armor:
                DrawArmorFields(serializedItem);
                break;
            case ItemType.Elixir:
                DrawElixirFields(serializedItem);
                break;
            case ItemType.Other:
                DrawOtherFields(serializedItem);
                break;
        }

        serializedItem.ApplyModifiedProperties();
    }

    private void DrawPropertyField(SerializedObject serializedObject, string propertyName)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        Color originalColor = EditorStyles.label.normal.textColor;
        EditorStyles.label.normal.textColor = Color.white;
        EditorGUILayout.PropertyField(property);
        EditorStyles.label.normal.textColor = originalColor;
    }

    private void DrawWeaponFields(SerializedObject serializedItem)
    {
        EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);
        DrawPropertyField(serializedItem, "damage");
        DrawPropertyField(serializedItem, "attackSpeed");
    }

    private void DrawArmorFields(SerializedObject serializedItem)
    {
        EditorGUILayout.LabelField("Armor Properties", EditorStyles.boldLabel);
        DrawPropertyField(serializedItem, "defense");
        DrawPropertyField(serializedItem, "armorType");
    }

    private void DrawElixirFields(SerializedObject serializedItem)
    {
        EditorGUILayout.LabelField("Elixir Properties", EditorStyles.boldLabel);
        DrawPropertyField(serializedItem, "healAmount");
        DrawPropertyField(serializedItem, "duration");
    }

    private void DrawOtherFields(SerializedObject serializedItem)
    {
        EditorGUILayout.LabelField("Other Properties", EditorStyles.boldLabel);
        DrawPropertyField(serializedItem, "customProperty");
    }

    private void OnEnable()
    {
        itemDatabase = (ItemDatabase)target;
        InitializeStyles();
        EnsureFoldoutStatesMatch();
    }


    private void InitializeStyles()
    {
        try
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel ?? EditorStyles.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 5, 5, 5)
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating headerStyle: {e.Message}");
            headerStyle = new GUIStyle
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 5, 5, 5)
            };
        }

        try
        {
            itemStyle = new GUIStyle(EditorStyles.helpBox ?? EditorStyles.textArea)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating itemStyle: {e.Message}");
            itemStyle = new GUIStyle
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };
        }
    }

    private void EnsureFoldoutStatesMatch()
    {
        while (itemDatabase.itemFoldoutStates.Count < itemDatabase.items.Count)
        {
            itemDatabase.itemFoldoutStates.Add(false);
        }
        while (itemDatabase.itemFoldoutStates.Count > itemDatabase.items.Count)
        {
            itemDatabase.itemFoldoutStates.RemoveAt(itemDatabase.itemFoldoutStates.Count - 1);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Item Database", headerStyle);
        EditorGUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int itemToRemove = -1;

        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            EditorGUILayout.BeginVertical(itemStyle);

            EditorGUILayout.BeginHorizontal();

            // Draw foldout with item name
            string itemName = itemDatabase.items[i] != null ? itemDatabase.items[i].itemName : "Null Item";
            itemDatabase.itemFoldoutStates[i] = EditorGUILayout.Foldout(itemDatabase.itemFoldoutStates[i], itemName, true, EditorStyles.foldoutHeader);

            // Draw remove button
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (EditorUtility.DisplayDialog("Remove Item", $"Are you sure you want to remove this item?", "Yes", "No"))
                {
                    itemToRemove = i;
                }
            }

            EditorGUILayout.EndHorizontal();

            // If foldout is open and item is not null, draw item properties
            if (itemDatabase.itemFoldoutStates[i] && itemDatabase.items[i] != null)
            {
                EditorGUI.indentLevel++;
                DrawItemProperties(itemDatabase.items[i]);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();

        // Handle item removal
        if (itemToRemove != -1)
        {
            RemoveItem(itemToRemove);
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add New Item", GUILayout.Height(30)))
        {
            AddNewItem();
        }

        if (GUILayout.Button("Clear All Items", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear All Items", "Are you sure you want to remove all items from the database?", "Yes", "No"))
            {
                ClearAllItems();
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(itemDatabase);
        }
    }

    private void RemoveItem(int index)
    {
        Item itemToRemove = itemDatabase.items[index];
        itemDatabase.items.RemoveAt(index);
        itemDatabase.itemFoldoutStates.RemoveAt(index);

        // Remove the item editor
        if (itemEditors.ContainsKey(index))
        {
            DestroyImmediate(itemEditors[index]);
            itemEditors.Remove(index);
        }

        // Shift the remaining editors
        for (int i = index; i < itemDatabase.items.Count; i++)
        {
            if (itemEditors.ContainsKey(i + 1))
            {
                itemEditors[i] = itemEditors[i + 1];
                itemEditors.Remove(i + 1);
            }
        }

        // Delete the asset
        string path = AssetDatabase.GetAssetPath(itemToRemove);
        AssetDatabase.RemoveObjectFromAsset(itemToRemove);
        AssetDatabase.SaveAssets();

        // Destroy the object
        DestroyImmediate(itemToRemove, true);
    }

    private void AddNewItem()
    {
        Item newItem = CreateInstance<Item>();
        newItem.itemName = "New Item";
        newItem.name = "New Item";
        AssetDatabase.AddObjectToAsset(newItem, itemDatabase);
        itemDatabase.items.Add(newItem);
        itemDatabase.itemFoldoutStates.Add(true);
        AssetDatabase.SaveAssets();
    }

    private void ClearAllItems()
    {
        for (int i = itemDatabase.items.Count - 1; i >= 0; i--)
        {
            RemoveItem(i);
        }
        itemDatabase.items.Clear();
        itemDatabase.itemFoldoutStates.Clear();
        itemEditors.Clear();
        AssetDatabase.SaveAssets();
    }

    private void OnDisable()
    {
        // Cleanup item editors
        foreach (var editor in itemEditors.Values)
        {
            DestroyImmediate(editor);
        }
        itemEditors.Clear();
    }
}