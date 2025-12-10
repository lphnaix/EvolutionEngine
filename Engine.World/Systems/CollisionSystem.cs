using Engine.World.Terrain;

namespace Engine.World.Systems;

public sealed class CollisionSystem
{
    private readonly TerrainMap _terrain;

    public CollisionSystem(TerrainMap terrain)
    {
        _terrain = terrain;
    }

    public bool CanMoveTo(float x, float y)
    {
        var ix = (int)MathF.Floor(x);
        var iy = (int)MathF.Floor(y);
        return _terrain.IsWalkable(ix, iy);
    }
}
