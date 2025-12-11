namespace Engine.Gameplay.Stats;

public sealed class StatBlock
{
    private readonly Dictionary<StatType, float> _baseValues = new();
    private readonly Dictionary<StatType, float> _bonusValues = new();

    public void SetBase(StatType type, float value) => _baseValues[type] = value;

    public void SetBonus(StatType type, float value) => _bonusValues[type] = value;

    public float Get(StatType type)
    {
        _baseValues.TryGetValue(type, out var b);
        _bonusValues.TryGetValue(type, out var bonus);
        return b + bonus;
    }
}
