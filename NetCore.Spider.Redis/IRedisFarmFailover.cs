using Microsoft.Extensions.Caching.Redis;
using System;

namespace NetCore.Spider.Redis
{
    internal interface IRedisFarmFailover : IDisposable
    {
        RedisCache SelectMaster();

        bool TrySwitchMaster(RedisCache fault);
    }

}
