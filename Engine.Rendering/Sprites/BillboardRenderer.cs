using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Sprites;

public sealed class BillboardRenderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly BasicEffect _effect;
    private DynamicVertexBuffer? _vb;
    private IndexBuffer? _ib;
    private int _maxQuads = 0;

    public BillboardRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _effect = new BasicEffect(graphicsDevice)
        {
            TextureEnabled = true,
            VertexColorEnabled = true,
            LightingEnabled = false
        };
    }

    public void Draw(Camera camera, IReadOnlyList<BillboardSprite> sprites)
    {
        if (sprites.Count == 0) return;

        // group by texture to allow different textures in one draw cycle
        var groups = sprites.GroupBy(s => s.Texture);

        var camForward = Vector3.Normalize(camera.Target - camera.Position);
        var worldUp = Vector3.UnitZ;
        var right = Vector3.Normalize(Vector3.Cross(camForward, worldUp));
        var up = Vector3.Normalize(Vector3.Cross(right, -camForward));

        foreach (var group in groups)
        {
            var list = group.ToList();
            EnsureBuffers(list.Count);

            var verts = new VertexPositionColorTexture[list.Count * 4];
            int v = 0;
            foreach (var s in list)
            {
                float w = s.WorldWidth;
                float h = s.WorldHeight;
                var bottomCenter = s.WorldPosition;

                var v0Pos = bottomCenter - right * (w / 2); // left bottom
                var v1Pos = bottomCenter + right * (w / 2); // right bottom
                var v2Pos = v0Pos + up * h;                 // left top
                var v3Pos = v1Pos + up * h;                 // right top

                Rectangle src = s.SourceRect ?? s.Texture.Bounds;
                float texW = s.Texture.Width;
                float texH = s.Texture.Height;
                float u0 = src.X / texW;
                float v0t = src.Y / texH;
                float u1 = (src.X + src.Width) / texW;
                float v1t = (src.Y + src.Height) / texH;

                verts[v++] = new VertexPositionColorTexture(v0Pos, s.Tint, new Vector2(u0, v1t));
                verts[v++] = new VertexPositionColorTexture(v1Pos, s.Tint, new Vector2(u1, v1t));
                verts[v++] = new VertexPositionColorTexture(v2Pos, s.Tint, new Vector2(u0, v0t));
                verts[v++] = new VertexPositionColorTexture(v3Pos, s.Tint, new Vector2(u1, v0t));
            }

            _vb!.SetData(verts, 0, verts.Length, SetDataOptions.Discard);
            _graphicsDevice.SetVertexBuffer(_vb);
            _graphicsDevice.Indices = _ib;

            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;

            _effect.TextureEnabled = true;
            _effect.Texture = group.Key;
            _effect.World = Matrix.Identity;
            _effect.View = camera.View;
            _effect.Projection = camera.Projection;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    list.Count * 2);
            }
        }
    }

    private void EnsureBuffers(int quadCount)
    {
        if (quadCount <= _maxQuads && _vb != null && _ib != null) return;
        _maxQuads = Math.Max(quadCount, _maxQuads * 2 + 4);

        _vb = new DynamicVertexBuffer(_graphicsDevice, typeof(VertexPositionColorTexture), _maxQuads * 4, BufferUsage.WriteOnly);
        var indices = new int[_maxQuads * 6];
        int i = 0;
        int v = 0;
        for (int q = 0; q < _maxQuads; q++)
        {
            indices[i++] = v;
            indices[i++] = v + 1;
            indices[i++] = v + 2;
            indices[i++] = v + 2;
            indices[i++] = v + 1;
            indices[i++] = v + 3;
            v += 4;
        }

        _ib = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
        _ib.SetData(indices);
    }
}
