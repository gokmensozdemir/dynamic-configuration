using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfiguration.Services
{
    public delegate void KeyInvalidateHandler(string applicationName, string name);

    public class CacheService : ICacheService, IDisposable
    {
        private IDatabase database;
        private ISubscriber subscriber;
        private RedisChannel invalidateChannel = new("invalidate_channel", RedisChannel.PatternMode.Literal);
        public event KeyInvalidateHandler KeyInvalidated;

        public CacheService(string redisConnectionString)
        {
            var redisClient = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(redisConnectionString));

            subscriber = redisClient.GetSubscriber();
            database = redisClient.GetDatabase();

            subscriber.Subscribe(invalidateChannel, (channel, message) =>
            {
                if (channel == invalidateChannel)
                {
                    var partials = message.ToString().Split(":");

                    KeyInvalidated.Invoke(partials[0], partials[1]);
                }
            });
        }

        public async Task<string> GetOrUpdateAsync(string key, Func<Task<string>> valueFactory, TimeSpan? expiry = null)
        {
            // Attempt to get the value from Redis
            string value = await database.StringGetAsync(key);

            if (value == null)
            {
                // If the key doesn't exist in Redis, use the valueFactory to create the value
                value = await valueFactory();

                // Add the value to Redis with an expiration time (e.g., 1 hour)
                await database.StringSetAsync(key, value, expiry);
            }

            return value;
        }

        public Task KeyDeleteAsync(string key)
        {
            return database.KeyDeleteAsync(key);
        }

        public void Dispose()
        {
            subscriber?.Unsubscribe(invalidateChannel);
        }
    }
}
