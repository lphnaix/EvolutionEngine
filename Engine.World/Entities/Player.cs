namespace Engine.World.Entities;

public sealed class Player
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Speed { get; }

    public Player(float x, float y, float speed)
    {
        X = x;
        Y = y;
        Speed = speed;
    }

    public void Move(float dx, float dy)
    {
        X += dx;
        Y += dy;
    }
}
