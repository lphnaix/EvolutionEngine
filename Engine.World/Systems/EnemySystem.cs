using Engine.World.Entities;

namespace Engine.World.Systems;

public sealed class EnemySystem
{
    private readonly List<Enemy> _enemies = new();
    public IReadOnlyList<Enemy> Enemies => _enemies;

    public void Spawn(Enemy enemy) => _enemies.Add(enemy);

    public void Update(float dt, float targetX, float targetY)
    {
        foreach (var e in _enemies.ToList())
        {
            if (e.Health.IsDead)
            {
                _enemies.Remove(e);
                continue;
            }

            var dx = targetX - e.X;
            var dy = targetY - e.Y;
            var len = MathF.Sqrt(dx * dx + dy * dy);
            if (len > 1e-4f)
            {
                dx /= len;
                dy /= len;
                e.Move(dx * e.Speed * dt, dy * e.Speed * dt);
            }
        }
    }
}
