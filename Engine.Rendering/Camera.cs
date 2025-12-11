using Microsoft.Xna.Framework;

namespace Engine.Rendering;

public sealed class Camera
{
    public Vector3 Position { get; set; } = new(0, -10, 10);
    public Vector3 Target { get; set; } = Vector3.Zero;
    public Vector3 Up { get; set; } = Vector3.UnitZ;

    public float ViewWidth { get; }
    public float ViewHeight { get; }
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 500f;

    public Camera(float viewWidth, float viewHeight)
    {
        ViewWidth = viewWidth;
        ViewHeight = viewHeight;
    }

    public Matrix View => Matrix.CreateLookAt(Position, Target, Up);

    public Matrix Projection => Matrix.CreateOrthographic(ViewWidth, ViewHeight, NearPlane, FarPlane);

    public void Follow(Vector3 focus, float distanceBack, float height, float tiltRadians = 0f)
    {
        var offset = new Vector3(0, -distanceBack, height);
        Position = focus + offset;
        if (tiltRadians != 0)
        {
            var rotation = Matrix.CreateRotationX(tiltRadians);
            offset = Vector3.Transform(offset, rotation);
            Position = focus + offset;
        }
        Target = focus;
    }
}
