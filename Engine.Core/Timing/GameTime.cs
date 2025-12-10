namespace Engine.Core.Timing;

public readonly struct GameTime
{
    public TimeSpan Delta { get; }
    public TimeSpan Elapsed { get; }

    public GameTime(TimeSpan delta, TimeSpan elapsed)
    {
        Delta = delta;
        Elapsed = elapsed;
    }
}
