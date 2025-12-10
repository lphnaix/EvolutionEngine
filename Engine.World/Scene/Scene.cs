using Engine.World.Terrain;

namespace Engine.World.Scene;

public sealed class Scene
{
    public TerrainMap Terrain { get; }

    public Scene(TerrainMap terrain)
    {
        Terrain = terrain;
    }
}
