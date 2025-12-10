using System.Diagnostics;
using Engine.Core.Logging;
using Engine.Core.Timing;

namespace Engine.Core;

public interface IGameSystem
{
    void Update(GameTime time);
}

/// <summary>
/// Minimal main loop with semi-fixed timestep (target 60 FPS) and clamp.
/// </summary>
public sealed class GameLoop
{
    private readonly ILogger _logger;
    private readonly List<IGameSystem> _systems = new();
    private readonly TimeSpan _targetDelta = TimeSpan.FromSeconds(1.0 / 60.0);
    private readonly TimeSpan _minDelta = TimeSpan.FromSeconds(1.0 / 90.0);
    private readonly TimeSpan _maxDelta = TimeSpan.FromSeconds(1.0 / 30.0);

    public GameLoop(ILogger logger)
    {
        _logger = logger;
    }

    public void AddSystem(IGameSystem system) => _systems.Add(system);

    public void Run(TimeSpan? runFor = null, CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? CancellationToken.None;
        var stopwatch = Stopwatch.StartNew();
        var last = stopwatch.Elapsed;
        var elapsed = TimeSpan.Zero;

        while (!token.IsCancellationRequested)
        {
            var now = stopwatch.Elapsed;
            var dt = now - last;
            last = now;

            if (dt < _minDelta)
            {
                Thread.Sleep(_minDelta - dt);
                continue;
            }

            if (dt > _maxDelta)
            {
                dt = _maxDelta;
            }

            elapsed += dt;
            var time = new GameTime(dt, elapsed);

            foreach (var system in _systems)
            {
                system.Update(time);
            }

            if (runFor.HasValue && elapsed >= runFor.Value)
            {
                _logger.Log(LogLevel.Info, $"GameLoop exiting after {elapsed.TotalSeconds:F2}s.");
                break;
            }
        }
    }
}
