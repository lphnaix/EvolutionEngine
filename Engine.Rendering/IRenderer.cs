using Engine.Core.Logging;

namespace Engine.Rendering;

public interface IRenderer
{
    void Clear();
    void DrawSprite(string id, float x, float y);
    void Present();
}

/// <summary>
/// Headless renderer placeholder that just logs draw calls.
/// </summary>
public sealed class NullRenderer : IRenderer
{
    private readonly ILogger _logger;

    public NullRenderer(ILogger logger)
    {
        _logger = logger;
    }

    public void Clear()
    {
        _logger.Log(LogLevel.Debug, "Renderer.Clear()");
    }

    public void DrawSprite(string id, float x, float y)
    {
        _logger.Log(LogLevel.Debug, $"DrawSprite {id} at ({x:0.00},{y:0.00})");
    }

    public void Present()
    {
        _logger.Log(LogLevel.Debug, "Renderer.Present()");
    }
}
