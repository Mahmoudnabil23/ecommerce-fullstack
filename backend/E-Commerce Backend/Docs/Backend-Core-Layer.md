# 🏗️ Backend Core Layer Design
## E-Commerce MEAN Stack → ASP.NET Core Clean Architecture

> **Source of Truth:** SRS Document + API Contract (previously generated)
> **Architecture:** Clean Architecture — Domain / Application / Infrastructure / API layers
> **Identity:** ASP.NET Core Identity integrated into `ApplicationUser`
> **Primary Key Type:** `Guid` (matches `uuid` in API Contract)
> **Soft Delete:** Applied to User, Product, Category per SRS ("soft delete" explicitly stated)

---

## Table of Contents
1. [Enumerations](#1-enumerations)
2. [Domain Models (Entities)](#2-domain-models-entities)
3. [API Response Wrapper](#3-api-response-wrapper)
4. [Request DTOs](#4-request-dtos)
5. [Response DTOs](#5-response-dtos)
6. [Pagination Wrapper](#6-pagination-wrapper)
7. [Generic Repository Interface](#7-generic-repository-interface)
8. [Specific Repository Interfaces](#8-specific-repository-interfaces)
9. [Unit of Work Interface](#9-unit-of-work-interface)
10. [Conflicts & Missing Definitions](#10-conflicts--missing-definitions)

---

## 1. Enumerations

> Derived strictly from API Contract status values and SRS role definitions.

```csharp
// ─── Domain/Enums/UserRole.cs ───────────────────────────────────────────────
namespace Domain.Enums
{
    public enum UserRole
    {
        Customer,
        Seller,
        Admin
    }
}

// ─── Domain/Enums/UserStatus.cs ─────────────────────────────────────────────
namespace Domain.Enums
{
    // Source: PUT /admin/users/{id}/status → status values: active | suspended | banned
    public enum UserStatus
    {
        Active,
        Suspended,
        Banned
    }
}

// ─── Domain/Enums/SellerStatus.cs ───────────────────────────────────────────
namespace Domain.Enums
{
    // Source: POST /sellers/register → status = "pending"; PUT /admin/sellers/{id}/verify
    public enum SellerStatus
    {
        Pending,
        Approved,
        Rejected
    }
}

// ─── Domain/Enums/OrderStatus.cs ────────────────────────────────────────────
namespace Domain.Enums
{
    // Source: API Contract § 1.6 PUT /orders/{id}/status
    // Flow: pending → confirmed → processing → shipped → delivered → completed
    // Cancellation: pending/confirmed → cancelled
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Completed,
        Cancelled
    }
}

// ─── Domain/Enums/PaymentMethod.cs ──────────────────────────────────────────
namespace Domain.Enums
{
    // Source: SRS §3 + API Contract POST /orders → paymentMethod values
    public enum PaymentMethod
    {
        Stripe,
        PayPal,
        CashOnDelivery,
        Wallet
    }
}

// ─── Domain/Enums/PaymentStatus.cs ──────────────────────────────────────────
namespace Domain.Enums
{
    // Source: API Contract GET /orders/{id} → paymentStatus field
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
}

// ─── Domain/Enums/PromoCodeType.cs ──────────────────────────────────────────
namespace Domain.Enums
{
    // Source: API Contract POST /admin/promo-codes → type: "percentage" | "fixed"
    // "fixed" inferred — percentage explicitly stated; fixed is the only other standard type
    public enum PromoCodeType
    {
        Percentage,
        Fixed
    }
}

// ─── Domain/Enums/BannerPosition.cs ─────────────────────────────────────────
namespace Domain.Enums
{
    // Source: API Contract POST /admin/banners → position field
    // Specific values: NOT DEFINED IN SRS/API CONTRACT — using int ordering instead
    public enum BannerPosition
    {
        First  = 1,
        Second = 2,
        Third  = 3,
        Fourth = 4
    }
}
```

---

## 2. Domain Models (Entities)

### 2.1 ApplicationUser

```csharp
// ─── Domain/Entities/ApplicationUser.cs ─────────────────────────────────────
// Extends ASP.NET Core IdentityUser for built-in auth infrastructure.
// Source: SRS §1 + API Contract §1.1 / §1.2

using Microsoft.AspNetCore.Identity;
using Domain.Enums;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // Source: API Contract GET /users/me → fullName
        public string FullName { get; set; } = string.Empty;

        // Source: API Contract GET /users/me → avatar
        public string? AvatarUrl { get; set; }

        // Source: SRS §1 → Multi-user roles
        public UserRole Role { get; set; } = UserRole.Customer;

        // Source: API Contract PUT /admin/users/{id}/status → status values
        public UserStatus Status { get; set; } = UserStatus.Active;

        // Source: API Contract POST /auth/register → email verification flow
        public bool IsEmailVerified { get; set; } = false;

        // Source: API Contract GET /auth/verify-email?token={token}
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        // Source: API Contract POST /auth/forgot-password → reset token (expiry: 1h noted in contract)
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Source: API Contract POST /auth/refresh-token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

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
```

---

### 2.2 Address

```csharp
// ─── Domain/Entities/Address.cs ─────────────────────────────────────────────
// Source: API Contract §1.2 POST /users/me/addresses
// Source: API Contract GET /users/me → addresses array

namespace Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }

        // Source: API Contract → label field
        public string Label { get; set; } = string.Empty;

        // Source: API Contract → street field
        public string Street { get; set; } = string.Empty;

        // Source: API Contract → city field
        public string City { get; set; } = string.Empty;

        // Source: API Contract POST /users/me/addresses → postalCode
        public string? PostalCode { get; set; }

        // Source: API Contract → isDefault flag
        public bool IsDefault { get; set; } = false;

        // ── Foreign Key ─────────────────────────────────────────────────────
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
```

---

### 2.3 Category

```csharp
// ─── Domain/Entities/Category.cs ────────────────────────────────────────────
// Source: SRS §2 + API Contract §1.4 GET /categories (tree structure)

namespace Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /categories → name
        public string Name { get; set; } = string.Empty;

        // Source: API Contract GET /categories → slug
        public string Slug { get; set; } = string.Empty;

        // Source: API Contract GET /categories → imageUrl
        public string? ImageUrl { get; set; }

        // Source: API Contract GET /categories → parentId (tree structure / children)
        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        // Source: SRS §6 → soft delete
        public bool IsDeleted { get; set; } = false;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
```

---

### 2.4 Product

```csharp
// ─── Domain/Entities/Product.cs ─────────────────────────────────────────────
// Source: SRS §2 + API Contract §1.3

namespace Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /products → name
        public string Name { get; set; } = string.Empty;

        // Source: API Contract GET /products → slug
        public string Slug { get; set; } = string.Empty;

        // Source: SRS §2 → descriptions
        public string Description { get; set; } = string.Empty;

        // Source: API Contract GET /products → price
        public decimal Price { get; set; }

        // Source: API Contract GET /products → discountedPrice
        public decimal? DiscountedPrice { get; set; }

        // Source: SRS §2 → stock availability
        public int Stock { get; set; }

        // Source: API Contract GET /products → specs (JSON string in request)
        // Stored as serialized JSON; NOT DEFINED as structured sub-entity in SRS/API Contract
        public string? SpecsJson { get; set; }

        // Source: SRS §2 → soft delete implied by admin soft-delete policy
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public Guid SellerId { get; set; }
        public SellerProfile Seller { get; set; } = null!;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
```

---

### 2.5 ProductImage

```csharp
// ─── Domain/Entities/ProductImage.cs ────────────────────────────────────────
// Source: API Contract §1.3 POST /products → images field (array of URLs)
// Source: API Contract DELETE /products/{id}/images/{imageId}

namespace Domain.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /products → images array (URL strings)
        public string ImageUrl { get; set; } = string.Empty;

        // Display order — required by DELETE /products/{id}/images/{imageId} existence
        // NOT DEFINED IN SRS/API CONTRACT as explicit field; required by multi-image model
        public int DisplayOrder { get; set; }

        // ── Foreign Key ─────────────────────────────────────────────────────
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
```

---

### 2.6 SellerProfile

```csharp
// ─── Domain/Entities/SellerProfile.cs ───────────────────────────────────────
// Source: SRS §7 + API Contract §1.11

using Domain.Enums;

namespace Domain.Entities
{
    public class SellerProfile
    {
        public Guid Id { get; set; }

        // Source: API Contract POST /sellers/register → storeName
        public string StoreName { get; set; } = string.Empty;

        // Source: API Contract POST /sellers/register → storeDescription
        public string? StoreDescription { get; set; }

        // Source: API Contract POST /sellers/register → taxId
        public string? TaxId { get; set; }

        // Source: API Contract PUT /admin/sellers/{id}/verify → verified bool
        public SellerStatus Status { get; set; } = SellerStatus.Pending;

        // Source: API Contract GET /sellers/me → rating (public store page)
        // Computed/cached value — NOT DEFINED as formula in SRS/API Contract
        public decimal AverageRating { get; set; } = 0;

        // Source: API Contract POST /sellers/register → bankAccount object
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountHolderName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Key (1-to-1 with ApplicationUser) ───────────────────────
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
```

---

### 2.7 Cart

```csharp
// ─── Domain/Entities/Cart.cs ─────────────────────────────────────────────────
// Source: SRS §3 + API Contract §1.5
// Supports both authenticated users and guests (X-Guest-Session header)

namespace Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }

        // Source: API Contract → guest carts use session ID; nullable for guest carts
        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Source: API Contract Headers → X-Guest-Session: {uuid}
        public string? GuestSessionId { get; set; }

        // Source: API Contract GET /cart → appliedPromoCode (nullable)
        public Guid? AppliedPromoCodeId { get; set; }
        public PromoCode? AppliedPromoCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
```

---

### 2.8 CartItem

```csharp
// ─── Domain/Entities/CartItem.cs ────────────────────────────────────────────
// Source: API Contract §1.5 GET /cart → items array

namespace Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /cart → quantity
        public int Quantity { get; set; }

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
```

---

### 2.9 Order

```csharp
// ─── Domain/Entities/Order.cs ────────────────────────────────────────────────
// Source: SRS §4 + API Contract §1.6

using Domain.Enums;

namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        // Source: API Contract → orderNumber (e.g., "ORD-20240115-00042")
        public string OrderNumber { get; set; } = string.Empty;

        // Source: API Contract → status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Source: API Contract GET /orders/{id} → paymentMethod
        public PaymentMethod PaymentMethod { get; set; }

        // Source: API Contract GET /orders/{id} → paymentStatus
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Source: API Contract GET /orders/{id} → trackingNumber
        public string? TrackingNumber { get; set; }

        // Source: API Contract POST /orders → notes
        public string? Notes { get; set; }

        // Source: API Contract GET /orders/{id} → total
        public decimal Total { get; set; }

        // Source: API Contract POST /orders → promoCode applied
        public Guid? PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }

        // Source: API Contract → estimatedDelivery
        // NOT DEFINED IN SRS/API CONTRACT as a calculation rule; stored as provided
        public DateTime? EstimatedDelivery { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Shipping Address Snapshot ────────────────────────────────────────
        // Snapshot (not FK) — address must not change after order placed
        // Source: API Contract GET /orders/{id} → address object
        public string ShippingStreet { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string? ShippingPostalCode { get; set; }

        // ── Foreign Keys ────────────────────────────────────────────────────
        // Nullable: guest checkout allowed per SRS §3
        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Source: API Contract POST /orders → guest requires email
        public string? GuestEmail { get; set; }

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}
```

---

### 2.10 OrderItem

```csharp
// ─── Domain/Entities/OrderItem.cs ───────────────────────────────────────────
// Source: API Contract GET /orders/{id} → items array

namespace Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        // Source: API Contract → qty
        public int Quantity { get; set; }

        // Source: API Contract → unitPrice (snapshot at time of order)
        public decimal UnitPrice { get; set; }

        // Source: API Contract → name (snapshot — product name at order time)
        public string ProductName { get; set; } = string.Empty;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
```

---

### 2.11 OrderStatusHistory

```csharp
// ─── Domain/Entities/OrderStatusHistory.cs ──────────────────────────────────
// Source: API Contract GET /orders/{id} → statusHistory array

using Domain.Enums;

namespace Domain.Entities
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; }

        // Source: API Contract → status
        public OrderStatus Status { get; set; }

        // Source: API Contract → timestamp
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ── Foreign Key ─────────────────────────────────────────────────────
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
```

---

### 2.12 Review

```csharp
// ─── Domain/Entities/Review.cs ──────────────────────────────────────────────
// Source: SRS §1 + API Contract §1.8

namespace Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /products/{id}/reviews → rating (1–5)
        public int Rating { get; set; }

        // Source: API Contract → title
        public string Title { get; set; } = string.Empty;

        // Source: API Contract → body
        public string Body { get; set; } = string.Empty;

        // Source: API Contract → images (array of URLs)
        // Stored as JSON array string; no separate ReviewImage entity defined in SRS/API Contract
        public string? ImagesJson { get; set; }

        // Source: API Contract → isVerifiedPurchase
        public bool IsVerifiedPurchase { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
```

---

### 2.13 WishlistItem

```csharp
// ─── Domain/Entities/WishlistItem.cs ────────────────────────────────────────
// Source: SRS §1 + API Contract §1.9

namespace Domain.Entities
{
    public class WishlistItem
    {
        public Guid Id { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
```

---

### 2.14 PromoCode

```csharp
// ─── Domain/Entities/PromoCode.cs ───────────────────────────────────────────
// Source: SRS §3 (Bonus) + API Contract §1.10 POST /admin/promo-codes

using Domain.Enums;

namespace Domain.Entities
{
    public class PromoCode
    {
        public Guid Id { get; set; }

        // Source: API Contract → code
        public string Code { get; set; } = string.Empty;

        // Source: API Contract → type: "percentage" | implicit "fixed"
        public PromoCodeType Type { get; set; }

        // Source: API Contract → value
        public decimal Value { get; set; }

        // Source: API Contract → minOrderAmount
        public decimal MinOrderAmount { get; set; }

        // Source: API Contract → usageLimit
        public int UsageLimit { get; set; }

        // Tracks current usage — required by usageLimit enforcement
        public int UsageCount { get; set; } = 0;

        // Source: API Contract → expiresAt
        public DateTime ExpiresAt { get; set; }

        // Source: API Contract → applicableCategories (array of category IDs)
        // Stored as JSON; no join table defined in SRS/API Contract
        public string? ApplicableCategoriesJson { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

---

### 2.15 Banner

```csharp
// ─── Domain/Entities/Banner.cs ──────────────────────────────────────────────
// Source: SRS §6 Admin + API Contract §1.10 POST /admin/banners

namespace Domain.Entities
{
    public class Banner
    {
        public Guid Id { get; set; }

        // Source: API Contract → title
        public string Title { get; set; } = string.Empty;

        // Source: API Contract → imageFile → stored as URL after upload
        public string ImageUrl { get; set; } = string.Empty;

        // Source: API Contract → linkUrl
        public string? LinkUrl { get; set; }

        // Source: API Contract → position
        public int Position { get; set; }

        // Source: API Contract → isActive
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

---

### 2.16 Notification

```csharp
// ─── Domain/Entities/Notification.cs ────────────────────────────────────────
// Source: SRS §4 (order notifications) + API Contract §1.13

namespace Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /notifications → type (order updates, promotions)
        // NOT DEFINED as enum in SRS/API Contract — stored as string
        public string Type { get; set; } = string.Empty;

        // Source: API Contract → message/body content
        // NOT DEFINED as explicit field name in SRS/API Contract
        public string Message { get; set; } = string.Empty;

        // Source: API Contract GET /notifications?read=false → read flag
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Key ─────────────────────────────────────────────────────
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
```

---

## 3. API Response Wrapper

```csharp
// ─── Application/Common/ApiResponse.cs ──────────────────────────────────────
// Source: API Contract — Standard Error/Success Envelope (non-negotiable)

namespace Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        // ── Static Factories ────────────────────────────────────────────────
        public static ApiResponse<T> Ok(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    // Non-generic variant for endpoints returning no data body (204-style with envelope)
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse OkNoData(string message = "")
            => new() { Success = true, Message = message };
    }
}
```

---

## 4. Request DTOs

### 4.1 Auth Request DTOs

```csharp
// ─── Application/DTOs/Auth/Requests/ ────────────────────────────────────────

// POST /auth/register
public class RegisterRequestDto
{
    public string FullName    { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Phone       { get; set; } = string.Empty;
    public string Password    { get; set; } = string.Empty;
    public UserRole Role      { get; set; } = UserRole.Customer;
}

// POST /auth/login
public class LoginRequestDto
{
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// POST /auth/refresh-token
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

// POST /auth/logout
public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

// POST /auth/forgot-password
public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}

// POST /auth/reset-password
public class ResetPasswordRequestDto
{
    public string Token       { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

// POST /auth/google (Bonus)
public class GoogleAuthRequestDto
{
    public string IdToken { get; set; } = string.Empty;
}

// POST /auth/change-password
public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword     { get; set; } = string.Empty;
}
```

---

### 4.2 User / Profile Request DTOs

```csharp
// ─── Application/DTOs/Users/Requests/ ───────────────────────────────────────

// PUT /users/me
public class UpdateProfileRequestDto
{
    public string? FullName { get; set; }
    public string? Phone    { get; set; }
}

// POST /users/me/addresses  |  PUT /users/me/addresses/{id}
public class UpsertAddressRequestDto
{
    public string  Label      { get; set; } = string.Empty;
    public string  Street     { get; set; } = string.Empty;
    public string  City       { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool    IsDefault  { get; set; } = false;
}
```

---

### 4.3 Product Request DTOs

```csharp
// ─── Application/DTOs/Products/Requests/ ────────────────────────────────────

// POST /products  (multipart/form-data — image files handled separately)
public class CreateProductRequestDto
{
    public string  Name        { get; set; } = string.Empty;
    public string  Description { get; set; } = string.Empty;
    public decimal Price       { get; set; }
    public Guid    CategoryId  { get; set; }
    public int     Stock       { get; set; }
    public string? SpecsJson   { get; set; }
    // Images are IFormFile[] — handled in controller, not in this DTO
}

// PUT /products/{id}
public class UpdateProductRequestDto
{
    public string?  Name        { get; set; }
    public string?  Description { get; set; }
    public decimal? Price       { get; set; }
    public Guid?    CategoryId  { get; set; }
    public int?     Stock       { get; set; }
    public string?  SpecsJson   { get; set; }
}

// GET /products — query parameters
public class ProductFilterRequestDto
{
    public string?  Search     { get; set; }
    public Guid?    CategoryId { get; set; }
    public decimal? MinPrice   { get; set; }
    public decimal? MaxPrice   { get; set; }
    public Guid?    SellerId   { get; set; }
    public bool?    InStock    { get; set; }
    public string?  SortBy     { get; set; }  // price_asc | price_desc | newest | rating
    public int      Page       { get; set; } = 1;
    public int      Limit      { get; set; } = 20;
}
```

---

### 4.4 Category Request DTOs

```csharp
// ─── Application/DTOs/Categories/Requests/ ──────────────────────────────────

// POST /categories  |  PUT /categories/{id}
public class UpsertCategoryRequestDto
{
    public string  Name     { get; set; } = string.Empty;
    public Guid?   ParentId { get; set; }
    public string? ImageUrl { get; set; }
}
```

---

### 4.5 Cart Request DTOs

```csharp
// ─── Application/DTOs/Cart/Requests/ ────────────────────────────────────────

// POST /cart/items
public class AddCartItemRequestDto
{
    public Guid ProductId { get; set; }
    public int  Quantity  { get; set; }
}

// PUT /cart/items/{cartItemId}
public class UpdateCartItemRequestDto
{
    public int Quantity { get; set; }
}

// POST /cart/promo
public class ApplyPromoCodeRequestDto
{
    public string Code { get; set; } = string.Empty;
}

// POST /cart/merge
public class MergeCartRequestDto
{
    public string GuestSessionId { get; set; } = string.Empty;
}
```

---

### 4.6 Order Request DTOs

```csharp
// ─── Application/DTOs/Orders/Requests/ ──────────────────────────────────────

// POST /orders
public class CreateOrderRequestDto
{
    public Guid?         AddressId     { get; set; }   // Nullable: guest may provide inline address
    public PaymentMethod PaymentMethod { get; set; }
    public string?       PaymentToken  { get; set; }
    public string?       PromoCode     { get; set; }
    public string?       Notes         { get; set; }
}

// PUT /orders/{id}/status
public class UpdateOrderStatusRequestDto
{
    public OrderStatus Status         { get; set; }
    public string?     TrackingNumber { get; set; }
}

// POST /orders/{id}/cancel
public class CancelOrderRequestDto
{
    public string? Reason { get; set; }
}
```

---

### 4.7 Review Request DTOs

```csharp
// ─── Application/DTOs/Reviews/Requests/ ─────────────────────────────────────

// POST /products/{productId}/reviews
public class CreateReviewRequestDto
{
    public int    Rating { get; set; }
    public string Title  { get; set; } = string.Empty;
    public string Body   { get; set; } = string.Empty;
    // images: NOT DEFINED as file upload or URL list in API Contract for this endpoint
    // Storing as optional list of URL strings per response schema
    public List<string>? Images { get; set; }
}

// PUT /products/{productId}/reviews/{reviewId}
public class UpdateReviewRequestDto
{
    public int?    Rating { get; set; }
    public string? Title  { get; set; }
    public string? Body   { get; set; }
}
```

---

### 4.8 Seller Request DTOs

```csharp
// ─── Application/DTOs/Sellers/Requests/ ─────────────────────────────────────

// POST /sellers/register
public class SellerRegistrationRequestDto
{
    public string  StoreName        { get; set; } = string.Empty;
    public string? StoreDescription { get; set; }
    public string? TaxId            { get; set; }
    public BankAccountDto BankAccount { get; set; } = new();
}

public class BankAccountDto
{
    public string BankName              { get; set; } = string.Empty;
    public string AccountNumber         { get; set; } = string.Empty;
    public string AccountHolderName     { get; set; } = string.Empty;
}

// PUT /sellers/me
public class UpdateSellerProfileRequestDto
{
    public string? StoreName        { get; set; }
    public string? StoreDescription { get; set; }
}
```

---

### 4.9 Admin Request DTOs

```csharp
// ─── Application/DTOs/Admin/Requests/ ───────────────────────────────────────

// PUT /admin/users/{id}/status
public class UpdateUserStatusRequestDto
{
    public UserStatus Status { get; set; }
    public string?    Reason { get; set; }
}

// PUT /admin/sellers/{id}/verify
public class VerifySellerRequestDto
{
    public bool Verified { get; set; }
}

// POST /admin/promo-codes  |  PUT /admin/promo-codes/{id}
public class UpsertPromoCodeRequestDto
{
    public string        Code                      { get; set; } = string.Empty;
    public PromoCodeType Type                      { get; set; }
    public decimal       Value                     { get; set; }
    public decimal       MinOrderAmount            { get; set; }
    public int           UsageLimit                { get; set; }
    public DateTime      ExpiresAt                 { get; set; }
    public List<Guid>?   ApplicableCategories      { get; set; }
}

// POST /admin/banners  (multipart/form-data — imageFile handled in controller)
public class CreateBannerRequestDto
{
    public string  Title    { get; set; } = string.Empty;
    public string? LinkUrl  { get; set; }
    public int     Position { get; set; }
    public bool    IsActive { get; set; } = true;
}

// PUT /admin/banners/{id}
public class UpdateBannerRequestDto
{
    public string? Title    { get; set; }
    public string? LinkUrl  { get; set; }
    public int?    Position { get; set; }
    public bool?   IsActive { get; set; }
}

// GET /admin/users — query params
public class AdminUserFilterRequestDto
{
    public UserRole?   Role   { get; set; }
    public UserStatus? Status { get; set; }
    public string?     Search { get; set; }
    public int         Page   { get; set; } = 1;
    public int         Limit  { get; set; } = 20;
}

// GET /admin/orders — query params
public class AdminOrderFilterRequestDto
{
    public OrderStatus? Status    { get; set; }
    public DateTime?    StartDate { get; set; }
    public DateTime?    EndDate   { get; set; }
    public int          Page      { get; set; } = 1;
    public int          Limit     { get; set; } = 20;
}
```

---

### 4.10 Payment Request DTOs

```csharp
// ─── Application/DTOs/Payments/Requests/ ────────────────────────────────────

// POST /payments/initiate
public class InitiatePaymentRequestDto
{
    public Guid          OrderId       { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string        Currency      { get; set; } = "EGP";
}

// POST /payments/wallet/topup (Bonus)
public class WalletTopupRequestDto
{
    public decimal       Amount        { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string?       PaymentToken  { get; set; }
}
```

---

### 4.11 Notification Request DTOs

```csharp
// ─── Application/DTOs/Notifications/Requests/ ───────────────────────────────

// GET /notifications — query params
public class NotificationFilterRequestDto
{
    public bool? Read  { get; set; }
    public int   Page  { get; set; } = 1;
    public int   Limit { get; set; } = 20;
}
```

---

## 5. Response DTOs

### 5.1 Auth Response DTOs

```csharp
// ─── Application/DTOs/Auth/Responses/ ───────────────────────────────────────

// POST /auth/login | POST /auth/register (on login) | POST /auth/refresh-token
public class AuthResponseDto
{
    public string      AccessToken  { get; set; } = string.Empty;
    public string      RefreshToken { get; set; } = string.Empty;
    public int         ExpiresIn    { get; set; }  // seconds
    public UserSummaryDto User      { get; set; } = new();
}

public class UserSummaryDto
{
    public Guid     Id       { get; set; }
    public string   FullName { get; set; } = string.Empty;
    public UserRole Role     { get; set; }
}

// POST /auth/register (201 — before email verification)
public class RegisterResponseDto
{
    public Guid UserId { get; set; }
}
```

---

### 5.2 User / Profile Response DTOs

```csharp
// ─── Application/DTOs/Users/Responses/ ──────────────────────────────────────

// GET /users/me
public class UserProfileResponseDto
{
    public Guid              Id             { get; set; }
    public string            FullName       { get; set; } = string.Empty;
    public string            Email          { get; set; } = string.Empty;
    public string?           Phone          { get; set; }
    public UserRole          Role           { get; set; }
    public string?           Avatar         { get; set; }
    public List<AddressResponseDto> Addresses { get; set; } = new();
    public DateTime          CreatedAt      { get; set; }
}

// Address item (used in GET /users/me and standalone address endpoints)
public class AddressResponseDto
{
    public Guid    Id         { get; set; }
    public string  Label      { get; set; } = string.Empty;
    public string  Street     { get; set; } = string.Empty;
    public string  City       { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool    IsDefault  { get; set; }
}

// POST /users/me/avatar
public class AvatarUploadResponseDto
{
    public string AvatarUrl { get; set; } = string.Empty;
}

// GET /admin/users list item
public class AdminUserListItemDto
{
    public Guid       Id       { get; set; }
    public string     FullName { get; set; } = string.Empty;
    public string     Email    { get; set; } = string.Empty;
    public UserRole   Role     { get; set; }
    public UserStatus Status   { get; set; }
    public DateTime   CreatedAt { get; set; }
}
```

---

### 5.3 Product Response DTOs

```csharp
// ─── Application/DTOs/Products/Responses/ ───────────────────────────────────

// GET /products → items array item
public class ProductListItemDto
{
    public Guid            Id              { get; set; }
    public string          Name            { get; set; } = string.Empty;
    public string          Slug            { get; set; } = string.Empty;
    public decimal         Price           { get; set; }
    public decimal?        DiscountedPrice { get; set; }
    public List<string>    Images          { get; set; } = new();
    public CategoryRefDto  Category        { get; set; } = new();
    public SellerRefDto    Seller          { get; set; } = new();
    public int             Stock           { get; set; }
    public decimal         AverageRating   { get; set; }
    public int             ReviewCount     { get; set; }
}

// GET /products/{id} — full detail
public class ProductDetailResponseDto : ProductListItemDto
{
    public string       Description { get; set; } = string.Empty;
    public string?      SpecsJson   { get; set; }
    public DateTime     CreatedAt   { get; set; }
}

// Inline ref objects used inside product responses
public class CategoryRefDto
{
    public Guid   Id   { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SellerRefDto
{
    public Guid   Id        { get; set; }
    public string StoreName { get; set; } = string.Empty;
}
```

---

### 5.4 Category Response DTOs

```csharp
// ─── Application/DTOs/Categories/Responses/ ─────────────────────────────────

// GET /categories → tree node
public class CategoryResponseDto
{
    public Guid                     Id       { get; set; }
    public string                   Name     { get; set; } = string.Empty;
    public string                   Slug     { get; set; } = string.Empty;
    public string?                  ImageUrl { get; set; }
    public List<CategoryResponseDto> Children { get; set; } = new();
}
```

---

### 5.5 Cart Response DTOs

```csharp
// ─── Application/DTOs/Cart/Responses/ ───────────────────────────────────────

// GET /cart
public class CartResponseDto
{
    public Guid                  CartId           { get; set; }
    public List<CartItemDto>     Items            { get; set; } = new();
    public CartSummaryDto        Summary          { get; set; } = new();
    public string?               AppliedPromoCode { get; set; }
}

public class CartItemDto
{
    public Guid               CartItemId { get; set; }
    public CartProductRefDto  Product    { get; set; } = new();
    public int                Quantity   { get; set; }
    public decimal            Subtotal   { get; set; }
}

public class CartProductRefDto
{
    public Guid    Id       { get; set; }
    public string  Name     { get; set; } = string.Empty;
    public decimal Price    { get; set; }
    public string? ImageUrl { get; set; }
}

public class CartSummaryDto
{
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total    { get; set; }
}
```

---

### 5.6 Order Response DTOs

```csharp
// ─── Application/DTOs/Orders/Responses/ ─────────────────────────────────────

// POST /orders (201)
public class OrderCreatedResponseDto
{
    public Guid        OrderId           { get; set; }
    public string      OrderNumber       { get; set; } = string.Empty;
    public OrderStatus Status            { get; set; }
    public decimal     Total             { get; set; }
    public string      PaymentStatus     { get; set; } = string.Empty;
    public DateTime?   EstimatedDelivery { get; set; }
}

// GET /orders/{id}
public class OrderDetailResponseDto
{
    public Guid                         OrderId       { get; set; }
    public string                       OrderNumber   { get; set; } = string.Empty;
    public OrderStatus                  Status        { get; set; }
    public List<OrderItemDto>           Items         { get; set; } = new();
    public OrderAddressDto              Address       { get; set; } = new();
    public PaymentMethod                PaymentMethod { get; set; }
    public PaymentStatus                PaymentStatus { get; set; }
    public string?                      TrackingNumber { get; set; }
    public List<OrderStatusHistoryDto>  StatusHistory { get; set; } = new();
    public decimal                      Total         { get; set; }
}

public class OrderItemDto
{
    public Guid    ProductId   { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public int     Qty         { get; set; }
    public decimal UnitPrice   { get; set; }
}

public class OrderAddressDto
{
    public string  Street     { get; set; } = string.Empty;
    public string  City       { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
}

public class OrderStatusHistoryDto
{
    public OrderStatus Status    { get; set; }
    public DateTime    Timestamp { get; set; }
}

// GET /orders (list item — shorter)
public class OrderListItemDto
{
    public Guid        OrderId     { get; set; }
    public string      OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status      { get; set; }
    public decimal     Total       { get; set; }
    public DateTime    CreatedAt   { get; set; }
}
```

---

### 5.7 Review Response DTOs

```csharp
// ─── Application/DTOs/Reviews/Responses/ ────────────────────────────────────

// GET /products/{productId}/reviews
public class ReviewListResponseDto
{
    public decimal              AverageRating  { get; set; }
    public int                  TotalReviews   { get; set; }
    public ReviewDistributionDto Distribution  { get; set; } = new();
    public List<ReviewItemDto>  Reviews        { get; set; } = new();
}

public class ReviewDistributionDto
{
    public int Five  { get; set; }
    public int Four  { get; set; }
    public int Three { get; set; }
    public int Two   { get; set; }
    public int One   { get; set; }
}

public class ReviewItemDto
{
    public Guid             Id                { get; set; }
    public ReviewUserDto    User              { get; set; } = new();
    public int              Rating            { get; set; }
    public string           Title             { get; set; } = string.Empty;
    public string           Body              { get; set; } = string.Empty;
    public List<string>     Images            { get; set; } = new();
    public DateTime         CreatedAt         { get; set; }
    public bool             IsVerifiedPurchase { get; set; }
}

public class ReviewUserDto
{
    public string  FullName { get; set; } = string.Empty;
    public string? Avatar   { get; set; }
}
```

---

### 5.8 Seller Response DTOs

```csharp
// ─── Application/DTOs/Sellers/Responses/ ────────────────────────────────────

// GET /sellers/me  |  GET /sellers/{id}
public class SellerProfileResponseDto
{
    public Guid         Id               { get; set; }
    public string       StoreName        { get; set; } = string.Empty;
    public string?      StoreDescription { get; set; }
    public SellerStatus Status           { get; set; }
    public decimal      AverageRating    { get; set; }
    public DateTime     CreatedAt        { get; set; }
}

// GET /sellers/me/earnings (Bonus)
public class SellerEarningsResponseDto
{
    public decimal                     TotalEarnings  { get; set; }
    public decimal                     PendingPayout  { get; set; }
    public decimal                     PaidOut        { get; set; }
    // Transactions: NOT DEFINED IN SRS/API CONTRACT (structure not specified)
    // Placeholder list — structure to be defined when transaction entity is designed
    public List<object>                Transactions   { get; set; } = new();
}

// GET /admin/sellers list item
public class AdminSellerListItemDto
{
    public Guid         Id          { get; set; }
    public string       StoreName   { get; set; } = string.Empty;
    public string       OwnerEmail  { get; set; } = string.Empty;
    public SellerStatus Status      { get; set; }
    public DateTime     CreatedAt   { get; set; }
}
```

---

### 5.9 Admin Dashboard Response DTO

```csharp
// ─── Application/DTOs/Admin/Responses/ ──────────────────────────────────────

// GET /admin/dashboard
public class AdminDashboardResponseDto
{
    public decimal                   TotalRevenue  { get; set; }
    public int                       TotalOrders   { get; set; }
    public int                       TotalUsers    { get; set; }
    public int                       TotalProducts { get; set; }
    public List<OrderListItemDto>    RecentOrders  { get; set; } = new();
    public List<ProductListItemDto>  TopProducts   { get; set; } = new();
    // salesByDay: NOT DEFINED IN SRS/API CONTRACT (structure/grouping not specified)
    public List<SalesByDayDto>       SalesByDay    { get; set; } = new();
}

public class SalesByDayDto
{
    // NOT DEFINED IN SRS/API CONTRACT — minimal definition
    public DateTime Date   { get; set; }
    public decimal  Amount { get; set; }
}
```

---

### 5.10 Payment Response DTOs

```csharp
// ─── Application/DTOs/Payments/Responses/ ───────────────────────────────────

// POST /payments/initiate
public class PaymentInitiateResponseDto
{
    public string ClientSecret    { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
}

// GET /payments/wallet/balance (Bonus)
public class WalletBalanceResponseDto
{
    public decimal Balance  { get; set; }
    public string  Currency { get; set; } = "EGP";
}
```

---

### 5.11 Promo Code Response DTOs

```csharp
// ─── Application/DTOs/PromoCodes/Responses/ ─────────────────────────────────

public class PromoCodeResponseDto
{
    public Guid          Id             { get; set; }
    public string        Code           { get; set; } = string.Empty;
    public PromoCodeType Type           { get; set; }
    public decimal       Value          { get; set; }
    public decimal       MinOrderAmount { get; set; }
    public int           UsageLimit     { get; set; }
    public int           UsageCount     { get; set; }
    public DateTime      ExpiresAt      { get; set; }
    public bool          IsActive       { get; set; }
}
```

---

### 5.12 Banner Response DTO

```csharp
// ─── Application/DTOs/Banners/Responses/ ────────────────────────────────────

public class BannerResponseDto
{
    public Guid    Id       { get; set; }
    public string  Title    { get; set; } = string.Empty;
    public string  ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl  { get; set; }
    public int     Position { get; set; }
    public bool    IsActive { get; set; }
}
```

---

### 5.13 Notification Response DTO

```csharp
// ─── Application/DTOs/Notifications/Responses/ ──────────────────────────────

public class NotificationResponseDto
{
    public Guid     Id        { get; set; }
    public string   Type      { get; set; } = string.Empty;
    public string   Message   { get; set; } = string.Empty;
    public bool     IsRead    { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## 6. Pagination Wrapper

```csharp
// ─── Application/Common/PaginatedResponse.cs ────────────────────────────────
// Used by all list endpoints in the API Contract that return paginated data.
// Source: API Contract → pagination object: { total, page, limit, totalPages }

namespace Application.Common
{
    public class PaginatedResponse<T>
    {
        public List<T> Items      { get; set; } = new();
        public int     Total      { get; set; }
        public int     Page       { get; set; }
        public int     Limit      { get; set; }
        public int     TotalPages => (int)Math.Ceiling((double)Total / Limit);
    }
}
```

---

## 7. Generic Repository Interface

```csharp
// ─── Application/Interfaces/Repositories/IGenericRepository.cs ──────────────

using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Returns IQueryable for deferred execution — callers project and filter
        IQueryable<T> GetAll();

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        Task<T?> GetByIdAsync(Guid id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
```

---

## 8. Specific Repository Interfaces

### 8.1 IUserRepository

```csharp
// ─── Application/Interfaces/Repositories/IUserRepository.cs ─────────────────
// Methods required by: §1.1 auth flows, §1.2 profile, §1.10 admin user mgmt

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        // Required by: POST /auth/login, POST /auth/register duplicate check
        Task<ApplicationUser?> GetByEmailAsync(string email);

        // Required by: POST /auth/register duplicate check
        Task<ApplicationUser?> GetByPhoneAsync(string phone);

        // Required by: GET /auth/verify-email?token={token}
        Task<ApplicationUser?> GetByEmailVerificationTokenAsync(string token);

        // Required by: POST /auth/reset-password
        Task<ApplicationUser?> GetByPasswordResetTokenAsync(string token);

        // Required by: POST /auth/refresh-token, POST /auth/logout
        Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken);

        // Required by: GET /admin/users with role/status/search filters + pagination
        Task<(List<ApplicationUser> Users, int Total)> GetPagedAsync(
            UserRole?   role,
            UserStatus? status,
            string?     search,
            int         page,
            int         limit);
    }
}
```

---

### 8.2 IAddressRepository

```csharp
// ─── Application/Interfaces/Repositories/IAddressRepository.cs ──────────────
// Methods required by: §1.2 address management endpoints

namespace Application.Interfaces.Repositories
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        // Required by: GET /users/me → addresses, POST /orders → addressId validation
        Task<List<Address>> GetByUserIdAsync(Guid userId);

        // Required by: POST /users/me/addresses → clear old default when setting new one
        Task<Address?> GetDefaultByUserIdAsync(Guid userId);
    }
}
```

---

### 8.3 ICategoryRepository

```csharp
// ─── Application/Interfaces/Repositories/ICategoryRepository.cs ─────────────
// Methods required by: §1.4 category endpoints

namespace Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Required by: GET /categories → full tree structure
        Task<List<Category>> GetTreeAsync();

        // Required by: POST /categories slug uniqueness check
        Task<bool> SlugExistsAsync(string slug);
    }
}
```

---

### 8.4 IProductRepository

```csharp
// ─── Application/Interfaces/Repositories/IProductRepository.cs ──────────────
// Methods required by: §1.3 product endpoints

namespace Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Required by: GET /products with all filters + pagination
        Task<(List<Product> Products, int Total)> GetPagedAsync(ProductFilterRequestDto filter);

        // Required by: GET /categories/{id}/products
        Task<(List<Product> Products, int Total)> GetByCategoryPagedAsync(
            Guid   categoryId,
            int    page,
            int    limit);

        // Required by: GET /sellers/me/products
        Task<(List<Product> Products, int Total)> GetBySellerPagedAsync(
            Guid    sellerId,
            string? status,
            int     page,
            int     limit);

        // Required by: GET /products/{id} slug-based URL support
        Task<Product?> GetBySlugAsync(string slug);

        // Required by: GET /admin/dashboard → topProducts
        Task<List<Product>> GetTopProductsAsync(int count);
    }
}
```

---

### 8.5 IProductImageRepository

```csharp
// ─── Application/Interfaces/Repositories/IProductImageRepository.cs ──────────
// Methods required by: §1.3 image management endpoints

namespace Application.Interfaces.Repositories
{
    public interface IProductImageRepository : IGenericRepository<ProductImage>
    {
        // Required by: DELETE /products/{id}/images/{imageId} ownership check
        Task<ProductImage?> GetByIdAndProductAsync(Guid imageId, Guid productId);
    }
}
```

---

### 8.6 ISellerProfileRepository

```csharp
// ─── Application/Interfaces/Repositories/ISellerProfileRepository.cs ─────────
// Methods required by: §1.11 seller endpoints

namespace Application.Interfaces.Repositories
{
    public interface ISellerProfileRepository : IGenericRepository<SellerProfile>
    {
        // Required by: GET /sellers/me — resolved from authenticated user
        Task<SellerProfile?> GetByUserIdAsync(Guid userId);

        // Required by: GET /admin/sellers — pagination with status filter
        Task<(List<SellerProfile> Sellers, int Total)> GetPagedAsync(
            SellerStatus? status,
            int           page,
            int           limit);
    }
}
```

---

### 8.7 ICartRepository

```csharp
// ─── Application/Interfaces/Repositories/ICartRepository.cs ─────────────────
// Methods required by: §1.5 cart endpoints

namespace Application.Interfaces.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        // Required by: GET /cart for authenticated user
        Task<Cart?> GetByUserIdAsync(Guid userId);

        // Required by: GET /cart for guest user (X-Guest-Session header)
        Task<Cart?> GetByGuestSessionAsync(string guestSessionId);

        // Required by: POST /cart/merge — load guest cart with items
        Task<Cart?> GetByGuestSessionWithItemsAsync(string guestSessionId);
    }
}
```

---

### 8.8 ICartItemRepository

```csharp
// ─── Application/Interfaces/Repositories/ICartItemRepository.cs ─────────────
// Methods required by: §1.5 cart item endpoints

namespace Application.Interfaces.Repositories
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        // Required by: PUT /cart/items/{cartItemId} — validate ownership
        Task<CartItem?> GetByIdAndCartAsync(Guid cartItemId, Guid cartId);

        // Required by: POST /cart/items — check if product already in cart
        Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId);
    }
}
```

---

### 8.9 IOrderRepository

```csharp
// ─── Application/Interfaces/Repositories/IOrderRepository.cs ────────────────
// Methods required by: §1.6 order endpoints

namespace Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Required by: GET /users/me/orders — customer's own orders with pagination
        Task<(List<Order> Orders, int Total)> GetByUserPagedAsync(
            Guid         userId,
            OrderStatus? status,
            int          page,
            int          limit);

        // Required by: GET /admin/orders — all orders with filters
        Task<(List<Order> Orders, int Total)> GetAllPagedAsync(
            OrderStatus? status,
            DateTime?    startDate,
            DateTime?    endDate,
            int          page,
            int          limit);

        // Required by: GET /sellers/me/orders — orders containing seller products
        Task<(List<Order> Orders, int Total)> GetBySellerPagedAsync(
            Guid    sellerId,
            int     page,
            int     limit);

        // Required by: GET /orders/{id} — with full navigation (items, history, address)
        Task<Order?> GetByIdWithDetailsAsync(Guid orderId);

        // Required by: GET /admin/dashboard → totalOrders, recentOrders
        Task<int>         GetTotalCountAsync();
        Task<List<Order>> GetRecentAsync(int count);

        // Required by: GET /admin/dashboard → totalRevenue
        Task<decimal> GetTotalRevenueAsync();
    }
}
```

---

### 8.10 IReviewRepository

```csharp
// ─── Application/Interfaces/Repositories/IReviewRepository.cs ───────────────
// Methods required by: §1.8 review endpoints

namespace Application.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        // Required by: GET /products/{productId}/reviews — paginated
        Task<(List<Review> Reviews, int Total)> GetByProductPagedAsync(
            Guid productId,
            int? rating,
            int  page,
            int  limit);

        // Required by: POST /products/{productId}/reviews → 409 duplicate check
        Task<bool> ExistsForUserAndProductAsync(Guid userId, Guid productId);

        // Required by: POST /products/{productId}/reviews → 403 verified purchase check
        Task<bool> UserHasPurchasedProductAsync(Guid userId, Guid productId);

        // Required by: GET /products → averageRating, reviewCount
        Task<(decimal Average, int Count)> GetRatingSummaryAsync(Guid productId);
    }
}
```

---

### 8.11 IWishlistRepository

```csharp
// ─── Application/Interfaces/Repositories/IWishlistRepository.cs ─────────────
// Methods required by: §1.9 wishlist endpoints

namespace Application.Interfaces.Repositories
{
    public interface IWishlistRepository : IGenericRepository<WishlistItem>
    {
        // Required by: GET /wishlist
        Task<List<WishlistItem>> GetByUserIdAsync(Guid userId);

        // Required by: DELETE /wishlist/{productId} and move-to-cart
        Task<WishlistItem?> GetByUserAndProductAsync(Guid userId, Guid productId);

        // Required by: POST /wishlist → 409 duplicate check
        Task<bool> ExistsAsync(Guid userId, Guid productId);
    }
}
```

---

### 8.12 IPromoCodeRepository

```csharp
// ─── Application/Interfaces/Repositories/IPromoCodeRepository.cs ─────────────
// Methods required by: §1.10 admin promo codes + §1.5 POST /cart/promo

namespace Application.Interfaces.Repositories
{
    public interface IPromoCodeRepository : IGenericRepository<PromoCode>
    {
        // Required by: POST /cart/promo — validate and retrieve by code string
        Task<PromoCode?> GetByCodeAsync(string code);

        // Required by: GET /admin/promo-codes — paginated list
        Task<(List<PromoCode> Codes, int Total)> GetPagedAsync(int page, int limit);
    }
}
```

---

### 8.13 IBannerRepository

```csharp
// ─── Application/Interfaces/Repositories/IBannerRepository.cs ───────────────
// Methods required by: §1.10 banner management endpoints

namespace Application.Interfaces.Repositories
{
    public interface IBannerRepository : IGenericRepository<Banner>
    {
        // Required by: Home page — only active banners, ordered by position
        Task<List<Banner>> GetActiveOrderedAsync();
    }
}
```

---

### 8.14 INotificationRepository

```csharp
// ─── Application/Interfaces/Repositories/INotificationRepository.cs ──────────
// Methods required by: §1.13 notification endpoints

namespace Application.Interfaces.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        // Required by: GET /notifications — paginated, optional unread filter
        Task<(List<Notification> Notifications, int Total)> GetByUserPagedAsync(
            Guid  userId,
            bool? read,
            int   page,
            int   limit);

        // Required by: PUT /notifications/read-all
        Task MarkAllAsReadAsync(Guid userId);
    }
}
```

---

## 9. Unit of Work Interface

```csharp
// ─── Application/Interfaces/I
.cs ───────────────────────────────────
// Coordinates all repository transactions.
// All repositories injected here to ensure single DbContext per request.

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository            Users              { get; }
        IAddressRepository         Addresses          { get; }
        ICategoryRepository        Categories         { get; }
        IProductRepository         Products           { get; }
        IProductImageRepository    ProductImages      { get; }
        ISellerProfileRepository   SellerProfiles     { get; }
        ICartRepository            Carts              { get; }
        ICartItemRepository        CartItems          { get; }
        IOrderRepository           Orders             { get; }
        IReviewRepository          Reviews            { get; }
        IWishlistRepository        WishlistItems      { get; }
        IPromoCodeRepository       PromoCodes         { get; }
        IBannerRepository          Banners            { get; }
        INotificationRepository    Notifications      { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

---

## 10. Conflicts & Missing Definitions

| # | Area | Detail |
|---|---|---|
| 1 | `SalesByDayDto` | API Contract mentions `salesByDay` in dashboard response but defines no field names or grouping interval. **NOT DEFINED IN SRS/API CONTRACT** — used `Date` + `Amount` as minimal placeholder. |
| 2 | `SellerEarnings.Transactions` | API Contract lists `transactions` array in earnings response but defines no transaction sub-schema. **NOT DEFINED IN SRS/API CONTRACT** — typed as `List<object>` until defined. |
| 3 | `Notification.Type` | API Contract references notification types (order updates, promotions) but provides no enum values. **NOT DEFINED IN SRS/API CONTRACT** — stored as `string`. |
| 4 | `BannerPosition` | API Contract includes a `position` field on banners but specifies no allowed values or ordering strategy. **NOT DEFINED IN SRS/API CONTRACT** — treated as `int`. |
| 5 | `Review.Images` | API Contract includes `images: []` in review response but the POST request does not specify file upload or URL input. **NOT DEFINED IN SRS/API CONTRACT** — treated as `List<string>` (URL strings). |
| 6 | `Order.GuestEmail` | API Contract allows guest checkout but does not explicitly state how guest identification is captured post-order. Inferred from SRS §3 guest checkout + API Contract address flow. |
| 7 | `PromoCode.ApplicableCategoriesJson` | API Contract sends `applicableCategories` as an array of UUIDs. No join table is defined in either document. **NOT DEFINED IN SRS/API CONTRACT** — stored as JSON string. |
| 8 | `SellerEarnings` | `paidOut` and `pendingPayout` are in the API Contract response but no transaction or payout entity is defined anywhere in SRS or API Contract. Reported as gap. |
| 9 | `Wallet` | Wallet entity/table is implied by `POST /payments/wallet/topup` and `GET /payments/wallet/balance` but no `Wallet` domain entity is defined in the SRS or API Contract schema. **NOT DEFINED IN SRS/API CONTRACT** — wallet entity design deferred. |
| 10 | `Order EstimatedDelivery` | Returned in API response but no calculation rule or setter is defined. **NOT DEFINED IN SRS/API CONTRACT** — stored as nullable `DateTime`; source/calculation rule not defined. |
