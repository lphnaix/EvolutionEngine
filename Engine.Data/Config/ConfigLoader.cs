using System.Text.Json;

namespace Engine.Data.Config;

public static class ConfigLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static T Load<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Config not found: {path}");
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, Options)
               ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} from {path}");
    }
}
