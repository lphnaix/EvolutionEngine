namespace Engine.Data.Config;

public sealed class EngineSettings
{
    public float PlayerSpeed { get; init; } = 5.0f;
    public int WorldWidth { get; init; } = 32;
    public int WorldHeight { get; init; } = 32;
    public int WorldSeed { get; init; } = 1337;
}
