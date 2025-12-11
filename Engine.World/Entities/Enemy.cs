using Engine.Gameplay.Combat;

namespace Engine.World.Entities;

public sealed class Enemy
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Speed { get; }
    public HealthComponent Health { get; }

    public Enemy(float x, float y, float speed, float maxHp)
    {
        X = x;
        Y = y;
        Speed = speed;
        Health = new HealthComponent(maxHp);
    }

    public void Move(float dx, float dy)
    {
        X += dx;
        Y += dy;
    }
}
