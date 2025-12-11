using Engine.Data.Config;
using Engine.World.Entities;
using Engine.World.Scene;
using Engine.World.Systems;
using Engine.World.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Gameplay.Quests;
using Engine.Gameplay.Items;
using Engine.Gameplay.Stats;
using Engine.Gameplay.Status;
using Engine.Gameplay.Skills;

namespace SampleApp;

/// <summary>
/// MonoGame-based sample：窗口/清屏、占位贴图、键鼠移动、相机跟随，基础战斗、拾取、简单任务/奖励。
/// </summary>
public sealed class SampleGame : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private Texture2D? _playerTexture;
    private Texture2D? _tileTexture;
    private Texture2D? _enemyTexture;
    private Texture2D? _itemTexture;
    private Player? _player;
    private EngineSettings? _settings;
    private SceneManager? _sceneManager;
    private CollisionSystem? _collision;
    private EnemySystem? _enemies;
    private ProjectileSystem? _projectiles;
    private CombatSystem? _combat;
    private Quest? _quest;
    private readonly List<(Item item, Vector2 pos)> _worldItems = new();
    private Skill? _powerShot;
    private Vector2 _camera;
    private const int TileSize = 32;
    private Vector2 _lastDir = new(1, 0);
    private readonly Random _rng = new(42);
    private int _gold;

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
        var terrain = new TerrainGenerator(_settings.WorldSeed).Generate(_settings.WorldWidth, _settings.WorldHeight);
        _sceneManager = new SceneManager(new Scene(terrain));
        _collision = new CollisionSystem(terrain);
        _player = new Player(_settings.WorldWidth / 2f, _settings.WorldHeight / 2f, _settings.PlayerSpeed);
        _enemies = new EnemySystem();
        _projectiles = new ProjectileSystem();
        _combat = new CombatSystem();
        _combat.EnemyKilled += OnEnemyKilled;
        SpawnEnemies(5);
        _quest = new Quest("quest_kill", "消灭5个敌人", 5);
        _powerShot = new Skill("power_shot", cooldown: 2f, resource: SkillResource.Stamina, cost: 25f);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _playerTexture = CreateSolidTexture(GraphicsDevice, 32, 32, Color.OrangeRed);
        _tileTexture = CreateSolidTexture(GraphicsDevice, 1, 1, Color.White);
        _enemyTexture = CreateSolidTexture(GraphicsDevice, 28, 28, Color.MediumPurple);
        _itemTexture = CreateSolidTexture(GraphicsDevice, 20, 20, Color.Gold);
    }

    protected override void Update(GameTime gameTime)
    {
        if (_player is null || _settings is null || _sceneManager is null || _collision is null || _enemies is null || _projectiles is null || _combat is null) return;

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
            _lastDir = dir;
        }

        var moveSpeed = _player.Stats.Get(StatType.MoveSpeed);
        var dx = dir.X * moveSpeed * dt;
        var dy = dir.Y * moveSpeed * dt;

        var targetX = _player.X + dx;
        var targetY = _player.Y + dy;
        if (_collision.CanMoveTo(targetX, targetY))
        {
            _player.Move(dx, dy);
        }

        if (kb.IsKeyDown(Keys.Space))
        {
            Shoot(_player, _projectiles, _lastDir);
        }

        if (kb.IsKeyDown(Keys.LeftShift) && _powerShot != null && _powerShot.TryCast(_player.Stamina, dt))
        {
            Shoot(_player, _projectiles, _lastDir, damageBonus: 20);
        }

        if (kb.IsKeyDown(Keys.Q))
        {
            UseSpeedPotion(_player);
        }

        if (kb.IsKeyDown(Keys.E))
        {
            TryPickup(_player);
        }

        _powerShot?.Update(dt);
        _player.Stamina.Update(dt);
        _player.Buffs.Update(dt);
        _player.Recalculate();

        _projectiles.Update(dt);
        _combat.ResolveProjectiles(_projectiles.Projectiles, _enemies.Enemies as IList<Enemy>);
        _enemies.Update(dt, _player.X, _player.Y);

        // simple item pickup: drop gold on kills stored via _gold
        // quest turn-in when completed and player presses T
        if (_quest != null && _quest.Status == QuestStatus.Completed && kb.IsKeyDown(Keys.T))
        {
            _quest.TurnIn();
            _gold += 50;
        }

        _camera = new Vector2(_player.X * TileSize, _player.Y * TileSize);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        if (_spriteBatch is null || _playerTexture is null || _tileTexture is null || _player is null || _sceneManager is null || _enemyTexture is null || _projectiles is null || _itemTexture is null) return;

        var viewport = GraphicsDevice.Viewport;
        var transform = Matrix.CreateTranslation(
            -_camera.X + viewport.Width / 2f - _playerTexture.Width / 2f,
            -_camera.Y + viewport.Height / 2f - _playerTexture.Height / 2f,
            0f);

        _spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);

        // draw terrain tiles
        var terrain = _sceneManager.Current.Terrain;
        for (var x = 0; x < terrain.Width; x++)
        {
            for (var y = 0; y < terrain.Height; y++)
            {
                var tile = terrain.GetTile(x, y);
                var color = tile.Type switch
                {
                    TileType.Blocked => Color.DarkSlateGray,
                    TileType.Water => Color.SteelBlue,
                    _ => Color.Lerp(Color.ForestGreen, Color.LightGreen, (tile.Height + 1f) * 0.5f)
                };
                var dest = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                _spriteBatch.Draw(_tileTexture, dest, color);
            }
        }

        // draw enemies
        foreach (var e in _enemies!.Enemies)
        {
            var pos = new Vector2(e.X * TileSize, e.Y * TileSize);
            _spriteBatch.Draw(_enemyTexture, pos, Color.White);
            DrawBar(_spriteBatch, pos + new Vector2(0, -8), e.Health.Current / e.Health.Max, Color.Red);
        }

        // draw projectiles
        foreach (var p in _projectiles.Projectiles)
        {
            var rect = new Rectangle((int)(p.X * TileSize), (int)(p.Y * TileSize), 8, 8);
            _spriteBatch.Draw(_tileTexture, rect, Color.Yellow);
        }

        // world items
        foreach (var wi in _worldItems)
        {
            var dest = new Rectangle((int)wi.pos.X, (int)wi.pos.Y, 16, 16);
            _spriteBatch.Draw(_itemTexture, dest, Color.White);
        }

        _spriteBatch.Draw(_playerTexture, new Vector2(_player.X * TileSize, _player.Y * TileSize), Color.White);
        DrawBar(_spriteBatch, new Vector2(_player.X * TileSize, _player.Y * TileSize - 10), _player.Health.Current / _player.Health.Max, Color.LimeGreen);
        DrawHud();
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private static EngineSettings LoadSettings()
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "Data", "config.json"));
        return ConfigLoader.Load<EngineSettings>(path);
    }

    private void DropItem(float x, float y)
    {
        // 50% 概率掉落武器，50% 掉落速度药水
        var roll = _rng.NextDouble();
        Item item;
        if (roll < 0.5)
        {
            item = new Item("weapon_basic", "基础武器", ItemType.Weapon, new Dictionary<StatType, float> { { StatType.Damage, 5 } });
        }
        else
        {
            item = new Item("potion_speed", "速度药水", ItemType.Consumable, new Dictionary<StatType, float> { { StatType.MoveSpeed, 2 } });
        }

        _worldItems.Add((item, new Vector2(x * TileSize, y * TileSize)));
    }

    private void TryPickup(Player player)
    {
        for (int i = 0; i < _worldItems.Count; i++)
        {
            var (item, pos) = _worldItems[i];
            var dx = player.X * TileSize - pos.X;
            var dy = player.Y * TileSize - pos.Y;
            if (dx * dx + dy * dy < (TileSize * TileSize))
            {
                player.Inventory.Add(item);
                _worldItems.RemoveAt(i);
                AutoEquip(player, item);
                break;
            }
        }
    }

    private void AutoEquip(Player player, Item item)
    {
        if (item.Type is ItemType.Weapon or ItemType.Armor)
        {
            player.Equipment.Equip(item);
            player.Recalculate();
        }
    }

    private void UseSpeedPotion(Player player)
    {
        var potion = player.Inventory.Items.FirstOrDefault(i => i.Id == "potion_speed");
        if (potion == null) return;
        player.Inventory.Remove(potion.Id);
        player.Buffs.Add(new Buff(StatType.MoveSpeed, amount: 2, duration: 5));
    }

    private void Shoot(Player player, ProjectileSystem system, Vector2 dir, float damageBonus = 0)
    {
        var speed = 10f;
        var damage = player.Stats.Get(StatType.Damage) + damageBonus;
        var proj = new Projectile(player.X, player.Y, dir.X * speed, dir.Y * speed, damage);
        system.Spawn(proj);
    }

    private void SpawnEnemies(int count)
    {
        if (_enemies == null || _settings == null) return;
        for (int i = 0; i < count; i++)
        {
            var ex = (float)_rng.NextDouble() * _settings.WorldWidth;
            var ey = (float)_rng.NextDouble() * _settings.WorldHeight;
            _enemies.Spawn(new Enemy(ex, ey, speed: 1.5f, maxHp: 20));
        }
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        _quest?.OnKill();
        _gold += 5;
        DropItem(enemy.X, enemy.Y);
    }

    private void DrawBar(SpriteBatch batch, Vector2 position, float pct, Color color)
    {
        pct = Math.Clamp(pct, 0, 1);
        var back = new Rectangle((int)position.X, (int)position.Y, 32, 4);
        var front = new Rectangle((int)position.X, (int)position.Y, (int)(32 * pct), 4);
        batch.Draw(_tileTexture!, back, Color.Black * 0.5f);
        batch.Draw(_tileTexture!, front, color);
    }

    private void DrawHud()
    {
        if (_spriteBatch == null || _tileTexture == null || _player == null) return;
        var hpPct = _player.Health.Current / _player.Health.Max;
        DrawBar(_spriteBatch, new Vector2(10, 10), hpPct, Color.LimeGreen);

        // gold / quest indicators as simple colored dots
        var goldWidth = Math.Min(100, _gold);
        _spriteBatch.Draw(_tileTexture, new Rectangle(10, 20, goldWidth, 4), Color.Gold);

        if (_quest != null)
        {
            var color = _quest.Status switch
            {
                QuestStatus.InProgress => Color.Orange,
                QuestStatus.Completed => Color.Cyan,
                QuestStatus.TurnedIn => Color.Gray,
                _ => Color.Red
            };
            _spriteBatch.Draw(_tileTexture, new Rectangle(10, 28, 40, 4), color);
        }

        // stamina bar
        var staminaPct = _player.Stamina.Current / _player.Stamina.Max;
        _spriteBatch.Draw(_tileTexture, new Rectangle(10, 36, 100, 4), Color.DarkSlateGray);
        _spriteBatch.Draw(_tileTexture, new Rectangle(10, 36, (int)(100 * staminaPct), 4), Color.DeepSkyBlue);
    }

    private static Texture2D CreateSolidTexture(GraphicsDevice device, int width, int height, Color color)
    {
        var tex = new Texture2D(device, width, height);
        var data = Enumerable.Repeat(color, width * height).ToArray();
        tex.SetData(data);
        return tex;
    }
}
