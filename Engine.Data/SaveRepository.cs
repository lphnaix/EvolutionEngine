using Microsoft.Data.Sqlite;

namespace Engine.Data;

/// <summary>
/// SQLite save repository (v0.2 schema-lite) with version check and simple backup.
/// </summary>
public sealed class SaveRepository : IDisposable
{
    public const int SAVE_SCHEMA_VERSION = 1;
    private readonly SqliteConnection _connection;
    private readonly string _path;

    public SaveRepository(string path)
    {
        _path = path;
        _connection = new SqliteConnection($"Data Source={path}");
        _connection.Open();
        EnsureSchema();
    }

    private void EnsureSchema()
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Saves(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                created_at INTEGER NOT NULL,
                updated_at INTEGER NOT NULL,
                play_time INTEGER NOT NULL DEFAULT 0,
                world_seed INTEGER NOT NULL,
                schema_version INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS PlayerState(
                save_id INTEGER PRIMARY KEY,
                scene_id TEXT,
                pos_x REAL NOT NULL,
                pos_y REAL NOT NULL,
                pos_z REAL NOT NULL,
                hp REAL NOT NULL,
                stamina REAL NOT NULL,
                FOREIGN KEY(save_id) REFERENCES Saves(id) ON DELETE CASCADE
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public void SaveSnapshot(string name, int seed, float playerX, float playerY, float playerZ, float hp, float stamina, string sceneId = "default", int playTime = 0)
    {
        Backup();
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        using var tx = _connection.BeginTransaction();
        long saveId;
        using (var cmd = _connection.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = """
                INSERT INTO Saves(name, created_at, updated_at, play_time, world_seed, schema_version)
                VALUES ($name, $created_at, $updated_at, $play_time, $seed, $schema_version);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$created_at", now);
            cmd.Parameters.AddWithValue("$updated_at", now);
            cmd.Parameters.AddWithValue("$play_time", playTime);
            cmd.Parameters.AddWithValue("$seed", seed);
            cmd.Parameters.AddWithValue("$schema_version", SAVE_SCHEMA_VERSION);
            saveId = (long)cmd.ExecuteScalar()!;
        }

        using (var cmd = _connection.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = """
                INSERT OR REPLACE INTO PlayerState(save_id, scene_id, pos_x, pos_y, pos_z, hp, stamina)
                VALUES ($save_id, $scene_id, $x, $y, $z, $hp, $stamina);
                """;
            cmd.Parameters.AddWithValue("$save_id", saveId);
            cmd.Parameters.AddWithValue("$scene_id", sceneId);
            cmd.Parameters.AddWithValue("$x", playerX);
            cmd.Parameters.AddWithValue("$y", playerY);
            cmd.Parameters.AddWithValue("$z", playerZ);
            cmd.Parameters.AddWithValue("$hp", hp);
            cmd.Parameters.AddWithValue("$stamina", stamina);
            cmd.ExecuteNonQuery();
        }
        tx.Commit();
    }

    public (float x, float y, float z, int seed)? LoadLatest()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT s.schema_version, s.world_seed, p.pos_x, p.pos_y, p.pos_z
            FROM Saves s
            JOIN PlayerState p ON p.save_id = s.id
            ORDER BY s.updated_at DESC
            LIMIT 1;
            """;

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var version = reader.GetInt32(0);
            if (version != SAVE_SCHEMA_VERSION)
            {
                throw new InvalidOperationException($"Save schema mismatch. Expected {SAVE_SCHEMA_VERSION}, got {version}");
            }
            int seed = reader.GetInt32(1);
            float x = reader.GetFloat(2);
            float y = reader.GetFloat(3);
            float z = reader.GetFloat(4);
            return (x, y, z, seed);
        }

        return null;
    }

    private void Backup()
    {
        if (File.Exists(_path))
        {
            File.Copy(_path, _path + ".bak", overwrite: true);
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
