using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NetCore.Spider.Redis
{
    internal static class RedisFactory
    {
        private const string TheVersion = "3.2.1";
        private const int SyncTimeout = 5000;

        internal static StatefulRedisCache CreateMaster(IPEndPoint hostAndPort, ushort database, string password)
        {
            string connectionString = BuildMasterString(new string[] { hostAndPort.ToString() }, database, password);
            return new StatefulRedisCache(Options.Create<RedisCacheOptions>(new RedisCacheOptions()
            {
                Configuration = connectionString
            }));
        }

        internal static StatefulRedisCache CreateMaster(List<string> hostAndPorts, ushort database, string password)
        {
            string connectionString = BuildMasterString(hostAndPorts, database, password);
            return new StatefulRedisCache(Options.Create<RedisCacheOptions>(new RedisCacheOptions()
            {
                Configuration = connectionString
            }));
        }

        internal static ConnectionMultiplexer CreateSentinel(List<string> hostAndPorts, ushort database, string serviceName)
        {
            ConfigurationOptions configuration = BuildSentinelConfig(hostAndPorts, database, serviceName);
            return ConnectionMultiplexer.Connect(configuration);
        }

        private static string BuildMasterString(IList<string> hostAndPorts, ushort database, string password)
        {
            List<string> configurations = new List<string>();
            configurations.AddRange(hostAndPorts);
            configurations.Add($"defaultDatabase={database}");
            if (!string.IsNullOrEmpty(password))
                configurations.Add($"password={password}");
            configurations.Add($"version={TheVersion}");
            configurations.Add($"syncTimeout={SyncTimeout}");

            return string.Join(",", configurations);
        }

        private static ConfigurationOptions BuildSentinelConfig(IList<string> hostAndPorts, ushort database, string serviceName)
        {
            ConfigurationOptions configuration = ConfigurationOptions.Parse(string.Join(",", hostAndPorts));
            configuration.DefaultDatabase = database;
            configuration.ServiceName = serviceName;
            configuration.TieBreaker = string.Empty;
            configuration.CommandMap = CommandMap.Sentinel;
            configuration.DefaultVersion = new Version(TheVersion);
            configuration.SyncTimeout = SyncTimeout;

            return configuration;
        }
    }
}
