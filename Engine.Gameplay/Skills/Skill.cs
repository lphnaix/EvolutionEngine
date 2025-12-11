using Engine.Gameplay.Resources;

namespace Engine.Gameplay.Skills;

public enum SkillResource
{
    None,
    Stamina
}

public sealed class Skill
{
    public string Id { get; }
    public float Cooldown { get; }
    public float Cost { get; }
    public SkillResource Resource { get; }
    private float _cooldownTimer;

    public Skill(string id, float cooldown, SkillResource resource, float cost)
    {
        Id = id;
        Cooldown = cooldown;
        Resource = resource;
        Cost = cost;
    }

    public bool TryCast(ResourcePool stamina, float dt)
    {
        if (_cooldownTimer > 0) return false;

        if (Resource == SkillResource.Stamina)
        {
            if (!stamina.Consume(Cost)) return false;
        }

        _cooldownTimer = Cooldown;
        return true;
    }

    public void Update(float dt)
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer = MathF.Max(0, _cooldownTimer - dt);
        }
    }

    public float CooldownRemaining => _cooldownTimer;
}
