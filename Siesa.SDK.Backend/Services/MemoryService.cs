using System;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;


namespace Siesa.SDK.Backend.Services
{
    public class MemoryService
    {
        private static string _redisUrl;
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
            return ConnectionMultiplexer.Connect(_redisUrl);
        });

        public static ConnectionMultiplexer Connection {
            get 
            {
                return lazyConnection.Value;
            }
        }
        public MemoryService(IConfiguration configuration)
        {
            _redisUrl = $"{configuration["ConnectionConfig:RedisUrl"]},abortConnect=false";
        }
        public string Get(string key)
        {
            try
            {
                IDatabase db = Connection.GetDatabase();
                var value = db.StringGet(key);
                return value;   
            }
            catch (System.Exception)
            {
                return "";
            }
        }
        public void Set(string key, string value)
        {
            try
            {
                IDatabase db = Connection.GetDatabase();
                db.StringSet(key, value);
            }
            catch (System.Exception)
            {
            }
        }
        
    }
}