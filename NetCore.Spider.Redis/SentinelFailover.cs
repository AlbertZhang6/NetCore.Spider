using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading;

namespace NetCore.Redis
{
    internal class SentinelFailover : IRedisFarmFailover, IDisposable
    {
        private readonly TimeSpan FailoverTimeout = TimeSpan.FromSeconds(30.0);

        private readonly RedisFarmOptions options;

        private ILogger<SentinelFailover> logger;

        private Lazy<ConnectionMultiplexer> sentinel;

        private ISubscriber subscriber;

        private volatile StatefulRedisCache master;

        private volatile bool tryingFailover;

        private volatile int timestamp;

        public SentinelFailover(RedisFarmOptions options, ILogger<SentinelFailover> logger)
        {
            this.options = options;
            this.logger = logger;
            sentinel = new Lazy<ConnectionMultiplexer>(() =>
            {
                var connection = RedisFactory.CreateSentinel(this.options.Servers, this.options.Database, this.options.ServiceName);
                subscriber = connection.GetSubscriber();
                subscriber.Subscribe("+try-failover", OnEvent);
                subscriber.Subscribe("+switch-master", OnEvent);

                return connection;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public RedisCache SelectMaster()
        {
            if (!SpinWait.SpinUntil(() => !tryingFailover, FailoverTimeout))
            {
                throw new TimeoutException();
            }
            //copy the present master's reference so that the current won't be null even though master has set to null at the same time
            RedisCache current = master;
            if (current == null)
            {
                lock (this)
                {
                    if (master == null)
                    {
                        var newMaster = CreateMaster();
                        if (newMaster == null)
                        {
                            string critical = "all of sentinels are gone down.";
                            logger.LogCritical(critical);
                            throw new InvalidOperationException(critical);
                        }
                        newMaster.Timestamp = ++timestamp;
                        master = newMaster;
                    }
                    current = master;
                }
            }
            //if current master is disposed, it doesn't matter because it won't be null and will try to switch master subsequently
            return current;
        }

        public bool TrySwitchMaster(RedisCache fault)
        {
            StatefulRedisCache current = fault as StatefulRedisCache;
            if (SpinWait.SpinUntil(() => tryingFailover || master == null || current.Timestamp != timestamp, FailoverTimeout))
            {
                return true;
            }
            return false;
        }

        private void OnEvent(RedisChannel channel, RedisValue value)
        {
            if (channel == "+try-failover")
            {
                tryingFailover = true;
            }
            else if (channel == "+switch-master")
            {
                lock (this)
                {
                    try
                    {
                        if (master != null)
                        {
                            master.Dispose();
                        }
                    }
                    finally
                    {
                        master = null;
                        tryingFailover = false;
                    }
                }
            }
            logger.LogCritical($"{channel}: {value}");
        }

        private StatefulRedisCache CreateMaster()
        {
            var endpoints = sentinel.Value.GetEndPoints();
            foreach (var ep in endpoints)
            {
                IServer server = sentinel.Value.GetServer(ep);
                if (server.ServerType == ServerType.Sentinel && server.IsConnected)
                {
                    var hostAndPort = server.SentinelGetMasterAddressByName(options.ServiceName);
                    return RedisFactory.CreateMaster(hostAndPort as IPEndPoint, options.Database, options.Password);
                }
            }
            //all of sentinels are gone down
            return null;
        }

        public void Dispose()
        {
            if (sentinel.IsValueCreated)
            {
                sentinel.Value.Dispose();
            }
            if (master != null)
            {
                master.Dispose();
                master = null;
            }
        }
    }

}
