namespace E_Commerce_Backend.DTOs.Notifications.Requests
{
    // GET /notifications — query params
    public class NotificationFilterRequestDto
    {
        public bool? Read { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }
}
