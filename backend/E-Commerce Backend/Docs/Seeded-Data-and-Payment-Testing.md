# Seeded Data and Payment Testing Guide

This guide explains the seed data available in development and how to test Stripe payment endpoints as a team.

## When Seeding Runs

Seeding runs automatically at app startup in Development mode through `IAppSeeder`.

## Seeded Accounts

### Admin
- Email: `admin@site.com`
- Password: `Admin@123`
- Role: `Admin`

### Seller
- Email: `seller.seed@site.com`
- Password: `Seed@12345`
- Role: `Seller`
- Store: `Seed Tech Hub`

### Customer
- Email: `customer.seed@site.com`
- Password: `Seed@12345`
- Role: `Customer`

## Seeded Domain Data

- Categories:
  - `electronics`
  - `electronics-accessories` (child of electronics)
- Promo code:
  - Code: `SEED10`
  - Type: `Percentage`
- Products:
  - `seed-wireless-mouse`
  - `seed-mechanical-keyboard`
- Customer default address and cart item are pre-created.

## Seeded Orders for Payment Tests

### Stripe Pending Order
- OrderNumber: `ORD-SEED-STRIPE-1001`
- Id: `cccccccc-cccc-cccc-cccc-cccccccccccc`
- PaymentMethod: `Stripe`
- PaymentStatus: `Pending`
- Intended for testing `POST /api/payments/initiate`

### Stripe Paid Order
- OrderNumber: `ORD-SEED-PAID-1002`
- Id: `ffffffff-ffff-ffff-ffff-ffffffffffff`
- PaymentMethod: `Stripe`
- PaymentStatus: `Paid`

## Stripe Configuration (Development)

The following section in `appsettings.Development.json` must be set:

```json
"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_...",
  "WebhookSecret": "whsec_...",
  "DefaultCurrency": "EGP"
}
```

Notes:
- `WebhookSecret` changes per `stripe listen` session.
- When you restart listener, copy the new `whsec_...` value.

## Local Webhook Listener

Run Stripe listener and forward to backend webhook:

```bash
stripe listen --forward-to http://localhost:5068/api/payments/webhook --events payment_intent.succeeded,payment_intent.payment_failed
```

## End-to-End Test Flow

1. Start API in Development mode.
2. Login as seeded customer:
   - `POST /api/auth/login`
3. Query seed order id for `ORD-SEED-STRIPE-1001` (via DB or API if added).
4. Initiate payment:
   - `POST /api/payments/initiate`
5. Confirm response includes:
   - `clientSecret`
   - `paymentIntentId`
6. Trigger Stripe fixtures:

```bash
stripe trigger payment_intent.succeeded
stripe trigger payment_intent.payment_failed
```

7. Verify listener reports:
   - `POST /api/payments/webhook [200]`

## Expected Smoke Result

During healthy validation, `POST /api/payments/initiate` should return:

- `success = true`
- non-empty `data.paymentIntentId`
- non-empty `data.clientSecret`

## Useful SQL Checks

```sql
SELECT Email FROM Users WHERE Email IN ('admin@site.com','seller.seed@site.com','customer.seed@site.com');
SELECT OrderNumber, PaymentMethod, PaymentStatus FROM Orders WHERE OrderNumber IN ('ORD-SEED-STRIPE-1001','ORD-SEED-PAID-1002');
SELECT Slug, Price, Stock FROM Products WHERE Slug IN ('seed-wireless-mouse','seed-mechanical-keyboard');
```

## Idempotency

Seed logic is idempotent:
- It creates missing records.
- It updates key fields for existing seed records.
- It is safe to run repeatedly in Development.
