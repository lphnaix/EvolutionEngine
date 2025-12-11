namespace Engine.World.Entities;

using Engine.Gameplay.Combat;
using Engine.Gameplay.Inventory;
using Engine.Gameplay.Items;
using Engine.Gameplay.Stats;
using Engine.Gameplay.Status;
using Engine.Gameplay.Resources;

public sealed class Player
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public StatBlock Stats { get; } = new();
    public HealthComponent Health { get; }
    public Inventory Inventory { get; } = new();
    public Equipment Equipment { get; } = new();
    public ResourcePool Stamina { get; }
    public BuffManager Buffs { get; } = new();

    public Player(float x, float y, float speed)
    {
        X = x;
        Y = y;
        Stats.SetBase(StatType.MoveSpeed, speed);
        Stats.SetBase(StatType.Damage, 10);
        Stats.SetBase(StatType.MaxHealth, 50);
        Health = new HealthComponent(Stats.Get(StatType.MaxHealth));
        Stamina = new ResourcePool(100, regenPerSecond: 20);
    }

    public void Move(float dx, float dy)
    {
        X += dx;
        Y += dy;
    }

    public void Recalculate()
    {
        Stats.ClearBonuses();
        Equipment.ApplyBonuses(Stats);
        Buffs.ApplyBonuses(Stats);
        Health.SetMax(Stats.Get(StatType.MaxHealth), fill: false);
    }
}
