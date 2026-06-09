using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationUser entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public void Delete(ApplicationUser entity)
        {
            _context.Users.Remove(entity);
        }

        public IQueryable<ApplicationUser> Find(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _context.Users.Where(predicate);
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return _context.Users.AsQueryable();
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetByEmailVerificationTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
        {
            
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
        }

        public async Task<ApplicationUser?> GetByPasswordResetTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> GetByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken)
        {

            throw new NotImplementedException();
        }

        public async Task<(List<ApplicationUser> Users, int Total)> GetPagedAsync(UserRole? role, UserStatus? status, string? search, int page, int limit)
        {
            IQueryable<ApplicationUser> query = _context.Users.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }


            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
            }

            int totalCount = await query.CountAsync();

            List<ApplicationUser> users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (users, totalCount);
        }

        public void Update(ApplicationUser entity)
        {
            _context.Users.Update(entity);
        }
    }
}