using Engine.Gameplay.Items;
using Engine.Gameplay.Stats;

namespace SampleApp;

public sealed class ItemsConfig
{
    public string Config_Version { get; set; } = "0.1";
    public List<ItemDto> Items { get; set; } = new();
}

public sealed class ItemDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public ItemType Type { get; set; }
    public Dictionary<string, float>? Bonuses { get; set; }
}
