using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Implementations;
using E_Commerce_Backend.Repositories.Interfaces;

namespace E_Commerce_Backend.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository Users {  get; private set; }

        public IAddressRepository Addresses { get; private set; }

        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }

        public IProductImageRepository ProductImages { get; private set; }

        public ISellerProfileRepository SellerProfiles { get; private set; }

        public ICartRepository Carts { get; private set; }

        public ICartItemRepository CartItems { get; private set; }

        public IOrderRepository Orders { get; private set; }
        public IReviewRepository Reviews { get; private set; }

        public IWishlistRepository WishlistItems { get; private set; }

        public IPromoCodeRepository PromoCodes { get; private set; }
        public IBannerRepository Banners { get; private set; }

        public INotificationRepository Notifications { get; private set; }

        AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Reviews = new ReviewRepository(_context);
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
