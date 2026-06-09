using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        // Required by: GET /products/{productId}/reviews — paginated
        Task<(List<Review> Reviews, int Total)> GetByProductPagedAsync(
            Guid productId,
            int? rating,
            int page,
            int limit);

        // Required by: POST /products/{productId}/reviews → 409 duplicate check
        Task<bool> ExistsForUserAndProductAsync(Guid userId, Guid productId);

        // Required by: POST /products/{productId}/reviews → 403 verified purchase check
        Task<bool> UserHasPurchasedProductAsync(Guid userId, Guid productId);

        // Required by: GET /products → averageRating, reviewCount
        Task<(double Average, int Count)> GetRatingSummaryAsync(Guid productId);
    }
}
