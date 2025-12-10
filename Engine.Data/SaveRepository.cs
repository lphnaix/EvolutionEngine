using Microsoft.Data.Sqlite;

namespace Engine.Data;

/// <summary>
/// Minimal SQLite wrapper for save/load prototypes.
/// </summary>
public sealed class SaveRepository : IDisposable
{
    private readonly SqliteConnection _connection;

    public SaveRepository(string path)
    {
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
                world_seed INTEGER NOT NULL,
                player_x REAL NOT NULL,
                player_y REAL NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public void SaveSnapshot(string name, int seed, float playerX, float playerY)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Saves(name, created_at, updated_at, world_seed, player_x, player_y)
            VALUES ($name, $created_at, $updated_at, $seed, $player_x, $player_y);
            """;
        cmd.Parameters.AddWithValue("$name", name);
        cmd.Parameters.AddWithValue("$created_at", now);
        cmd.Parameters.AddWithValue("$updated_at", now);
        cmd.Parameters.AddWithValue("$seed", seed);
        cmd.Parameters.AddWithValue("$player_x", playerX);
        cmd.Parameters.AddWithValue("$player_y", playerY);
        cmd.ExecuteNonQuery();
    }

    public (float x, float y, int seed)? LoadLatest()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT player_x, player_y, world_seed
            FROM Saves
            ORDER BY updated_at DESC
            LIMIT 1;
            """;

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return (reader.GetFloat(0), reader.GetFloat(1), reader.GetInt32(2));
        }

        return null;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
