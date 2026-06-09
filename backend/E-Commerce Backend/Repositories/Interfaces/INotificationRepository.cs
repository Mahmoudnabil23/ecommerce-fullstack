using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        // Required by: GET /notifications — paginated, optional unread filter
        Task<(List<Notification> Notifications, int Total)> GetByUserPagedAsync(
            Guid userId,
            bool? read,
            int page,
            int limit);

        // Required by: PUT /notifications/read-all
        Task MarkAllAsReadAsync(Guid userId);
    }
}
