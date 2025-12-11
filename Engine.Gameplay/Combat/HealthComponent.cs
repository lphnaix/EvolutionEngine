namespace Engine.Gameplay.Combat;

public sealed class HealthComponent
{
    public float Max { get; private set; }
    public float Current { get; private set; }
    public bool IsDead => Current <= 0;

    public HealthComponent(float max)
    {
        Max = max;
        Current = max;
    }

    public void ApplyDamage(float amount)
    {
        Current = MathF.Max(0, Current - amount);
    }

    public void Heal(float amount)
    {
        Current = MathF.Min(Max, Current + amount);
    }

    public void SetMax(float max, bool fill = true)
    {
        Max = max;
        if (fill)
        {
            Current = max;
        }
        else
        {
            Current = MathF.Min(Current, Max);
        }
    }
}
