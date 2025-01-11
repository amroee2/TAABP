using TAABP.Application.DTOs;

namespace TAABP.Application
{
    public interface IStorageService
    {
        Task StoreUserAsync(string token, RegisterDto registerDto);
        Task<RegisterDto?> RetrieveUserAsync(string token);
        Task DeleteTokenAsync(string token);
    }
}
