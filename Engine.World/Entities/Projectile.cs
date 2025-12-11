namespace Engine.World.Entities;

public sealed class Projectile
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float VX { get; }
    public float VY { get; }
    public float Damage { get; }
    public bool Expired { get; private set; }

    public Projectile(float x, float y, float vx, float vy, float damage)
    {
        X = x;
        Y = y;
        VX = vx;
        VY = vy;
        Damage = damage;
    }

    public void Update(float dt)
    {
        X += VX * dt;
        Y += VY * dt;
    }

    public void Kill() => Expired = true;
}
