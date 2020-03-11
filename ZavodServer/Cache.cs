using Microsoft.Extensions.Caching.Memory;

namespace ZavodServer
{
    public static class Cache
    {
        public static MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        
        
    }
}