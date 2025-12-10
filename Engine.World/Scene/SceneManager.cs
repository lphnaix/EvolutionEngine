namespace Engine.World.Scene;

public sealed class SceneManager
{
    public Scene Current { get; private set; }

    public SceneManager(Scene initial)
    {
        Current = initial;
    }

    public void Load(Scene scene)
    {
        Current = scene;
    }
}
