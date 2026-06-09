namespace E_Commerce_Backend.Comman
{
    // Used by all list endpoints in the API Contract that return paginated data.
    // Source: API Contract → pagination object: { total, page, limit, totalPages }

    namespace Application.Common
    {
        public class PaginatedResponse<T>
        {
            public List<T> Items { get; set; } = new();
            public int Total { get; set; }
            public int Page { get; set; }
            public int Limit { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
        }
    }
}
