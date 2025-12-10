using Engine.Data.Config;
using Engine.World.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SampleApp;

/// <summary>
/// MonoGame-based sample satisfying阶段1：窗口/清屏、占位贴图、键鼠移动、相机跟随。
/// </summary>
public sealed class SampleGame : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private Texture2D? _playerTexture;
    private Player? _player;
    private EngineSettings? _settings;
    private Vector2 _camera;

    public SampleGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            SynchronizeWithVerticalRetrace = true
        };
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _settings = LoadSettings();
        _player = new Player(_settings.WorldWidth / 2f, _settings.WorldHeight / 2f, _settings.PlayerSpeed);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _playerTexture = CreateSolidTexture(GraphicsDevice, 32, 32, Color.OrangeRed);
    }

    protected override void Update(GameTime gameTime)
    {
        if (_player is null || _settings is null) return;

        var kb = Keyboard.GetState();
        if (kb.IsKeyDown(Keys.Escape))
        {
            Exit();
            return;
        }

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var dir = Vector2.Zero;
        if (kb.IsKeyDown(Keys.W)) dir.Y -= 1f;
        if (kb.IsKeyDown(Keys.S)) dir.Y += 1f;
        if (kb.IsKeyDown(Keys.A)) dir.X -= 1f;
        if (kb.IsKeyDown(Keys.D)) dir.X += 1f;
        if (dir.LengthSquared() > 0)
        {
            dir = Vector2.Normalize(dir);
        }

        var dx = dir.X * _player.Speed * dt;
        var dy = dir.Y * _player.Speed * dt;

        var targetX = Math.Clamp(_player.X + dx, 0, _settings.WorldWidth);
        var targetY = Math.Clamp(_player.Y + dy, 0, _settings.WorldHeight);
        _player.Move(targetX - _player.X, targetY - _player.Y);

        _camera = new Vector2(_player.X, _player.Y);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        if (_spriteBatch is null || _playerTexture is null || _player is null) return;

        var viewport = GraphicsDevice.Viewport;
        var transform = Matrix.CreateTranslation(
            -_camera.X + viewport.Width / 2f - _playerTexture.Width / 2f,
            -_camera.Y + viewport.Height / 2f - _playerTexture.Height / 2f,
            0f);

        _spriteBatch.Begin(transformMatrix: transform);
        _spriteBatch.Draw(_playerTexture, new Vector2(_player.X, _player.Y), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private static EngineSettings LoadSettings()
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "Data", "config.json"));
        return ConfigLoader.Load<EngineSettings>(path);
    }

    private static Texture2D CreateSolidTexture(GraphicsDevice device, int width, int height, Color color)
    {
        var tex = new Texture2D(device, width, height);
        var data = Enumerable.Repeat(color, width * height).ToArray();
        tex.SetData(data);
        return tex;
    }
}
