using Engine.World.Entities;

namespace Engine.World.Systems;

public sealed class ProjectileSystem
{
    private readonly List<Projectile> _projectiles = new();
    public IReadOnlyList<Projectile> Projectiles => _projectiles;

    public void Spawn(Projectile p) => _projectiles.Add(p);

    public void Update(float dt)
    {
        foreach (var p in _projectiles)
        {
            if (!p.Expired)
            {
                p.Update(dt);
            }
        }

        _projectiles.RemoveAll(p => p.Expired);
    }
}
