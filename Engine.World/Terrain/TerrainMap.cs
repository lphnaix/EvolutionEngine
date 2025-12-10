namespace Engine.World.Terrain;

public sealed class TerrainMap
{
    private readonly Tile[,] _tiles;

    public int Width { get; }
    public int Height { get; }

    public TerrainMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width, height];
    }

    public void SetTile(int x, int y, Tile tile)
    {
        _tiles[x, y] = tile;
    }

    public Tile GetTile(int x, int y) => _tiles[x, y];

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return false;
        return _tiles[x, y].Walkable;
    }
}
