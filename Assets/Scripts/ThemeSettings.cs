using UnityEngine;

[CreateAssetMenu(fileName = "NewTheme", menuName = "Level Designer/Theme")]
public class ThemeSettings : ScriptableObject
{
    public Color backgroundColor = Color.gray;
    public Color textColor = Color.white;
}