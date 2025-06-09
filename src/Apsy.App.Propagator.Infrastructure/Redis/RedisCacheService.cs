using StackExchange.Redis;
using System.Text.Json;

namespace Apsy.App.Propagator.Infrastructure.Redis;

public interface IRedisCacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan expiration);
    Task SetAsync<T>(string key, T value);
    Task<T> GetAsync<T>(string key);
    Task RemoveAsync(string key);
    Task PublishUpdateAsync(string channel, string message);
    Task SubscribeToUpdatesAsync(string channel, Action<string> handler);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly StackExchange.Redis.IDatabase _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _cache = redis.GetDatabase();

        //InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        //await SubscribeToUpdatesAsync("cache_invalidation", async key =>
        //{
        //    await RemoveAsync(key);
        //});
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var jsonData = System.Text.Json.JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            MaxDepth = 64
        });
        await _cache.StringSetAsync(key, jsonData, expiration);
    }
    public async Task SetAsync<T>(string key, T value)
    {
        var jsonData = System.Text.Json.JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            MaxDepth = 64
        });
        await _cache.StringSetAsync(key, jsonData);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var jsonData = await _cache.StringGetAsync(key);
        return jsonData.HasValue ? System.Text.Json.JsonSerializer.Deserialize<T>(jsonData, new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            MaxDepth = 64
        }) : default;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }

    public async Task PublishUpdateAsync(string channel, string message)
    {
        var subscriber = _redis.GetSubscriber();
        await subscriber.PublishAsync(RedisChannel.Literal(channel), message);
    }

    public async Task SubscribeToUpdatesAsync(string channel, Action<string> handler)
    {
        var subscriber = _redis.GetSubscriber();
        await subscriber.SubscribeAsync(RedisChannel.Literal(channel), (_, message) => handler(message));
    }
}