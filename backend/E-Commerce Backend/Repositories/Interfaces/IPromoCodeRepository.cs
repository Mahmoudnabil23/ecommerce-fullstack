using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IPromoCodeRepository : IGenericRepository<PromoCode>
    {
        // Required by: POST /cart/promo — validate and retrieve by code string
        Task<PromoCode?> GetByCodeAsync(string code);

        // Required by: GET /admin/promo-codes — paginated list
        Task<(List<PromoCode> Codes, int Total)> GetPagedAsync(int page, int limit);
    }
}
