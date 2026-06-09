using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(Order entity)
        {
            await _context.Orders.AddAsync(entity);
        }

        public void Delete(Order entity)
        {
            _context.Orders.Remove(entity);
        }

        public IQueryable<Order> Find(Expression<Func<Order, bool>> predicate)
        {
            return _context.Orders.Where(predicate);
        }

        public IQueryable<Order> GetAll()
        {
            return _context.Orders.AsQueryable();
        }

        public async Task<(List<Order> Orders, int Total)> GetAllPagedAsync(OrderStatus? status, DateTime? startDate, DateTime? endDate, int page, int limit)
        {
            IQueryable<Order> query = _context.Orders.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= endDate.Value);
            }

            int totalCount = await query.CountAsync();

            List<Order> orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<(List<Order> Orders, int Total)> GetBySellerPagedAsync(Guid sellerId, int page, int limit)
        {
            var sellerIdString = sellerId.ToString();
            var query = _context.Orders
                .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId)); // Order items belong to seller

            int totalCount = await query.CountAsync();

            List<Order> orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<(List<Order> Orders, int Total)> GetByUserPagedAsync(Guid userId, OrderStatus? status, int page, int limit)
        {
            string userIdString = userId.ToString();
            IQueryable<Order> query = _context.Orders.Where(o => o.UserId == userIdString);

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            int totalCount = await query.CountAsync();

            List<Order> orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<List<Order>> GetRecentAsync(int count)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders.SumAsync(o => o.Total);
        }

        public void Update(Order entity)
        {
            _context.Orders.Update(entity);
        }
    }
}