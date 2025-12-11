using Engine.Gameplay.Stats;

namespace Engine.Gameplay.Items;

public sealed class Item
{
    public string Id { get; }
    public string Name { get; }
    public ItemType Type { get; }
    public Dictionary<StatType, float> StatBonuses { get; }
    public int Value { get; }

    public Item(string id, string name, ItemType type, Dictionary<StatType, float>? statBonuses = null, int value = 0)
    {
        Id = id;
        Name = name;
        Type = type;
        StatBonuses = statBonuses ?? new Dictionary<StatType, float>();
        Value = value;
    }
}
