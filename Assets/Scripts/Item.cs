using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Elixir,
    Other
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    [TextArea(3, 10)]
    public string description;
    [RarityField]
    public Rarity rarity;

    // Weapon-specific fields
    public int damage;
    public float attackSpeed;

    // Armor-specific fields
    public int defense;
    public string armorType;

    // Elixir-specific fields
    public float healAmount;
    public float duration;

    // Other-specific fields
    public string customProperty;
}