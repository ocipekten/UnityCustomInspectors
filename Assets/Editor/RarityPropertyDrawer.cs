using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(RarityFieldAttribute))]
public class RarityPropertyDrawer : PropertyDrawer
{
    private const float IconSize = 20f;
    private const float Padding = 2f;

    // Load icons - make sure these paths match the project structure
    private static Texture2D[] rarityIcons;

    // Initialize icons
    static RarityPropertyDrawer()
    {
        string[] iconPaths = new string[]
        {
            "Assets/Icons/CommonIcon.png",
            "Assets/Icons/UncommonIcon.png",
            "Assets/Icons/RareIcon.png",
            "Assets/Icons/EpicIcon.png",
            "Assets/Icons/LegendaryIcon.png"
        };

        rarityIcons = iconPaths.Select(path => AssetDatabase.LoadAssetAtPath<Texture2D>(path)).ToArray();

        // Debug: Prints loaded icons
        for (int i = 0; i < rarityIcons.Length; i++)
        {
            if (rarityIcons[i] != null)
                Debug.Log($"Loaded icon for rarity {(Rarity)i}: {iconPaths[i]}");
            else
                Debug.LogError($"Failed to load icon for rarity {(Rarity)i}: {iconPaths[i]}");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect iconRect = new Rect(position.x, position.y, IconSize, IconSize);
        Rect popupRect = new Rect(position.x + IconSize + Padding, position.y, position.width - IconSize - Padding, position.height);

        // Get the current enum value
        Rarity currentRarity = (Rarity)property.enumValueIndex;

        // Draw icon
        if (property.enumValueIndex >= 0 && property.enumValueIndex < rarityIcons.Length && rarityIcons[property.enumValueIndex] != null)
        {
            GUI.DrawTexture(iconRect, rarityIcons[property.enumValueIndex]);
        }
        else
        {
            Debug.LogError($"Missing icon for rarity: {currentRarity}");
        }

        // Store the original GUI background color
        Color originalColor = GUI.backgroundColor;

        // Set the background color based on the current rarity
        GUI.backgroundColor = GetRarityColor(currentRarity);

        // Create a style for the enum popup
        GUIStyle style = new GUIStyle(EditorStyles.popup)
        {
            fixedHeight = popupRect.height
        };

        // Draw the enum popup with the custom background color
        EditorGUI.BeginChangeCheck();
        int selectedIndex = EditorGUI.Popup(popupRect, property.enumValueIndex, property.enumNames, style);
        if (EditorGUI.EndChangeCheck())
        {
            property.enumValueIndex = selectedIndex;
        }

        // Restore the original GUI background color
        GUI.backgroundColor = originalColor;

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    private Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return new Color(0.8f, 0.8f, 0.8f); // Light Gray
            case Rarity.Uncommon:
                return new Color(0.0f, 0.7f, 0.0f); // Dark Green
            case Rarity.Rare:
                return new Color(0.0f, 0.4f, 0.8f); // Blue
            case Rarity.Epic:
                return new Color(0.5f, 0.0f, 0.5f); // Purple
            case Rarity.Legendary:
                return new Color(1.0f, 0.8f, 0.0f); // Gold
            default:
                return Color.white;
        }
    }
}