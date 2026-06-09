using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Orders.Requests;
using E_Commerce_Backend.DTOs.Orders.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<List<OrderListItemDto>>>> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<List<OrderListItemDto>>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListItemDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    Total = o.Total,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<OrderListItemDto>>
            {
                Success = true,
                Data = orders
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<OrderDetailResponseDto>>> GetOrderById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<OrderDetailResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            var isAdmin = User.IsInRole("Admin");

            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new ApiResponse<OrderDetailResponseDto>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            if (!isAdmin && order.UserId != userId)
            {
                return StatusCode(403, new ApiResponse<OrderDetailResponseDto>
                {
                    Success = false,
                    Message = "You cannot access this order"
                });
            }

            var detail = new OrderDetailResponseDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                TrackingNumber = order.TrackingNumber,
                Total = order.Total,
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
                StatusHistory = order.StatusHistory
                    .OrderBy(h => h.Timestamp)
                    .Select(h => new OrderStatusHistoryDto
                    {
                        Status = h.Status,
                        Timestamp = h.Timestamp
                    })
                    .ToList()
            };

            return Ok(new ApiResponse<OrderDetailResponseDto>
            {
                Success = true,
                Data = detail
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderCreatedResponseDto>>> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<OrderCreatedResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return BadRequest(new ApiResponse<OrderCreatedResponseDto>
                {
                    Success = false,
                    Message = "Cart is empty"
                });
            }

            Address? address = null;
            if (request.AddressId.HasValue)
            {
                address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.AddressId.Value && a.UserId == userId);
            }

            if (address == null)
            {
                address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
            }

            if (address == null)
            {
                address = new Address
                {
                    Street = "Not specified",
                    City = "Not specified",
                    PostalCode = string.Empty
                };
            }

            var total = cart.Items.Sum(i => i.Quantity * i.Product.Price);
            var now = DateTime.UtcNow;

            var order = new Order
            {
                UserId = userId,
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
                Status = OrderStatus.OnDelivery,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                Total = total,
                Notes = request.Notes,
                CreatedAt = now,
                EstimatedDelivery = now.AddDays(5),
                ShippingStreet = address.Street,
                ShippingCity = address.City,
                ShippingPostalCode = address.PostalCode,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price
                }).ToList(),
                StatusHistory = new List<OrderStatusHistory>
                {
                    new OrderStatusHistory
                    {
                        Status = OrderStatus.Pending,
                        Timestamp = now
                    }
                }
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = now;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            var response = new OrderCreatedResponseDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                Total = order.Total,
                PaymentStatus = order.PaymentStatus.ToString(),
                EstimatedDelivery = order.EstimatedDelivery
            };

            return StatusCode(201, new ApiResponse<OrderCreatedResponseDto>
            {
                Success = true,
                Message = "Order created successfully",
                Data = response
            });
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(Guid id, [FromBody] CancelOrderRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            var isAdmin = User.IsInRole("Admin");

            var order = await _context.Orders
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            if (!isAdmin && order.UserId != userId)
            {
                return StatusCode(403, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "You cannot cancel this order"
                });
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Only pending or confirmed orders can be cancelled"
                });
            }

            order.Status = OrderStatus.Cancelled;
            order.Notes = string.IsNullOrWhiteSpace(request?.Reason)
                ? order.Notes
                : $"{order.Notes} | Cancel reason: {request.Reason}";

            order.StatusHistory.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Cancelled,
                Timestamp = DateTime.UtcNow
            });

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Order cancelled",
                Data = true
            });
        }
    }
}