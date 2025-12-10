using System.Numerics;

namespace Engine.Rendering;

public sealed class Camera
{
    public Vector3 Position { get; set; } = new(0, 0, 0);
    public float Width { get; }
    public float Height { get; }
    public float Near { get; }
    public float Far { get; }

    public Camera(float width, float height, float near = -10f, float far = 100f)
    {
        Width = width;
        Height = height;
        Near = near;
        Far = far;
    }

    public Matrix4x4 GetViewMatrix()
    {
        var target = Position + Vector3.UnitZ;
        return Matrix4x4.CreateLookAt(Position, target, Vector3.UnitY);
    }

    public Matrix4x4 GetOrthographicProjection()
    {
        return Matrix4x4.CreateOrthographic(Width, Height, Near, Far);
    }
}
