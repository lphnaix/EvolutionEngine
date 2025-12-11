using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering.Sprites;

public readonly struct BillboardSprite
{
    public Texture2D Texture { get; }
    public Rectangle? SourceRect { get; }
    public Vector3 WorldPosition { get; }
    public float WorldWidth { get; }
    public float WorldHeight { get; }
    public Color Tint { get; }

    public BillboardSprite(Texture2D texture, Rectangle? sourceRect, Vector3 worldPosition, float worldWidth, float worldHeight, Color tint)
    {
        Texture = texture;
        SourceRect = sourceRect;
        WorldPosition = worldPosition;
        WorldWidth = worldWidth;
        WorldHeight = worldHeight;
        Tint = tint;
    }
}
