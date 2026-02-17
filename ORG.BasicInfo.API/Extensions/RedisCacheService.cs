using StackExchange.Redis;
using System.Text.Json;

public interface IRedisCacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;
    private readonly string _prefix;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDatabase db, IConfiguration configuration)
    {
        _db = db;
        _prefix = configuration.GetSection("Redis").GetValue<string>("InstanceName") ?? string.Empty;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    private string Key(string key) => _prefix + key;

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await _db.StringSetAsync(Key(key), json, expiry).ConfigureAwait(false);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var val = await _db.StringGetAsync(Key(key)).ConfigureAwait(false);
        if (val.IsNullOrEmpty) return default;

        // تبدیل صریح به string برای جلوگیری از ابهام در انتخاب overload
        var json = (string)val;
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(Key(key)).ConfigureAwait(false);
    }
}
