using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review entity)
        {
            await _context.Reviews.AddAsync(entity);
        }

        public void Delete(Review entity)
        {
            _context.Reviews.Remove(entity);
        }

        public async Task<bool> ExistsForUserAndProductAsync(Guid userId, Guid productId)
        {
            string userIdString = userId.ToString();
            return await _context.Reviews.AnyAsync(r => r.UserId == userIdString && r.ProductId == productId);
        }

        public IQueryable<Review> Find(Expression<Func<Review, bool>> predicate)
        {
            return _context.Reviews.Where(predicate);
        }

        public IQueryable<Review> GetAll()
        {
            return _context.Reviews.AsQueryable();
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(List<Review> Reviews, int Total)> GetByProductPagedAsync(Guid productId, int? rating, int page, int limit)
        {
            IQueryable<Review> query = _context.Reviews.Where(r => r.ProductId == productId);

            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            int totalCount = await query.CountAsync();

            List<Review> reviews = await query
                .Include(r => r.User) 
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (reviews, totalCount);
        }

        public async Task<(double Average, int Count)> GetRatingSummaryAsync(Guid productId)
        {
            IQueryable<Review> query = _context.Reviews.Where(r => r.ProductId == productId);

            int count = await query.CountAsync();
            double average = 0;

            if (count > 0)
            {

                double avgResult = await query.AverageAsync(r => (double)r.Rating);
                average = avgResult;
            }

            return (average, count);
        }

        public void Update(Review entity)
        {
            _context.Reviews.Update(entity);
        }

        public async Task<bool> UserHasPurchasedProductAsync(Guid userId, Guid productId)
        {
            throw new NotImplementedException();
        }
    }
}