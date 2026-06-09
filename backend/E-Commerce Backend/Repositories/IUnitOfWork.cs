using E_Commerce_Backend.Repositories.Interfaces;

namespace E_Commerce_Backend.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IAddressRepository Addresses { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IProductImageRepository ProductImages { get; }
        ISellerProfileRepository SellerProfiles { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IOrderRepository Orders { get; }
        IReviewRepository Reviews { get; }
        IWishlistRepository WishlistItems { get; }
        IPromoCodeRepository PromoCodes { get; }
        IBannerRepository Banners { get; }
        INotificationRepository Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
