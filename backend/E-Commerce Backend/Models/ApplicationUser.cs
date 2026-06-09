using E_Commerce_Backend.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Net;

namespace E_Commerce_Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Source: API Contract GET /users/me → fullName
        public string FullName { get; set; } = string.Empty;

        // Source: API Contract GET /users/me → avatar
        public string? AvatarUrl { get; set; }

        // Source: API Contract PUT /admin/users/{id}/status → status values
        public UserStatus Status { get; set; } = UserStatus.Active;

        
        // Source: API Contract GET /users/me → createdAt
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Source: SRS §6 Admin → soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // ── Navigation Properties ───────────────────────────────────────────
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public SellerProfile? SellerProfile { get; set; }
        public Cart? Cart { get; set; }
    }


}

