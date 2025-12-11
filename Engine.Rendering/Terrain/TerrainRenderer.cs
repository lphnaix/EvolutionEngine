using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.World.Terrain;

namespace Engine.Rendering.Terrain;

public sealed class TerrainRenderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly BasicEffect _effect;
    private VertexBuffer? _vb;
    private IndexBuffer? _ib;
    private int _primitiveCount;

    public TerrainRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };
    }

    public void SetMesh(TerrainMesh mesh)
    {
        _vb = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), mesh.Vertices.Length, BufferUsage.WriteOnly);
        _vb.SetData(mesh.Vertices);

        _ib = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, mesh.Indices.Length, BufferUsage.WriteOnly);
        _ib.SetData(mesh.Indices);

        _primitiveCount = mesh.PrimitiveCount;
    }

    public void Draw(Camera camera, Matrix world)
    {
        if (_vb == null || _ib == null) return;

        _graphicsDevice.SetVertexBuffer(_vb);
        _graphicsDevice.Indices = _ib;

        _effect.World = world;
        _effect.View = camera.View;
        _effect.Projection = camera.Projection;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);
        }
    }
}
