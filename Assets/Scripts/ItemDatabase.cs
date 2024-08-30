using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>();

    [HideInInspector] public List<bool> itemFoldoutStates = new List<bool>();
}