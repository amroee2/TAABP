using Microsoft.Extensions.Caching.Memory;
using TAABP.Application;
using TAABP.Application.DTOs;

namespace TAABP.Infrastructure
{
    public class StorageService : IStorageService
    {
        private readonly IMemoryCache _cache;

        public StorageService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task StoreUserAsync(string token, RegisterDto registerDto)
        {
            _cache.Set(token, registerDto, TimeSpan.FromHours(1));
            await Task.CompletedTask;
        }

        public async Task<RegisterDto?> RetrieveUserAsync(string token)
        {
            _cache.TryGetValue(token, out RegisterDto? registerDto);
            return await Task.FromResult(registerDto);
        }

        public async Task DeleteTokenAsync(string token)
        {
            _cache.Remove(token);
            await Task.CompletedTask;
        }
    }
}
