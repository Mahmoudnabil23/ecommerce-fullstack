using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Services.AuthServices.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateAccessToken(ApplicationUser user);
        public string GenerateRefreshToken();
    }
}
