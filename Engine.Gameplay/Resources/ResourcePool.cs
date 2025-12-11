namespace Engine.Gameplay.Resources;

public sealed class ResourcePool
{
    public float Max { get; private set; }
    public float Current { get; private set; }
    public float RegenPerSecond { get; set; }

    public ResourcePool(float max, float regenPerSecond)
    {
        Max = max;
        Current = max;
        RegenPerSecond = regenPerSecond;
    }

    public bool Consume(float amount)
    {
        if (Current < amount) return false;
        Current -= amount;
        return true;
    }

    public void Update(float dt)
    {
        if (Current < Max)
        {
            Current = MathF.Min(Max, Current + RegenPerSecond * dt);
        }
    }

    public void SetMax(float max, bool fill = true)
    {
        Max = max;
        if (fill) Current = Max;
        else Current = MathF.Min(Current, Max);
    }
}
