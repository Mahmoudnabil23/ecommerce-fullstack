using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Responses;
using E_Commerce_Backend.DTOs.Orders.Responses;
using E_Commerce_Backend.DTOs.Products.Responses;
using E_Commerce_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<AdminDashboardResponseDto>>> GetAdminDashboard()
        {
            var orders = await _unitOfWork.Orders.GetRecentAsync(10);
            var orderItemList = orders.Select(order => new OrderListItemDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Total = order.Total
            }).ToList();

            var products = await _unitOfWork.Products.GetTopProductsAsync(10);
            var productListItem = new List<ProductListItemDto>();

            foreach (var product in products)
            {
                var review = await _unitOfWork.Reviews.GetRatingSummaryAsync(product.Id);

                productListItem.Add(new ProductListItemDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Images = product.Images.Select(i => i.ImageUrl).ToList(),
                    Category = new CategoryRefDto
                    {
                        Id = product.CategoryId.Value,
                        Name = product.Category?.Name 
                    },
                    Seller = new SellerRefDto
                    {
                        Id = product.SellerId.Value,
                        StoreName = product.Seller?.StoreName 
                    },
                    Slug = product.Slug,
                    Stock = product.Stock,
                    ReviewCount = review.Count,
                    AverageRating = review.Average,
                    DiscountedPrice = product.DiscountedPrice
                });
            }

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var dashboard = new AdminDashboardResponseDto
            {
                TopProducts = productListItem,
                RecentOrders = orderItemList,

                TotalProducts = await _unitOfWork.Products.GetAll().CountAsync(),
                TotalOrders = await _unitOfWork.Orders.GetTotalCountAsync(),
                TotalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync(),
                TotalUsers = await _unitOfWork.Users.GetAll().CountAsync(),

                SalesByDay = await _unitOfWork.Orders.GetAll()
                    .Where(o => o.CreatedAt >= thirtyDaysAgo)
                    .GroupBy(o => o.CreatedAt.Date)
                    .Select(g => new SalesByDayDto
                    {
                        Date = g.Key,
                        Amount = g.Sum(o => o.Total)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync()
            };

            return Ok(new ApiResponse<AdminDashboardResponseDto>
            {
                Success = true,
                Data = dashboard
            });
        }
    }
}