using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByTokenWithUserAsync(string token);

        Task AddAsync(RefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}
