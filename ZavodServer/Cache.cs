using Microsoft.Extensions.Caching.Memory;

namespace ZavodServer
{
    public static class Cache
    {
        public static MemoryCache LocalCache = new MemoryCache(new MemoryCacheOptions());
    }
}