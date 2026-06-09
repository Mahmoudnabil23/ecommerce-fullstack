using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Orders.Requests;
using E_Commerce_Backend.DTOs.Orders.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories;
using E_Commerce_Backend.Repositories.Interfaces;
using E_Commerce_Backend.Services.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public OrderService(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Result<OrderCreatedResponseDto>> PlaceOrderAsync(CreateOrderRequestDto request, string userId)
        {
            // 1. Get User's Cart
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(Guid.Parse(userId));
            if (cart == null || !cart.Items.Any())
            {
                return Result<OrderCreatedResponseDto>.Fail("Cart is empty.");
            }

            // 2. Validate Stock and Create Order Items
            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var cartItem in cart.Items)
            {
                if (cartItem.Product.Stock < cartItem.Quantity)
                {
                    return Result<OrderCreatedResponseDto>.Fail($"Insufficient stock for product: {cartItem.Product.Name}");
                }

                // Create snapshot
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };

                orderItems.Add(orderItem);
                total += orderItem.Quantity * orderItem.UnitPrice;

                // Decrement stock
                cartItem.Product.Stock -= cartItem.Quantity;
            }

            // 3. Address Handling
            string street = string.Empty;
            string city = string.Empty;
            string? postalCode = null;

            if (request.AddressId.HasValue)
            {
                var address = await _unitOfWork.Addresses.GetByIdAsync(request.AddressId.Value);
                if (address == null) return Result<OrderCreatedResponseDto>.Fail("Address not found.");
                street = address.Street;
                city = address.City;
                postalCode = address.PostalCode;
            }
            else if (!string.IsNullOrEmpty(request.Street) && !string.IsNullOrEmpty(request.City))
            {
                street = request.Street;
                city = request.City;
                postalCode = request.PostalCode;
            }
            else
            {
                return Result<OrderCreatedResponseDto>.Fail("Shipping address is required.");
            }

            // 4. Create Order Object
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                Status = OrderStatus.Pending,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                Total = total,
                ShippingStreet = street,
                ShippingCity = city,
                ShippingPostalCode = postalCode,
                Notes = request.Notes,
                Items = orderItems
            };

            // 5. Initial Status History
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = OrderStatus.Pending,
                Timestamp = DateTime.UtcNow,
                
            });

            // 6. Persist Changes
            await _unitOfWork.Orders.AddAsync(order);

            // Clear Cart
            _unitOfWork.CartItems.DeleteRange(cart.Items);

            await _unitOfWork.SaveChangesAsync();

            return Result<OrderCreatedResponseDto>.Ok(new OrderCreatedResponseDto 
            { 
                OrderId = order.Id, 
                OrderNumber = order.OrderNumber 
            });
        }

        public async Task<(Result<List<OrderListItemDto>> Result, int TotalCount)> GetUserOrderHistoryAsync(string userId, int page, int limit)
        {
            var (orders, total) = await _unitOfWork.Orders.GetByUserPagedAsync(Guid.Parse(userId), null, page, limit);

            var dtos = orders.Select(o => new OrderListItemDto
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                Total = o.Total,
                CreatedAt = o.CreatedAt
            }).ToList();

            return (Result<List<OrderListItemDto>>.Ok(dtos), total);
        }

        public async Task<Result<OrderDetailResponseDto>> GetOrderDetailsAsync(Guid orderId, string userId, bool isAdmin)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
            if (order == null) return Result<OrderDetailResponseDto>.Fail("Order not found.");

            if (!isAdmin && order.UserId != userId)
            {
                return Result<OrderDetailResponseDto>.Fail("Unauthorized access to order details.");
            }

            var dto = new OrderDetailResponseDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                Total = order.Total,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                TrackingNumber = order.TrackingNumber,
                Address = new OrderAddressDto
                {
                    Street = order.ShippingStreet,
                    City = order.ShippingCity,
                    PostalCode = order.ShippingPostalCode
                },
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Name = i.ProductName,
                    Qty = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                StatusHistory = order.StatusHistory.Select(sh => new OrderStatusHistoryDto
                {
                    Status = sh.Status,
                    Timestamp = sh.Timestamp,
                    
                     
                }).ToList()
            };

            return Result<OrderDetailResponseDto>.Ok(dto);
        }

        public async Task<Result<bool>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequestDto request)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return Result<bool>.Fail("Order not found.");

            order.Status = request.Status;
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = request.Status,
                Timestamp = DateTime.UtcNow,

            });

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> CancelOrderAsync(Guid orderId, string userId, CancelOrderRequestDto request)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return Result<bool>.Fail("Order not found.");

            if (order.UserId != userId) return Result<bool>.Fail("Unauthorized.");

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            {
                return Result<bool>.Fail("Order cannot be cancelled at this stage.");
            }

            order.Status = OrderStatus.Cancelled;
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = OrderStatus.Cancelled,
                Timestamp = DateTime.UtcNow,
            });

            // Restore Stock
            foreach (var item in order.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null) product.Stock += item.Quantity;
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}";
        }
    }
}
