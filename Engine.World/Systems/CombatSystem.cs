using Engine.World.Entities;

namespace Engine.World.Systems;

public sealed class CombatSystem
{
    public event Action<Enemy>? EnemyKilled;

    public void ResolveProjectiles(IEnumerable<Projectile> projectiles, IList<Enemy> enemies)
    {
        foreach (var p in projectiles)
        {
            if (p.Expired) continue;
            foreach (var e in enemies)
            {
                if (e.Health.IsDead) continue;
                var dx = e.X - p.X;
                var dy = e.Y - p.Y;
                if (dx * dx + dy * dy < 0.5f * 0.5f)
                {
                    e.Health.ApplyDamage(p.Damage);
                    p.Kill();
                    if (e.Health.IsDead)
                    {
                        EnemyKilled?.Invoke(e);
                    }
                    break;
                }
            }
        }
    }
}
