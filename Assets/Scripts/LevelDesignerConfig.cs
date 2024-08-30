using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDesignerConfig", menuName = "Level Designer/Configuration")]
public class LevelDesignerConfig : ScriptableObject
{
    public List<GameObject> defaultPrefabs;
    public float gridSize = 1f;
}
