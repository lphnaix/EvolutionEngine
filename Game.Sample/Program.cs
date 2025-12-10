using Engine.Core;
using Engine.Core.Logging;
using Engine.Core.Timing;
using Engine.Data;
using Engine.Data.Config;
using Engine.Platform.Input;
using Engine.Rendering;
using Engine.World.Entities;
using Engine.World.Systems;
using Engine.World.Terrain;

var logger = new ConsoleLogger();
logger.Log(LogLevel.Info, "Bootstrapping sample demo (stages 0-2).");

// Load config
var settingsPath = Path.Combine("Data", "config.json");
var settings = ConfigLoader.Load<EngineSettings>(settingsPath);
logger.Log(LogLevel.Info, $"Loaded config: seed={settings.WorldSeed}, world=({settings.WorldWidth},{settings.WorldHeight}), speed={settings.PlayerSpeed}");

// Generate terrain
var generator = new TerrainGenerator(settings.WorldSeed);
var terrain = generator.Generate(settings.WorldWidth, settings.WorldHeight);
var collision = new CollisionSystem(terrain);

// Create player at center
var player = new Player(settings.WorldWidth / 2f, settings.WorldHeight / 2f, settings.PlayerSpeed);

// Input script: move right, up, left, then exit
var scriptedInput = new[]
{
    new InputState(up:false, down:false, left:false, right:true, exit:false),
    new InputState(up:true, down:false, left:false, right:true, exit:false),
    new InputState(up:false, down:false, left:true, right:false, exit:false),
    new InputState(up:false, down:false, left:false, right:false, exit:true)
};
var input = new DummyInputManager(scriptedInput);

// Renderer placeholder
var renderer = new NullRenderer(logger);
var camera = new Camera(settings.WorldWidth, settings.WorldHeight);

// Game systems
var loop = new GameLoop(logger);
loop.AddSystem(new PlayerControllerSystem(player, input, collision, renderer, logger));

// Limit run time for demo
var cts = new CancellationTokenSource();
loop.Run(TimeSpan.FromSeconds(2), cts.Token);

// Save a snapshot
var savePath = Path.Combine("Saves", "sample.db");
Directory.CreateDirectory("Saves");
using var repo = new SaveRepository(savePath);
repo.SaveSnapshot("autosave", settings.WorldSeed, player.X, player.Y);
logger.Log(LogLevel.Info, $"Saved snapshot to {savePath} at ({player.X:0.00},{player.Y:0.00}).");

logger.Log(LogLevel.Info, "Demo finished.");

/// <summary>
/// Minimal player controller that consumes input, applies collision, and renders.
/// </summary>
internal sealed class PlayerControllerSystem : IGameSystem
{
    private readonly Player _player;
    private readonly IInputManager _input;
    private readonly CollisionSystem _collision;
    private readonly IRenderer _renderer;
    private readonly ILogger _logger;

    public PlayerControllerSystem(Player player, IInputManager input, CollisionSystem collision, IRenderer renderer, ILogger logger)
    {
        _player = player;
        _input = input;
        _collision = collision;
        _renderer = renderer;
        _logger = logger;
    }

    public void Update(GameTime time)
    {
        var state = _input.GetState(time);
        var dirX = 0f;
        var dirY = 0f;
        if (state.Up) dirY -= 1f;
        if (state.Down) dirY += 1f;
        if (state.Left) dirX -= 1f;
        if (state.Right) dirX += 1f;

        var length = MathF.Sqrt(dirX * dirX + dirY * dirY);
        if (length > 1e-5f)
        {
            dirX /= length;
            dirY /= length;
        }

        var dx = dirX * _player.Speed * (float)time.Delta.TotalSeconds;
        var dy = dirY * _player.Speed * (float)time.Delta.TotalSeconds;

        var targetX = _player.X + dx;
        var targetY = _player.Y + dy;
        if (_collision.CanMoveTo(targetX, targetY))
        {
            _player.Move(dx, dy);
        }

        _renderer.Clear();
        _renderer.DrawSprite("player", _player.X, _player.Y);
        _renderer.Present();

        _logger.Log(LogLevel.Info, $"Player at ({_player.X:0.00},{_player.Y:0.00}), dt={time.Delta.TotalMilliseconds:0.0} ms");

        if (state.Exit)
        {
            _logger.Log(LogLevel.Info, "Received exit input, stopping loop soon.");
        }
    }
}
