using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Terrain;

public sealed class TerrainMesh
{
    public VertexPositionColor[] Vertices { get; }
    public int[] Indices { get; }
    public int PrimitiveCount => Indices.Length / 3;

    public TerrainMesh(VertexPositionColor[] vertices, int[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
}
