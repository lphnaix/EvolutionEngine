namespace Engine.World.Terrain;

public sealed class TerrainMap
{
    private readonly Tile[,] _tiles;

    public int Width { get; }
    public int Height { get; }
    public float CellSize { get; }

    public TerrainMap(int width, int height, float cellSize = 1f)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
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

    public float GetHeight(int x, int y) => _tiles[x, y].Height;

    public float GetHeightWorld(float worldX, float worldY)
    {
        int tx = (int)(worldX / CellSize);
        int ty = (int)(worldY / CellSize);
        tx = Math.Clamp(tx, 0, Width - 1);
        ty = Math.Clamp(ty, 0, Height - 1);
        return _tiles[tx, ty].Height;
    }
}
