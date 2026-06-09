using E_Commerce_Backend.DTOs.Orders.Requests;
using E_Commerce_Backend.DTOs.Orders.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Comman;

namespace E_Commerce_Backend.Services.Orders.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderCreatedResponseDto>> PlaceOrderAsync(CreateOrderRequestDto request, string userId);
        Task<(Result<List<OrderListItemDto>> Result, int TotalCount)> GetUserOrderHistoryAsync(string userId, int page, int limit);
        Task<Result<OrderDetailResponseDto>> GetOrderDetailsAsync(Guid orderId, string userId, bool isAdmin);
        Task<Result<bool>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequestDto request);
        Task<Result<bool>> CancelOrderAsync(Guid orderId, string userId, CancelOrderRequestDto request);
    }
}
