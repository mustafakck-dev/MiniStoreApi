using StackExchange.Redis; //.NET uygulaması ile Redis arasında konuşmayı sağlayan NuGet kütüphanesi.

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly StackExchange.Redis.IDatabase _database;//Redis veritabanı ile konuşmayı sağlayan nesne.

    public RedisCacheService(IConnectionMultiplexer redis)//Redis bağlantısını sağlayan nesne.
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task<string?> GetAsync(string key)//Bu Redis'ten veri okuyor.
    {
        var value = await _database.StringGetAsync(key);//Redis'ten veri okuma işlemi yapılıyor.

        return value.HasValue
            ? value.ToString()
            : null;
    }

    public async Task SetAsync(string key,string value,TimeSpan expiration)//Bu Redis'e veri yazıyor.
    {
        await _database.StringSetAsync(key,value,expiration);//Redis'e veri yazma işlemi yapılıyor.
    }

    public async Task RemoveAsync(string key)//Bu Redis'ten veri siliyor.
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix)//Bu Redis'ten belirli bir prefix ile başlayan tüm verileri siliyor.
    {
        foreach (var endpoint in _redis.GetEndPoints()) //bu foreach döngüsü ile Redis sunucusundaki tüm endpoint'ler üzerinde dönülüyor.
        {
            var server = _redis.GetServer(endpoint);//Redis sunucusuna bağlanmak için kullanılan nesne.

            var keys = server.Keys(pattern: $"{prefix}*").ToArray();//Redis sunucusundaki belirli bir prefix ile başlayan tüm anahtarları alıyor.

            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);//Redis sunucusundaki belirli bir prefix ile başlayan tüm anahtarları siliyor.
            }
        }
    }
}