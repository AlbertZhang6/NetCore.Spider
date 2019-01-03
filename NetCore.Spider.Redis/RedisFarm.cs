using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Redis
{
    internal class RedisFarm : IDistributedCache, IDisposable
    {
        private const int MaxRetryCount = 2;

        private readonly RedisFarmOptions options;

        private ILogger<RedisFarm> logger;

        private IRedisFarmFailover farmFailover;

        public RedisFarm(IOptions<RedisFarmOptions> options, ILoggerFactory loggerFactory)
        {
            this.options = options.Value;
            this.logger = loggerFactory.CreateLogger<RedisFarm>();

            switch (this.options.Mode)
            {
                case RedisFarmMode.MasterSlave:
                    this.farmFailover = new MasterSlaveFailover(this.options);
                    break;
                case RedisFarmMode.Sentinel:
                    this.farmFailover = new SentinelFailover(this.options, loggerFactory.CreateLogger<SentinelFailover>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.options.Mode));
            }
        }

        public byte[] Get(string key)
        {
            return ExecuteResult(async redis => redis.Get(key)).Result;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return await ExecuteResult((RedisCache redis) => redis.GetAsync(key, token));
        }

        public async void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            await ExecuteVoid(async redis => redis.Set(key, value, options));
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await ExecuteVoid(redis => redis.SetAsync(key, value, options, token));
        }

        public async void Refresh(string key)
        {
            await ExecuteVoid(async redis => redis.Refresh(key));
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await ExecuteVoid(redis => redis.RefreshAsync(key, token));
        }

        public async void Remove(string key)
        {
            await ExecuteVoid(async redis => redis.Remove(key));
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await ExecuteVoid(redis => redis.RemoveAsync(key, token));
        }

        private async Task ExecuteVoid(Func<RedisCache, Task> execution)
        {
            int retry = 0;
            for (; ; )
            {
                RedisCache redis = null;
                try
                {
                    redis = farmFailover.SelectMaster();
                    await execution(redis);
                    return;
                }
                catch (Exception ex)
                {
                    if (!CanFailover(redis, ex, ref retry))
                        throw;
                }
            }
        }

        private async Task<T> ExecuteResult<T>(Func<RedisCache, Task<T>> execution)
        {
            int retry = 0;
            for (; ; )
            {
                RedisCache redis = null;
                try
                {
                    redis = farmFailover.SelectMaster();
                    T result = await execution(redis);
                    return result;
                }
                catch (Exception ex)
                {
                    if (!CanFailover(redis, ex, ref retry))
                        throw;
                }
            }
        }

        private bool CanFailover(RedisCache redis, Exception ex, ref int retry)
        {
            if (redis == null)
                //can not get a master
                return false;
            if (ex is RedisConnectionException || ex is RedisTimeoutException || ex is TimeoutException)
            {
                if (retry >= MaxRetryCount)
                {
                    logger.LogCritical($"failover has tried {retry} times and been interrupted because it has reached the maximum retry limit.");
                    return false;
                }

                retry++;
                logger.LogCritical($"failover is going to try {retry} times.");
                bool canSwitch = farmFailover.TrySwitchMaster(redis);
                if (canSwitch)
                {
                    logger.LogInformation("failover is running and allowed to switch master");
                }
                else
                {
                    logger.LogCritical($"failover responds failed. it can't run right now.");
                }
                return canSwitch;
            }
            return false;
        }

        public void Dispose()
        {
            farmFailover.Dispose();
        }
    }
}
