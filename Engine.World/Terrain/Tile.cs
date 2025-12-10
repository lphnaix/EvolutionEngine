namespace Engine.World.Terrain;

public enum TileType
{
    Unknown = 0,
    Ground = 1,
    Blocked = 2,
    Water = 3
}

public sealed class Tile
{
    public TileType Type { get; }
    public float Height { get; }

    public bool Walkable => Type == TileType.Ground;

    public Tile(TileType type, float height)
    {
        Type = type;
        Height = height;
    }
}
