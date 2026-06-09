using E_Commerce_Backend.DTOs.Orders.Responses;
using E_Commerce_Backend.DTOs.Products.Responses;

namespace E_Commerce_Backend.DTOs.Admin.Responses
{

    // GET /admin/dashboard
    public class AdminDashboardResponseDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public List<OrderListItemDto> RecentOrders { get; set; } = new();
        public List<ProductListItemDto> TopProducts { get; set; } = new();
        // salesByDay: NOT DEFINED IN SRS/API CONTRACT (structure/grouping not specified)
        public List<SalesByDayDto> SalesByDay { get; set; } = new();
    }
}
