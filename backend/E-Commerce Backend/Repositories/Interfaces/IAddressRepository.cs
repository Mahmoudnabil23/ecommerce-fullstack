using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        // Required by: GET /users/me → addresses, POST /orders → addressId validation
        Task<List<Address>> GetByUserIdAsync(Guid userId);

        // Required by: POST /users/me/addresses → clear old default when setting new one
        Task<Address?> GetDefaultByUserIdAsync(Guid userId);
    }
}
