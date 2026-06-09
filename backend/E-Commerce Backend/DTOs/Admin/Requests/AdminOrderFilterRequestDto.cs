using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    // GET /admin/orders — query params
    public class AdminOrderFilterRequestDto
    {
        public OrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }
}
