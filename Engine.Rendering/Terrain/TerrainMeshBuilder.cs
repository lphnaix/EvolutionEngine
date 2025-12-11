using Engine.World.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Terrain;

public static class TerrainMeshBuilder
{
    public static TerrainMesh Build(TerrainMap map)
    {
        int w = map.Width;
        int h = map.Height;
        var vertices = new VertexPositionColor[w * h];

        int idx = 0;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float wx = x * map.CellSize;
                float wy = y * map.CellSize;
                float wz = map.GetHeight(x, y);
                var color = HeightToColor(wz);
                vertices[idx++] = new VertexPositionColor(new Vector3(wx, wy, wz), color);
            }
        }

        int quadCount = (w - 1) * (h - 1);
        var indices = new int[quadCount * 6];
        int i = 0;
        for (int y = 0; y < h - 1; y++)
        {
            for (int x = 0; x < w - 1; x++)
            {
                int topLeft = y * w + x;
                int topRight = topLeft + 1;
                int bottomLeft = (y + 1) * w + x;
                int bottomRight = bottomLeft + 1;

                indices[i++] = topLeft;
                indices[i++] = bottomLeft;
                indices[i++] = topRight;

                indices[i++] = topRight;
                indices[i++] = bottomLeft;
                indices[i++] = bottomRight;
            }
        }

        return new TerrainMesh(vertices, indices);
    }

    private static Color HeightToColor(float h)
    {
        float t = Math.Clamp((h + 1f) * 0.5f, 0f, 1f);
        return Color.Lerp(Color.ForestGreen, Color.SandyBrown, t);
    }
}
