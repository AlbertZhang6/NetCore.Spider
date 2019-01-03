using Microsoft.Extensions.Caching.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Redis
{
    internal class MasterSlaveFailover : IRedisFarmFailover
    {
        private readonly RedisFarmOptions options;
        private RedisCache master;

        public MasterSlaveFailover(RedisFarmOptions options)
        {
            this.options = options;
        }

        public RedisCache SelectMaster()
        {
            if (master == null)
            {
                lock (this)
                {
                    if (master == null)
                    {
                        master = RedisFactory.CreateMaster(options.Servers, options.Database, options.Password);
                    }
                }
            }
            return master;
        }

        public bool TrySwitchMaster(RedisCache fault)
        {
            return false;
        }

        public void Dispose()
        {
            if (master != null)
                master.Dispose();
        }
    }
}
