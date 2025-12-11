using Engine.Gameplay.Items;

namespace Engine.Gameplay.Inventory;

public sealed class Inventory
{
    private readonly List<Item> _items = new();
    public IReadOnlyList<Item> Items => _items;

    public void Add(Item item) => _items.Add(item);

    public bool Remove(string id)
    {
        var idx = _items.FindIndex(i => i.Id == id);
        if (idx >= 0)
        {
            _items.RemoveAt(idx);
            return true;
        }

        return false;
    }

    public Item? Find(string id) => _items.FirstOrDefault(i => i.Id == id);
}
