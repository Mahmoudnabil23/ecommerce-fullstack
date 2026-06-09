# E-Commerce Backend

ASP.NET Core Web API backend for the ecommerce system, using Entity Framework Core, ASP.NET Identity, JWT authentication, repository/service patterns, and Stripe payment integration.

## What Is Included

- Authentication and authorization with roles (`Admin`, `Seller`, `Customer`)
- Domain models for products, categories, carts, orders, reviews, promo codes, and sellers
- Stripe payment intent initiation and webhook handling
- Development startup seeding for identity + deterministic domain test data

## Project Structure

- Solution root: `E-Commerce-Backend/`
- Main API project: `E-Commerce Backend/`
- Feature docs: `E-Commerce Backend/Docs/`

## Prerequisites

- .NET 8 SDK
- SQL Server (local or accessible instance)
- Optional: Stripe CLI (for local webhook testing)

## Setup and Run (Development)

From solution root:

```powershell
dotnet restore .\E-Commerce Backend\E-Commerce.csproj
dotnet ef database update --project .\E-Commerce Backend\E-Commerce.csproj
dotnet run --project .\E-Commerce Backend\E-Commerce.csproj
```

By default, the API listens on:

- `http://localhost:5068`

## Seeding

Development startup automatically runs:

1. Identity seeding (roles + admin)
2. Domain seeding (seller/customer accounts, categories, products, promo code, cart, orders)

Seed data is idempotent, so repeated app starts are safe.

Detailed seeded records and test users are documented in:

- `E-Commerce Backend/Docs/Seeded-Data-and-Payment-Testing.md`

## Stripe Configuration

`Stripe` settings are available in appsettings with placeholders and should be set locally via either:

- `appsettings.Development.json` (local only)
- `dotnet user-secrets` (recommended for safety)

Required values:

- `SecretKey`: `sk_test_...`
- `PublishableKey`: `pk_test_...`
- `WebhookSecret`: `whsec_...`
- `DefaultCurrency`: for example `EGP`

Never commit real Stripe keys to source control.

## Payment Endpoints

- `POST /api/payments/initiate`
	- Auth required (`Bearer`)
	- Creates Stripe payment intent for eligible order
- `POST /api/payments/webhook`
	- No bearer token
	- Stripe signature validated via `Stripe-Signature` header
	- Handles `payment_intent.succeeded` and `payment_intent.payment_failed`

Detailed payment documentation:

- `E-Commerce Backend/Docs/Stripe-Payment-Integration.md`

## Local Webhook Testing

```bash
stripe listen --forward-to http://localhost:5068/api/payments/webhook --events payment_intent.succeeded,payment_intent.payment_failed
```

Copy the generated `whsec_...` into local Stripe config before sending webhook events.

## Quick Smoke Validation

- Login as seeded customer (`customer.seed@site.com` / `Seed@12345`)
- Initiate payment for seeded pending order `ORD-SEED-STRIPE-1001`
- Verify response includes `paymentIntentId` and `clientSecret`
- Trigger Stripe fixtures and verify webhook responds with HTTP 200

## Documentation Index

- `E-Commerce Backend/Docs/Stripe-Payment-Integration.md`
- `E-Commerce Backend/Docs/Seeded-Data-and-Payment-Testing.md`
- `E-Commerce Backend/Docs/Ecommerce-Technical-Breakdown.md`
- `E-Commerce Backend/Docs/Backend-Core-Layer.md`