using Engine.Gameplay.Stats;

namespace Engine.Gameplay.Status;

public sealed class BuffManager
{
    private readonly List<Buff> _buffs = new();
    public IReadOnlyList<Buff> Buffs => _buffs;

    public void Add(Buff buff) => _buffs.Add(buff);

    public void Update(float dt)
    {
        foreach (var b in _buffs)
        {
            b.Update(dt);
        }
        _buffs.RemoveAll(b => b.Expired);
    }

    public void ApplyBonuses(StatBlock stats)
    {
        foreach (var b in _buffs)
        {
            stats.AddBonus(b.Stat, b.Amount);
        }
    }
}
