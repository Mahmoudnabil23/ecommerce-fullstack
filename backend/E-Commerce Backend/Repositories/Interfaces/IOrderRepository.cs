using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Required by: GET /users/me/orders — customer's own orders with pagination
        Task<(List<Order> Orders, int Total)> GetByUserPagedAsync(
            Guid userId,
            OrderStatus? status,
            int page,
            int limit);

        // Required by: GET /admin/orders — all orders with filters
        Task<(List<Order> Orders, int Total)> GetAllPagedAsync(
            OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int limit);

        // Required by: GET /sellers/me/orders — orders containing seller products
        Task<(List<Order> Orders, int Total)> GetBySellerPagedAsync(
            Guid sellerId,
            int page,
            int limit);

        // Required by: GET /orders/{id} — with full navigation (items, history, address)
        Task<Order?> GetByIdWithDetailsAsync(Guid orderId);

        // Required by: GET /admin/dashboard → totalOrders, recentOrders
        Task<int> GetTotalCountAsync();
        Task<List<Order>> GetRecentAsync(int count);

        // Required by: GET /admin/dashboard → totalRevenue
        Task<decimal> GetTotalRevenueAsync();
    }
}
