using Engine.Gameplay.Items;
using Engine.Gameplay.Stats;

namespace Engine.Gameplay.Inventory;

public sealed class Equipment
{
    private Item? _weapon;
    private Item? _armor;

    public Item? Weapon => _weapon;
    public Item? Armor => _armor;

    public void Equip(Item item)
    {
        switch (item.Type)
        {
            case ItemType.Weapon:
                _weapon = item;
                break;
            case ItemType.Armor:
                _armor = item;
                break;
        }
    }

    public void ApplyBonuses(StatBlock stats)
    {
        foreach (var kv in EnumerateBonuses())
        {
            stats.SetBonus(kv.Key, kv.Value);
        }
    }

    private IEnumerable<KeyValuePair<StatType, float>> EnumerateBonuses()
    {
        if (_weapon != null)
        {
            foreach (var kv in _weapon.StatBonuses) yield return kv;
        }
        if (_armor != null)
        {
            foreach (var kv in _armor.StatBonuses) yield return kv;
        }
    }
}
