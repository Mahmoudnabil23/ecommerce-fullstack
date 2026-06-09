# 🛒 E-Commerce MEAN Stack — Full Technical Architecture Breakdown

> **Stack:** Backend → ASP.NET Core Web API | Frontend → Angular + Angular Material  
> **Auth:** JWT (Access + Refresh Tokens) | **Roles:** Customer · Seller · Admin  
> **Prepared by:** Senior Software Architect Analysis

---

## Table of Contents
1. [API Contract (Backend)](#1-api-contract-backend)
   - 1.1 Auth & User Management
   - 1.2 Profile Management
   - 1.3 Product Management
   - 1.4 Category Management
   - 1.5 Shopping Cart
   - 1.6 Order Management
   - 1.7 Payment Integration
   - 1.8 Reviews & Ratings
   - 1.9 Wishlist
   - 1.10 Admin Panel
   - 1.11 Seller Management
   - 1.12 Promo Codes & Discounts (Bonus)
   - 1.13 Notifications (Bonus)
2. [Frontend Pages & Features (Angular)](#2-frontend-pages--features-angular)
3. [Feature Mapping](#3-feature-mapping)
4. [Assumptions & Gaps](#4-assumptions--gaps)

---

# 1. API Contract (Backend)

> **Base URL:** `https://api.yourstore.com/api/v1`  
> **Auth Header (where required):** `Authorization: Bearer <jwt_token>`  
> **Standard Error Envelope:**
> ```json
> { "success": false, "message": "Human-readable error", "errors": ["field-level errors"] }
> ```
> **Standard Success Envelope:**
> ```json
> { "success": true, "message": "...", "data": { ... } }
> ```

---

## 1.1 Auth & User Management

### POST `/auth/register`
- **Description:** Register a new user (Customer or Seller).
- **Auth:** None
- **Request Body:**
```json
{
  "fullName": "Ahmed Ali",
  "email": "ahmed@example.com",
  "phone": "+201001234567",
  "password": "P@ssw0rd!",
  "role": "Customer"
}
```
- **Success `201`:**
```json
{
  "success": true,
  "message": "Registration successful. Please verify your email.",
  "data": { "userId": "uuid-123" }
}
```
- **Errors:**
  - `400` — Validation error (weak password, invalid email)
  - `409` — Email or phone already exists
- **Notes:**
  - Triggers confirmation email with a verification token (expiry: 24h).
  - Password: min 8 chars, 1 uppercase, 1 number, 1 special char.
  - Role defaults to `Customer` if not provided.

---

### POST `/auth/login`
- **Description:** Authenticate user, return JWT access + refresh tokens.
- **Auth:** None
- **Request Body:**
```json
{ "email": "ahmed@example.com", "password": "P@ssw0rd!" }
```
- **Success `200`:**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJ...",
    "refreshToken": "dGhp...",
    "expiresIn": 3600,
    "user": { "id": "uuid-123", "fullName": "Ahmed Ali", "role": "Customer" }
  }
}
```
- **Errors:**
  - `401` — Invalid credentials
  - `403` — Email not verified / account suspended
- **Notes:** Access token TTL = 1h; Refresh token TTL = 7d. Refresh token stored in HttpOnly cookie (recommended) or returned in body.

---

### POST `/auth/refresh-token`
- **Description:** Issue a new access token using a refresh token.
- **Request Body:** `{ "refreshToken": "dGhp..." }`
- **Success `200`:** Returns new `accessToken` + `refreshToken`.
- **Errors:** `401` — Invalid/expired refresh token

---

### POST `/auth/logout`
- **Description:** Revoke refresh token.
- **Auth:** Bearer token
- **Request Body:** `{ "refreshToken": "dGhp..." }`
- **Success `200`:** `{ "success": true, "message": "Logged out successfully" }`

---

### GET `/auth/verify-email?user-id={userId}&token={token}`
- **Description:** Verify email from the link sent during registration.
- **Auth:** None
- **Success `200`:** `{ "success": true, "message": "Email verified successfully" }`
- **Errors:** `400` — Token expired or invalid

---

### POST `/auth/forgot-password`
- **Request Body:** `{ "email": "ahmed@example.com" }`
- **Success `200`:** `{ "success": true, "message": "Password reset link sent to your email" }`
- **Notes:** Reset token expires in 1h.

---

### POST `/auth/reset-password`
- **Request Body:** `{ "token": "...", "newPassword": "NewP@ss1!" }`
- **Success `200`:** Password changed, all existing sessions revoked.
- **Errors:** `400` — Token expired/invalid | `422` — Password does not meet policy

---

### POST `/auth/google` *(Bonus)*
- **Description:** OAuth2 login/register via Google.
- **Request Body:** `{ "idToken": "google-id-token" }`
- **Success `200`:** Same as `/auth/login` success response.

---

### POST `/auth/change-password`
- **Auth:** Bearer token (Any authenticated user)
- **Request Body:** `{ "currentPassword": "...", "newPassword": "..." }`
- **Success `200`:** Password updated.
- **Errors:** `401` — Wrong current password

---

## 1.2 Profile Management

### GET `/users/me`
- **Description:** Get current authenticated user's profile.
- **Auth:** Bearer token (Customer / Seller / Admin)
- **Success `200`:**
```json
{
  "success": true,
  "data": {
    "id": "uuid-123",
    "fullName": "Ahmed Ali",
    "email": "ahmed@example.com",
    "phone": "+201001234567",
    "role": "Customer",
    "avatar": "https://cdn.yourstore.com/avatars/uuid-123.jpg",
    "addresses": [
      { "id": "addr-1", "label": "Home", "street": "123 Tahrir St", "city": "Cairo", "isDefault": true }
    ],
    "paymentDetails": { "savedCards": [] },
    "createdAt": "2024-01-15T10:00:00Z"
  }
}
```

---

### PUT `/users/me`
- **Description:** Update profile (name, phone, avatar).
- **Auth:** Bearer token (Any user)
- **Request Body:**
```json
{ "fullName": "Ahmed Mohamed Ali", "phone": "+201009876543" }
```
- **Success `200`:** Updated user object.
- **Errors:** `409` — Phone already in use

---

### POST `/users/me/avatar`
- **Description:** Upload profile avatar.
- **Auth:** Bearer token
- **Content-Type:** `multipart/form-data`
- **Body:** `file` (image, max 2MB, formats: jpg/png/webp)
- **Success `200`:** `{ "avatarUrl": "https://cdn.yourstore.com/..." }`

---

### POST `/users/me/addresses`
- **Description:** Add a new address.
- **Auth:** Bearer token (Customer)
- **Request Body:**
```json
{ "label": "Work", "street": "456 Nile Ave", "city": "Cairo", "postalCode": "11511", "isDefault": false }
```
- **Success `201`:** Created address object.

---

### PUT `/users/me/addresses/{addressId}`
- **Description:** Update an existing address.
- **Auth:** Bearer token
- **Success `200`:** Updated address.
- **Errors:** `404` — Address not found

---

### DELETE `/users/me/addresses/{addressId}`
- **Auth:** Bearer token
- **Success `204`:** No content.

---

### GET `/users/me/orders`
- **Description:** Get current user's order history.
- **Auth:** Bearer token (Customer)
- **Query Params:** `page=1&limit=10&status=delivered`
- **Success `200`:** Paginated list of orders (see Order schema in §1.6).

---

## 1.3 Product Management

### GET `/products`
- **Description:** List all products with search, filter, and pagination.
- **Auth:** None
- **Query Params:**

| Param | Type | Description |
|---|---|---|
| `search` | string | Search by name/description |
| `categoryId` | UUID | Filter by category |
| `minPrice` | number | Min price filter |
| `maxPrice` | number | Max price filter |
| `sellerId` | UUID | Filter by seller |
| `inStock` | boolean | Only in-stock items |
| `sortBy` | string | `price_asc`, `price_desc`, `newest`, `rating` |
| `page` | int | Default 1 |
| `limit` | int | Default 20, max 100 |

- **Success `200`:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "prod-uuid",
        "name": "Wireless Mouse",
        "slug": "wireless-mouse",
        "price": 299.99,
        "discountedPrice": 249.99,
        "images": ["https://cdn.../img1.jpg"],
        "category": { "id": "cat-1", "name": "Electronics" },
        "seller": { "id": "sel-1", "storeName": "TechHub" },
        "stock": 45,
        "averageRating": 4.3,
        "reviewCount": 128
      }
    ],
    "pagination": { "total": 200, "page": 1, "limit": 20, "totalPages": 10 }
  }
}
```

---

### GET `/products/{id}`
- **Description:** Get a single product's full details.
- **Auth:** None
- **Success `200`:** Full product object including all images, description, specs, reviews summary.
- **Errors:** `404` — Product not found

---

### POST `/products`
- **Description:** Create a new product listing.
- **Auth:** Bearer token (Seller / Admin)
- **Content-Type:** `multipart/form-data`
- **Body Fields:**

| Field | Type | Required |
|---|---|---|
| `name` | string | ✅ |
| `description` | string | ✅ |
| `price` | number | ✅ |
| `categoryId` | UUID | ✅ |
| `stock` | int | ✅ |
| `images` | file[] | ✅ min 1, max 8 |
| `specs` | JSON string | ❌ |

- **Success `201`:** Created product object.
- **Errors:** `400` — Validation | `403` — Not a seller

---

### PUT `/products/{id}`
- **Description:** Update product details.
- **Auth:** Bearer token (Seller who owns it / Admin)
- **Request Body:** Any subset of product fields.
- **Success `200`:** Updated product.
- **Errors:** `403` — Not owner | `404` — Not found

---

### DELETE `/products/{id}`
- **Description:** Soft-delete a product.
- **Auth:** Bearer token (Seller who owns it / Admin)
- **Success `204`:** No content.
- **Notes:** Product becomes invisible to customers but remains in past orders.

---

### POST `/products/{id}/images`
- **Description:** Add images to an existing product.
- **Auth:** Bearer token (Seller / Admin)
- **Content-Type:** `multipart/form-data`
- **Success `200`:** Updated images array.

---

### DELETE `/products/{id}/images/{imageId}`
- **Auth:** Bearer token (Seller / Admin)
- **Success `204`:** No content.

---

## 1.4 Category Management

### GET `/categories`
- **Description:** Get all categories (tree structure).
- **Auth:** None
- **Success `200`:**
```json
{
  "data": [
    { "id": "cat-1", "name": "Electronics", "slug": "electronics", "imageUrl": "...",
      "children": [
        { "id": "cat-1-1", "name": "Phones", "slug": "phones" }
      ]
    }
  ]
}
```

---

### GET `/categories/{id}/products`
- **Description:** Get all products in a category (supports sub-categories).
- **Auth:** None
- **Query Params:** Same filters as `GET /products`
- **Success `200`:** Paginated product list.

---

### POST `/categories`
- **Auth:** Bearer token (Admin only)
- **Request Body:** `{ "name": "Clothing", "parentId": null, "imageUrl": "..." }`
- **Success `201`:** Created category.

---

### PUT `/categories/{id}`
- **Auth:** Bearer token (Admin only)
- **Success `200`:** Updated category.

---

### DELETE `/categories/{id}`
- **Auth:** Bearer token (Admin only)
- **Success `204`:** Soft delete. Products in this category are uncategorized.

---

## 1.5 Shopping Cart

### GET `/cart`
- **Description:** Get the current user's cart (or guest cart by session).
- **Auth:** Bearer token (optional — guest carts use session ID header)
- **Headers:** `X-Guest-Session: {uuid}` (for guest checkout)
- **Success `200`:**
```json
{
  "data": {
    "cartId": "cart-uuid",
    "items": [
      {
        "cartItemId": "item-1",
        "product": { "id": "prod-1", "name": "Wireless Mouse", "price": 299.99, "imageUrl": "..." },
        "quantity": 2,
        "subtotal": 599.98
      }
    ],
    "summary": {
      "subtotal": 599.98,
      "discount": 0,
      "shipping": 50.00,
      "total": 649.98
    },
    "appliedPromoCode": null
  }
}
```

---

### POST `/cart/items`
- **Description:** Add a product to the cart.
- **Auth:** Optional (Bearer or Guest session)
- **Request Body:** `{ "productId": "prod-1", "quantity": 2 }`
- **Success `200`:** Updated cart.
- **Errors:** `400` — Insufficient stock | `404` — Product not found

---

### PUT `/cart/items/{cartItemId}`
- **Description:** Update quantity of a cart item.
- **Request Body:** `{ "quantity": 3 }`
- **Success `200`:** Updated cart.
- **Errors:** `400` — Quantity exceeds stock | `404` — Item not in cart

---

### DELETE `/cart/items/{cartItemId}`
- **Description:** Remove an item from cart.
- **Success `204`:** Item removed, returns updated cart summary.

---

### DELETE `/cart`
- **Description:** Clear entire cart.
- **Success `204`:** Cart emptied.

---

### POST `/cart/promo`
- **Description:** Apply a promo code to the cart.
- **Auth:** Required
- **Request Body:** `{ "code": "SAVE20" }`
- **Success `200`:** Updated cart summary with discount applied.
- **Errors:** `400` — Invalid/expired code | `409` — Code already applied

---

### DELETE `/cart/promo`
- **Description:** Remove applied promo code.
- **Success `200`:** Cart summary without discount.

---

### POST `/cart/merge`
- **Description:** Merge guest cart into user cart on login.
- **Auth:** Bearer token
- **Request Body:** `{ "guestSessionId": "uuid" }`
- **Success `200`:** Merged cart.

---

## 1.6 Order Management

### POST `/orders`
- **Description:** Place a new order from the current cart.
- **Auth:** Bearer token (Customer) or Guest
- **Request Body:**
```json
{
  "addressId": "addr-1",
  "paymentMethod": "stripe",
  "paymentToken": "tok_visa_...",
  "promoCode": "SAVE20",
  "notes": "Please leave at door"
}
```
- **Success `201`:**
```json
{
  "data": {
    "orderId": "ord-uuid",
    "orderNumber": "ORD-20240115-00042",
    "status": "pending",
    "total": 649.98,
    "paymentStatus": "paid",
    "estimatedDelivery": "2024-01-20"
  }
}
```
- **Errors:** `400` — Cart empty / insufficient stock | `402` — Payment failed | `422` — Invalid address

---

### GET `/orders`
- **Description:** Get list of all orders (Admin) or own orders (Customer/Seller).
- **Auth:** Bearer token
- **Query Params:** `page`, `limit`, `status`, `startDate`, `endDate`
- **Notes:** Admin sees all orders; Customer sees their own; Seller sees orders containing their products.

---

### GET `/orders/{id}`
- **Description:** Get full order details.
- **Auth:** Bearer token (Order owner / Admin)
- **Success `200`:**
```json
{
  "data": {
    "orderId": "ord-uuid",
    "orderNumber": "ORD-20240115-00042",
    "status": "shipped",
    "items": [
      { "productId": "prod-1", "name": "Wireless Mouse", "qty": 2, "unitPrice": 299.99 }
    ],
    "address": { "street": "123 Tahrir", "city": "Cairo" },
    "paymentMethod": "stripe",
    "paymentStatus": "paid",
    "trackingNumber": "TRK123456",
    "statusHistory": [
      { "status": "pending", "timestamp": "2024-01-15T10:00:00Z" },
      { "status": "confirmed", "timestamp": "2024-01-15T10:30:00Z" },
      { "status": "shipped", "timestamp": "2024-01-16T08:00:00Z" }
    ],
    "total": 649.98
  }
}
```

---

### PUT `/orders/{id}/status`
- **Description:** Update order status.
- **Auth:** Bearer token (Admin / Seller for their items)
- **Request Body:** `{ "status": "shipped", "trackingNumber": "TRK123456" }`
- **Status Flow:** `pending → confirmed → processing → shipped → delivered → completed`
- **Cancellation:** `pending/confirmed → cancelled`
- **Success `200`:** Updated order. Triggers email notification to customer.

---

### POST `/orders/{id}/cancel`
- **Description:** Customer cancels their own order.
- **Auth:** Bearer token (Customer — order owner)
- **Request Body:** `{ "reason": "Changed my mind" }`
- **Errors:** `400` — Order cannot be cancelled at this stage (already shipped)

---

### GET `/orders/{id}/invoice`
- **Description:** Download order invoice as PDF.
- **Auth:** Bearer token (Order owner / Admin)
- **Response:** `application/pdf`

---

## 1.7 Payment Integration

### POST `/payments/initiate`
- **Description:** Create a payment intent (Stripe) or initiate PayPal session.
- **Auth:** Bearer token
- **Request Body:**
```json
{
  "orderId": "ord-uuid",
  "paymentMethod": "stripe",
  "currency": "EGP"
}
```
- **Success `200`:**
```json
{
  "data": {
    "clientSecret": "pi_xxx_secret_xxx",
    "paymentIntentId": "pi_xxx"
  }
}
```

---

### POST `/payments/webhook`
- **Description:** Stripe/PayPal webhook for payment confirmation.
- **Auth:** Webhook signature (not Bearer)
- **Notes:** Verifies signature, updates order payment status, triggers confirmation email. This endpoint is called by the payment gateway, not the frontend.

---

### POST `/payments/wallet/topup` *(Bonus)*
- **Description:** Add funds to the user's in-app wallet.
- **Auth:** Bearer token (Customer)
- **Request Body:** `{ "amount": 500, "paymentMethod": "stripe", "paymentToken": "tok_..." }`

---

### GET `/payments/wallet/balance` *(Bonus)*
- **Auth:** Bearer token
- **Success `200`:** `{ "balance": 1250.00, "currency": "EGP" }`

---

## 1.8 Reviews & Ratings

### GET `/products/{productId}/reviews`
- **Auth:** None
- **Query Params:** `page`, `limit`, `rating` (filter by 1-5 stars)
- **Success `200`:**
```json
{
  "data": {
    "averageRating": 4.3,
    "totalReviews": 128,
    "distribution": { "5": 60, "4": 40, "3": 15, "2": 8, "1": 5 },
    "reviews": [
      {
        "id": "rev-1",
        "user": { "fullName": "Sara M.", "avatar": "..." },
        "rating": 5,
        "title": "Great product!",
        "body": "Works perfectly, fast delivery.",
        "images": [],
        "createdAt": "2024-01-10T00:00:00Z",
        "isVerifiedPurchase": true
      }
    ]
  }
}
```

---

### POST `/products/{productId}/reviews`
- **Auth:** Bearer token (Customer who purchased the product)
- **Request Body:**
```json
{
  "rating": 5,
  "title": "Excellent!",
  "body": "Very happy with this purchase.",
  "images": []
}
```
- **Success `201`:** Created review.
- **Errors:** `403` — User has not purchased this product | `409` — Review already submitted

---

### PUT `/products/{productId}/reviews/{reviewId}`
- **Auth:** Bearer token (Review author)
- **Success `200`:** Updated review.

---

### DELETE `/products/{productId}/reviews/{reviewId}`
- **Auth:** Bearer token (Review author / Admin)
- **Success `204`:** Review deleted.

---

## 1.9 Wishlist & Favorites

### GET `/wishlist`
- **Auth:** Bearer token (Customer)
- **Success `200`:** List of wishlisted products.

---

### POST `/wishlist`
- **Request Body:** `{ "productId": "prod-1" }`
- **Success `201`:** Product added to wishlist.
- **Errors:** `409` — Already in wishlist

---

### DELETE `/wishlist/{productId}`
- **Success `204`:** Removed from wishlist.

---

### POST `/wishlist/{productId}/move-to-cart`
- **Description:** Move wishlist item to cart.
- **Success `200`:** Item moved; updated cart returned.

---

## 1.10 Admin Panel

### GET `/admin/users`
- **Auth:** Bearer token (Admin only)
- **Query Params:** `page`, `limit`, `role`, `status`, `search`
- **Success `200`:** Paginated user list with roles and statuses.

---

### GET `/admin/users/{id}`
- **Auth:** Admin
- **Success `200`:** Full user profile including orders and activity.

---

### PUT `/admin/users/{id}/status`
- **Description:** Approve, suspend, or activate a user.
- **Request Body:** `{ "status": "suspended", "reason": "Policy violation" }`
- **Status values:** `active`, `suspended`, `banned`
- **Success `200`:** Updated user status.

---

### DELETE `/admin/users/{id}`
- **Description:** Soft-delete a user account.
- **Success `204`:** User anonymized; their orders retained.

---

### GET `/admin/dashboard`
- **Auth:** Admin
- **Success `200`:**
```json
{
  "data": {
    "totalRevenue": 125000.00,
    "totalOrders": 842,
    "totalUsers": 1540,
    "totalProducts": 320,
    "recentOrders": [...],
    "topProducts": [...],
    "salesByDay": [...]
  }
}
```

---

### GET `/admin/orders`
- **Auth:** Admin
- **Query Params:** `page`, `limit`, `status`, `startDate`, `endDate`
- **Success `200`:** All orders with seller and customer info.

---

### POST `/admin/categories`
- Covered in §1.4

---

### GET `/admin/sellers`
- **Auth:** Admin
- **Success `200`:** Paginated seller list with verification status.

---

### PUT `/admin/sellers/{id}/verify`
- **Request Body:** `{ "verified": true }`
- **Success `200`:** Seller verification status updated.

---

### GET `/admin/promo-codes` *(Bonus)*
- **Auth:** Admin
- **Success `200`:** All promo codes with usage stats.

---

### POST `/admin/promo-codes` *(Bonus)*
- **Request Body:**
```json
{
  "code": "SUMMER25",
  "type": "percentage",
  "value": 25,
  "minOrderAmount": 200,
  "usageLimit": 500,
  "expiresAt": "2024-08-31T23:59:59Z",
  "applicableCategories": ["cat-1"]
}
```
- **Success `201`:** Created promo code.

---

### PUT `/admin/promo-codes/{id}`
- **Success `200`:** Updated promo code.

---

### DELETE `/admin/promo-codes/{id}`
- **Success `204`:** Deactivated.

---

### GET `/admin/banners`
- **Auth:** Admin
- **Success `200`:** List of homepage banners.

---

### POST `/admin/banners`
- **Content-Type:** `multipart/form-data`
- **Body:** `title`, `imageFile`, `linkUrl`, `position`, `isActive`
- **Success `201`:** Created banner.

---

### PUT `/admin/banners/{id}`
- **Success `200`:** Updated banner.

---

### DELETE `/admin/banners/{id}`
- **Success `204`:** Removed.

---

## 1.11 Seller Management

### POST `/sellers/register`
- **Description:** Apply for a seller account (separate from user registration).
- **Auth:** Bearer token (existing Customer)
- **Request Body:**
```json
{
  "storeName": "TechHub Egypt",
  "storeDescription": "Premium electronics at best prices",
  "taxId": "123456789",
  "bankAccount": {
    "bankName": "CIB",
    "accountNumber": "...",
    "accountHolderName": "Ahmed Ali"
  }
}
```
- **Success `201`:** Seller application submitted. Status = `pending`.

---

### GET `/sellers/me`
- **Auth:** Bearer token (Seller)
- **Success `200`:** Seller profile with store info, rating, payout info.

---

### PUT `/sellers/me`
- **Auth:** Bearer token (Seller)
- **Success `200`:** Updated seller profile.

---

### GET `/sellers/me/products`
- **Auth:** Bearer token (Seller)
- **Query Params:** `page`, `limit`, `status`
- **Success `200`:** Paginated list of seller's own products.

---

### GET `/sellers/me/orders`
- **Auth:** Bearer token (Seller)
- **Success `200`:** Orders containing seller's products.

---

### GET `/sellers/me/earnings` *(Bonus)*
- **Auth:** Bearer token (Seller)
- **Query Params:** `startDate`, `endDate`
- **Success `200`:**
```json
{
  "data": {
    "totalEarnings": 15000.00,
    "pendingPayout": 3500.00,
    "paidOut": 11500.00,
    "transactions": [...]
  }
}
```

---

### GET `/sellers/{id}` (Public Store Page)
- **Auth:** None
- **Success `200`:** Public seller profile (store name, rating, active products).

---

## 1.12 Promo Codes & Discounts *(Bonus)*

> Admin endpoints are in §1.10. The customer-facing endpoint is:

### POST `/cart/promo`
Already defined in §1.5.

---

## 1.13 Notifications *(Bonus)*

### GET `/notifications`
- **Auth:** Bearer token
- **Query Params:** `page`, `limit`, `read=false`
- **Success `200`:** List of notifications (order updates, promotions).

---

### PUT `/notifications/{id}/read`
- **Success `200`:** Notification marked as read.

---

### PUT `/notifications/read-all`
- **Success `200`:** All notifications marked as read.

---

### POST `/notifications/subscribe` *(Push - Bonus)*
- **Request Body:** `{ "endpoint": "...", "keys": { "p256dh": "...", "auth": "..." } }`
- **Success `201`:** Push subscription saved.

---

# 2. Frontend Pages & Features (Angular)

## Angular Module Structure

```
src/
├── app/
│   ├── core/                    # Singleton services, guards, interceptors
│   │   ├── auth/
│   │   ├── guards/
│   │   ├── interceptors/
│   │   └── services/
│   ├── shared/                  # Reusable components, pipes, directives
│   │   ├── components/
│   │   │   ├── navbar/
│   │   │   ├── footer/
│   │   │   ├── product-card/
│   │   │   ├── rating-stars/
│   │   │   ├── breadcrumb/
│   │   │   ├── pagination/
│   │   │   ├── loading-spinner/
│   │   │   ├── image-gallery/
│   │   │   └── confirm-dialog/
│   │   ├── pipes/
│   │   └── directives/
│   ├── features/
│   │   ├── auth/                # Login, Register, Reset Password
│   │   ├── home/                # Landing page
│   │   ├── products/            # Product listing, detail
│   │   ├── cart/                # Cart page
│   │   ├── checkout/            # Checkout flow
│   │   ├── orders/              # Order history, tracking
│   │   ├── profile/             # User profile management
│   │   ├── wishlist/
│   │   ├── seller/              # Seller dashboard module
│   │   └── admin/               # Admin dashboard module
│   └── app-routing.module.ts
```

---

## Shared Components

| Component | Purpose | Angular Material Used |
|---|---|---|
| `NavbarComponent` | Top nav: logo, search, cart icon, user menu | `MatToolbar`, `MatMenu`, `MatBadge` |
| `FooterComponent` | Links, social icons, newsletter signup | Custom |
| `ProductCardComponent` | Reusable card: image, name, price, rating, wishlist btn | `MatCard`, `MatButton` |
| `RatingStarsComponent` | Display/input star rating | Custom SVG |
| `BreadcrumbComponent` | Navigation path | `MatChips` |
| `PaginationComponent` | Page controls | `MatPaginator` |
| `LoadingSpinnerComponent` | Full-page / inline loader | `MatProgressSpinner` |
| `ImageGalleryComponent` | Zoomable product image carousel | Custom / CDK |
| `ConfirmDialogComponent` | Reusable yes/no dialog | `MatDialog` |
| `EmptyStateComponent` | "No results" placeholder | Custom |
| `PriceDisplayComponent` | Shows original + discounted price | Custom |
| `AddressCardComponent` | Displays a saved address | `MatCard` |

---

## 2.1 Home Page

| | |
|---|---|
| **Route** | `/` |
| **Purpose** | Landing page with featured products, banners, categories |
| **Components** | Hero carousel (banners), category quick-links, featured products grid, deals section |
| **User Actions** | Browse categories, search, add to cart/wishlist from card |
| **APIs** | `GET /categories`, `GET /products?sortBy=newest`, `GET /admin/banners` |
| **State** | `banners[]`, `categories[]`, `featuredProducts[]` |
| **Angular Material** | `MatCarousel` (3rd party or custom), `MatGridList`, `MatCard` |

---

## 2.2 Auth Pages

### Register Page
| | |
|---|---|
| **Route** | `/auth/register` |
| **Purpose** | New user registration |
| **Components** | Reactive form, role selector (Customer/Seller), Google OAuth button |
| **Validation** | Email format, min password strength, phone format (+20...), confirm password match |
| **APIs** | `POST /auth/register`, `POST /auth/google` |
| **Angular Material** | `MatFormField`, `MatInput`, `MatSelect`, `MatStepperModule` |

### Login Page
| | |
|---|---|
| **Route** | `/auth/login` |
| **Purpose** | User authentication |
| **Components** | Email/password form, "Forgot password" link, Google login |
| **Validation** | Required fields, email format |
| **APIs** | `POST /auth/login`, `POST /auth/google` |
| **Post-login redirect** | Previous URL or home |

### Forgot Password Page
| | |
|---|---|
| **Route** | `/auth/forgot-password` |
| **APIs** | `POST /auth/forgot-password` |

### Reset Password Page
| | |
|---|---|
| **Route** | `/auth/reset-password?token={token}` |
| **APIs** | `POST /auth/reset-password` |

### Email Verification Page
| | |
|---|---|
| **Route** | `/auth/verify-email?token={token}` |
| **Purpose** | Auto-submit token on load, show success/fail message |
| **APIs** | `GET /auth/verify-email` |

---

## 2.3 Product Listing Page

| | |
|---|---|
| **Route** | `/products` |
| **Purpose** | Browse and search all products |
| **Components** | Search bar, filter sidebar, product grid, sorting dropdown, pagination |
| **Filter Sidebar** | Category tree, price range slider, in-stock toggle, min rating |
| **User Actions** | Search, filter, sort, add to cart, add to wishlist, click product |
| **APIs** | `GET /products`, `GET /categories` |
| **State** | `products[]`, `filters{}`, `pagination{}`, `isLoading` |
| **Angular Material** | `MatSidenav`, `MatSlider`, `MatCheckbox`, `MatSelect`, `MatChips` |
| **Validation** | minPrice ≤ maxPrice |

---

## 2.4 Product Detail Page

| | |
|---|---|
| **Route** | `/products/:id/:slug` |
| **Purpose** | Full product view with images, specs, reviews |
| **Components** | `ImageGalleryComponent`, quantity selector, add-to-cart button, review list, write review form, seller info card |
| **User Actions** | View images, select quantity, add to cart, add to wishlist, read/write reviews |
| **APIs** | `GET /products/{id}`, `GET /products/{id}/reviews`, `POST /cart/items`, `POST /wishlist`, `POST /products/{id}/reviews` |
| **State** | `product{}`, `reviews[]`, `selectedQty`, `isInWishlist` |
| **Angular Material** | `MatTabs` (Details / Reviews / Seller), `MatSnackBar` |

---

## 2.5 Shopping Cart Page

| | |
|---|---|
| **Route** | `/cart` |
| **Purpose** | Review and manage cart items before checkout |
| **Components** | Cart items list, quantity adjusters, remove buttons, order summary panel, promo code input, proceed to checkout button |
| **User Actions** | Update quantity, remove item, apply/remove promo code, proceed to checkout |
| **APIs** | `GET /cart`, `PUT /cart/items/{id}`, `DELETE /cart/items/{id}`, `POST /cart/promo`, `DELETE /cart/promo` |
| **State** | `cart{}`, `isLoading`, `promoError` |
| **Angular Material** | `MatTable`, `MatInput`, `MatButton`, `MatProgressBar` |
| **Notes** | Show "Cart is empty" state with CTA button |

---

## 2.6 Checkout Page

| | |
|---|---|
| **Route** | `/checkout` |
| **Purpose** | Multi-step checkout: address → payment → review → confirm |
| **Components** | `MatStepper` with steps: 1) Address, 2) Payment, 3) Review & Place Order |
| **Step 1 — Address** | Select saved address or add new; guest email field |
| **Step 2 — Payment** | Payment method tabs: Credit Card (Stripe Elements), PayPal, Cash on Delivery, Wallet |
| **Step 3 — Review** | Order summary, final price, place order button |
| **User Actions** | Select/add address, select payment, apply promo, place order |
| **APIs** | `GET /users/me` (addresses), `POST /users/me/addresses`, `GET /cart`, `POST /payments/initiate`, `POST /orders` |
| **State** | `selectedAddress`, `paymentMethod`, `orderSummary`, `isPlacing` |
| **Angular Material** | `MatStepper`, `MatRadioGroup`, `MatFormField`, `MatDialog` |
| **Guard** | `AuthGuard` (or allow guest with session ID) |

---

## 2.7 Order Confirmation Page

| | |
|---|---|
| **Route** | `/orders/confirmation/:orderId` |
| **Purpose** | Show success screen after placing order |
| **Components** | Order number, summary, estimated delivery, "Continue Shopping" CTA |
| **APIs** | `GET /orders/{id}` |

---

## 2.8 Order History Page

| | |
|---|---|
| **Route** | `/profile/orders` |
| **Purpose** | List of all customer orders with status badges |
| **Components** | Order list table, status chips, filter by status, date range picker |
| **User Actions** | View order details, cancel order, download invoice |
| **APIs** | `GET /users/me/orders` |
| **Angular Material** | `MatTable`, `MatChip`, `MatDatepicker` |

---

## 2.9 Order Detail & Tracking Page

| | |
|---|---|
| **Route** | `/profile/orders/:id` |
| **Purpose** | Full order details and live status tracking |
| **Components** | Status stepper, items list, address card, invoice download button, cancel button |
| **APIs** | `GET /orders/{id}`, `POST /orders/{id}/cancel`, `GET /orders/{id}/invoice` |
| **Angular Material** | `MatStepper` (horizontal, read-only), `MatExpansionPanel` |

---

## 2.10 User Profile Page

| | |
|---|---|
| **Route** | `/profile` |
| **Purpose** | View and edit personal info |
| **Components** | Avatar uploader, name/phone form, change password section |
| **User Actions** | Edit name/phone, upload avatar, change password |
| **APIs** | `GET /users/me`, `PUT /users/me`, `POST /users/me/avatar`, `POST /auth/change-password` |
| **Angular Material** | `MatCard`, `MatFormField`, `MatDivider` |

---

## 2.11 Address Book Page

| | |
|---|---|
| **Route** | `/profile/addresses` |
| **Purpose** | Manage saved addresses |
| **Components** | Address cards grid, add/edit address dialog |
| **User Actions** | Add, edit, delete, set default address |
| **APIs** | `GET /users/me`, `POST /users/me/addresses`, `PUT /users/me/addresses/{id}`, `DELETE /users/me/addresses/{id}` |
| **Angular Material** | `MatCard`, `MatDialog`, `MatFormField` |

---

## 2.12 Wishlist Page

| | |
|---|---|
| **Route** | `/wishlist` |
| **Purpose** | View and manage saved products |
| **Components** | Product cards grid with remove + move-to-cart actions |
| **APIs** | `GET /wishlist`, `DELETE /wishlist/{productId}`, `POST /wishlist/{productId}/move-to-cart` |

---

## 2.13 Seller Dashboard

> Protected by `SellerGuard`. Lazy-loaded `SellerModule`.

### Seller Overview Page
| | |
|---|---|
| **Route** | `/seller/dashboard` |
| **Components** | Stats cards (revenue, orders, products), recent orders table, low stock alerts |
| **APIs** | `GET /sellers/me`, `GET /sellers/me/orders`, `GET /sellers/me/earnings` |
| **Angular Material** | `MatCard`, `MatTable`, `MatBadge` |

### Seller Products Page
| | |
|---|---|
| **Route** | `/seller/products` |
| **User Actions** | View, add, edit, delete products |
| **APIs** | `GET /sellers/me/products`, `POST /products`, `PUT /products/{id}`, `DELETE /products/{id}` |

### Seller Add/Edit Product Page
| | |
|---|---|
| **Route** | `/seller/products/new`, `/seller/products/:id/edit` |
| **Components** | Multi-step form: details, images (drag-drop upload), pricing, stock |
| **Validation** | Name required, price > 0, stock ≥ 0, min 1 image |
| **APIs** | `POST /products`, `PUT /products/{id}`, `POST /products/{id}/images` |
| **Angular Material** | `MatStepper`, `MatChips` (tags), CDK DragDrop (image order) |

### Seller Orders Page
| | |
|---|---|
| **Route** | `/seller/orders` |
| **User Actions** | View order details, update status (mark as shipped) |
| **APIs** | `GET /sellers/me/orders`, `PUT /orders/{id}/status` |

### Seller Earnings Page *(Bonus)*
| | |
|---|---|
| **Route** | `/seller/earnings` |
| **Components** | Revenue chart (Chart.js / ngx-charts), payout history table, pending balance |
| **APIs** | `GET /sellers/me/earnings` |

---

## 2.14 Admin Dashboard

> Protected by `AdminGuard`. Lazy-loaded `AdminModule`.

### Admin Overview
| | |
|---|---|
| **Route** | `/admin/dashboard` |
| **Components** | Revenue KPI cards, orders/users charts, recent activity feed |
| **APIs** | `GET /admin/dashboard` |

### Admin User Management
| | |
|---|---|
| **Route** | `/admin/users` |
| **Components** | Users data table, search, filter by role/status, actions menu (suspend/ban) |
| **APIs** | `GET /admin/users`, `PUT /admin/users/{id}/status`, `DELETE /admin/users/{id}` |

### Admin Product Management
| | |
|---|---|
| **Route** | `/admin/products` |
| **User Actions** | View all products, soft-delete, edit |
| **APIs** | `GET /products`, `PUT /products/{id}`, `DELETE /products/{id}` |

### Admin Category Management
| | |
|---|---|
| **Route** | `/admin/categories` |
| **Components** | Category tree view, add/edit category dialog |
| **APIs** | `GET /categories`, `POST /categories`, `PUT /categories/{id}`, `DELETE /categories/{id}` |

### Admin Order Management
| | |
|---|---|
| **Route** | `/admin/orders` |
| **User Actions** | View all orders, update status, view invoice |
| **APIs** | `GET /admin/orders`, `PUT /orders/{id}/status` |

### Admin Seller Management
| | |
|---|---|
| **Route** | `/admin/sellers` |
| **User Actions** | View seller applications, approve/reject, view seller profile |
| **APIs** | `GET /admin/sellers`, `PUT /admin/sellers/{id}/verify` |

### Admin Promo Codes *(Bonus)*
| | |
|---|---|
| **Route** | `/admin/promo-codes` |
| **User Actions** | Create, edit, deactivate promo codes |
| **APIs** | `GET /admin/promo-codes`, `POST /admin/promo-codes`, `PUT /admin/promo-codes/{id}`, `DELETE /admin/promo-codes/{id}` |

### Admin Banner Management
| | |
|---|---|
| **Route** | `/admin/banners` |
| **User Actions** | Upload banners, reorder, set active/inactive |
| **APIs** | `GET /admin/banners`, `POST /admin/banners`, `PUT /admin/banners/{id}`, `DELETE /admin/banners/{id}` |

---

# 3. Feature Mapping

| SRS Feature | Backend Endpoints | Frontend Pages | Roles |
|---|---|---|---|
| User Registration | `POST /auth/register` | Register Page | Guest |
| User Login | `POST /auth/login` | Login Page | Guest |
| Email Verification | `GET /auth/verify-email` | Verify Email Page | Guest |
| Social Login (Bonus) | `POST /auth/google` | Login / Register | Guest |
| Password Reset | `POST /auth/forgot-password`, `POST /auth/reset-password` | Forgot/Reset Password | Guest |
| Profile Management | `GET/PUT /users/me`, avatar upload | Profile Page | Customer/Seller/Admin |
| Address Management | `POST/PUT/DELETE /users/me/addresses` | Address Book | Customer |
| Multi-user Roles | All protected endpoints | Role-based UI | All |
| Wishlist | `GET/POST/DELETE /wishlist` | Wishlist Page | Customer |
| Order History | `GET /users/me/orders` | Order History | Customer |
| Reviews & Ratings | `GET/POST/PUT/DELETE /products/{id}/reviews` | Product Detail | Customer |
| Product Categories | `GET /categories`, `GET/POST/PUT/DELETE /categories` (Admin) | Home, Listing, Admin | All / Admin |
| Product Listings | `GET /products`, `GET /products/{id}` | Product Listing, Product Detail | All |
| Product CRUD | `POST/PUT/DELETE /products` | Seller Add/Edit, Admin Products | Seller/Admin |
| Stock Availability | Stock field in product, `inStock` filter | Listing, Detail | All |
| Search & Filter | `GET /products?search=&categoryId=&minPrice=` | Product Listing | All |
| Shopping Cart | `GET/POST/PUT/DELETE /cart/*` | Cart Page | Customer/Guest |
| Quantity Adjustment | `PUT /cart/items/{id}` | Cart Page | Customer/Guest |
| Guest Checkout | Session header on cart/order endpoints | Cart, Checkout | Guest |
| Payment Methods | `POST /payments/initiate`, `POST /orders` | Checkout | Customer/Guest |
| Promo Codes (Bonus) | `POST/DELETE /cart/promo`, Admin CRUD | Cart, Checkout, Admin | Customer/Admin |
| Wallet (Bonus) | `POST /payments/wallet/topup`, `GET /payments/wallet/balance` | Checkout, Profile | Customer |
| Order Placement | `POST /orders` | Checkout | Customer/Guest |
| Order Tracking | `GET /orders/{id}`, `PUT /orders/{id}/status` | Order Detail | Customer/Seller/Admin |
| Order Notifications | Triggered publicly by status changes | — (email) | System |
| Payment Gateway | `POST /payments/initiate`, `POST /payments/webhook` | Checkout | Customer |
| Admin User Mgmt | `GET/PUT/DELETE /admin/users` | Admin User Management | Admin |
| Admin Product Mgmt | `GET/PUT/DELETE /products` (admin) | Admin Products | Admin |
| Admin Order Mgmt | `GET /admin/orders`, `PUT /orders/{id}/status` | Admin Orders | Admin |
| Admin Discounts (Bonus) | `GET/POST/PUT/DELETE /admin/promo-codes` | Admin Promo Codes | Admin |
| Admin Banners | `GET/POST/PUT/DELETE /admin/banners` | Admin Banners | Admin |
| Seller Registration | `POST /sellers/register` | Register + Seller Profile | Customer → Seller |
| Seller Products | `GET/POST/PUT/DELETE /products` (seller) | Seller Products | Seller |
| Seller Orders | `GET /sellers/me/orders`, `PUT /orders/{id}/status` | Seller Orders | Seller |
| Seller Earnings (Bonus) | `GET /sellers/me/earnings` | Seller Earnings | Seller |

---

# 4. Assumptions & Gaps

## 4.1 Missing / Unclear Items in the SRS

| # | Area | Issue | Recommendation |
|---|---|---|---|
| 1 | **Currency** | No currency specified | Define a base currency (e.g., EGP). Support multi-currency as a future enhancement. |
| 2 | **Shipping** | No shipping cost calculation method defined | Add a basic flat-rate or weight-based shipping model. Consider integrating a shipping provider API. |
| 3 | **Email Provider** | Confirmation/notification emails mentioned but no provider specified | Use SendGrid or Mailgun. Define email templates for: registration, order confirmation, status update, password reset. |
| 4 | **Guest Checkout Flow** | "Guest checkout" mentioned but how a guest tracks their order is unclear | Require guest email on checkout; send order link via email. Allow order lookup by email + order number. |
| 5 | **Product Variants** | No mention of size/color/variant support | Critically missing for fashion/electronics. Add a `variants` sub-entity to products (e.g., color: Red, size: XL, each with its own stock/price). |
| 6 | **Return & Refund Policy** | Orders can be cancelled, but no return/refund flow | Add a `POST /orders/{id}/return` endpoint and a return management page in Admin. |
| 7 | **Seller Approval Flow** | Seller registration is mentioned but approval workflow is vague | Clearly define: Pending → Admin Review → Approved/Rejected. Notify seller via email on decision. |
| 8 | **Product Approval** | No mention of whether admin must approve new product listings | For marketplace integrity, add an `isApproved` flag on products. Admin reviews new listings before they go live. |
| 9 | **Tax Calculation** | No tax/VAT logic mentioned | Add a configurable tax rate (e.g., 14% Egyptian VAT) applied at checkout. |
| 10 | **Image Storage** | Image upload is implied but no CDN/storage strategy is defined | Use a cloud object store (AWS S3 / Azure Blob / Cloudinary) with CDN. Never store images in the database. |
| 11 | **Inventory Management** | No stock reservation strategy defined | When item is added to cart, reserve stock (soft-lock for X minutes). Release on cart expiry or order failure. |
| 12 | **Multiple Sellers in One Order** | Cart can have products from different sellers; no order-splitting logic defined | Split a single customer order into sub-orders per seller. Each seller sees and processes only their sub-order. |
| 13 | **Wallet Funding Source** | Wallet is listed as a payment method but topup flow is unclear | Define explicit wallet top-up flow (pay via card/bank to add funds) before using at checkout. |
| 14 | **Loyalty Program (Bonus)** | Listed as a bonus but no rules defined | Define point earning (e.g., 1 point per 10 EGP spent) and redemption rules (e.g., 100 points = 10 EGP discount). |

---

## 4.2 Security Recommendations

- Implement **rate limiting** on auth endpoints (login, register, forgot-password) to prevent brute force.
- Use **HTTPS only**. Enforce via HSTS header.
- **Input sanitization** on all user-supplied text fields (prevent XSS/SQL injection).
- Validate **Stripe/PayPal webhook signatures** server-side before processing.
- Implement **RBAC (Role-Based Access Control)** via middleware on every protected route.
- Refresh tokens should be **rotated** on every use (one-time-use refresh tokens).
- Soft-delete for users/products — never hard delete customer or financial records.

---

## 4.3 Edge Cases Not Covered in SRS

| Edge Case | Suggested Handling |
|---|---|
| User deletes account with active orders | Anonymize personal data but retain order records for legal/financial compliance. |
| Product goes out of stock while in cart | Notify user at checkout; remove/disable item in cart automatically. |
| Seller deletes product that is in customer's cart/wishlist | Mark as unavailable; notify customer on next visit. |
| Promo code applied but product later removed from cart | Re-validate promo code applicability on checkout submit. |
| Payment succeeds but order creation fails | Use idempotency keys + database transactions; never double-charge. Implement a reconciliation job. |
| Two users buying the last item simultaneously | Use optimistic/pessimistic locking on stock field at the database level. First to commit wins; second gets a "sold out" error. |
| Seller account suspended mid-active orders | Admin must manually handle in-flight orders. Seller products hidden immediately. |
| Review submitted for a product from a returned/cancelled order | Only allow reviews for `delivered` or `completed` order statuses. |

---

## 4.4 Suggested Technical Enhancements

| Enhancement | Value |
|---|---|
| **Elasticsearch for product search** | Far superior full-text search, typo tolerance, and faceted filtering vs. SQL LIKE queries. |
| **Redis caching** | Cache product listings, category trees, banners (TTL: 5–15 min). Drastically reduces DB load. |
| **Signed image upload URLs** | Frontend uploads directly to S3 with a short-lived signed URL — no binary data passes through your API server. |
| **Event-driven architecture** | Use a message queue (RabbitMQ / Azure Service Bus) for: order confirmation emails, inventory updates, notifications — decoupled from the request cycle. |
| **Angular Signals** | Use Angular 17+ Signals for reactive state management instead of BehaviorSubjects for cleaner, more performant state. |
| **PWA Support** | Add Angular PWA for push notifications and offline browsing support. |
| **Swagger / OpenAPI** | Auto-generate API documentation from ASP.NET Core with Swashbuckle. Critical for team collaboration. |
| **Health checks & monitoring** | Add `/health` endpoint. Use Application Insights / Sentry for error tracking. |

---

*End of Technical Breakdown*
