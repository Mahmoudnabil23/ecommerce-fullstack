namespace E_Commerce_Backend.Enums
{
    // Source: API Contract § 1.6 PUT /orders/{id}/status
    // Flow: pending → confirmed → processing → shipped → delivered → completed
    // Cancellation: pending/confirmed → cancelled
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        OnDelivery,
        Delivered,
        Completed,
        Cancelled
    }

}


