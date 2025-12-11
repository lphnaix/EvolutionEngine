using Engine.Gameplay.Stats;

namespace Engine.Gameplay.Status;

public sealed class Buff
{
    public StatType Stat { get; }
    public float Amount { get; }
    public float Duration { get; private set; }

    public bool Expired => Duration <= 0;

    public Buff(StatType stat, float amount, float duration)
    {
        Stat = stat;
        Amount = amount;
        Duration = duration;
    }

    public void Update(float dt) => Duration -= dt;
}
