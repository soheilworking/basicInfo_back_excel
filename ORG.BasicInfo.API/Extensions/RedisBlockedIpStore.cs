using StackExchange.Redis;

public interface IRedisBlockedIpStore
{
    Task<bool> IsBlockedAsync(string ip, CancellationToken ct = default);
    Task BlockIpAsync(string ip, TimeSpan? ttl = null, CancellationToken ct = default);
    Task UnblockIpAsync(string ip, CancellationToken ct = default);
    Task<IEnumerable<string>> GetAllBlockedIpsAsync(CancellationToken ct = default);
}

public class RedisBlockedIpStore : IRedisBlockedIpStore
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IDatabase _db;
    private const string SetKey = "blocked:ips";
    private const string TempKeyPrefix = "blocked:ip:"; // used for TTL-based blocks

    public RedisBlockedIpStore(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
        _db = _multiplexer.GetDatabase();
    }

    public async Task<bool> IsBlockedAsync(string ip, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ip)) return false;

        // 1) ابتدا بررسی در set (دنبال بلاک دائمی)
        if (await _db.SetContainsAsync(SetKey, ip).ConfigureAwait(false))
            return true;

        // 2) بررسی وجود کلید موقتی مربوط به IP (برای TTL)
        return await _db.KeyExistsAsync(TempKeyPrefix + ip).ConfigureAwait(false);
    }

    public async Task BlockIpAsync(string ip, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ip)) return;

        if (ttl == null)
        {
            // بلاک دائمی: وارد set می‌کنیم
            await _db.SetAddAsync(SetKey, ip).ConfigureAwait(false);
        }
        else
        {
            // بلاک موقت: یک key با TTL می‌سازیم + در صورت نیاز set را هم مدیریت می‌کنیم
            var tempKey = TempKeyPrefix + ip;
            // مقدار دلخواه (مثلا timestamp) ذخیره می‌کنیم
            await _db.StringSetAsync(tempKey, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), ttl).ConfigureAwait(false);
        }
    }

    public async Task UnblockIpAsync(string ip, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ip)) return;

        var tempKey = TempKeyPrefix + ip;
        await _db.KeyDeleteAsync(tempKey).ConfigureAwait(false);
        await _db.SetRemoveAsync(SetKey, ip).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetAllBlockedIpsAsync(CancellationToken ct = default)
    {
        var members = await _db.SetMembersAsync(SetKey).ConfigureAwait(false);
        return members.Select(m => (string)m);
    }
}
