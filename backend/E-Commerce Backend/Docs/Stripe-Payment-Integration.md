# Stripe Payment Integration

This document describes the Stripe integration added to the backend payment module.

## Endpoints

### 1) Initiate Payment Intent

- Method: `POST`
- Route: `/api/payments/initiate`
- Auth: `Bearer` token required
- Body:

```json
{
  "orderId": "00000000-0000-0000-0000-000000000000",
  "paymentMethod": "Stripe",
  "currency": "EGP"
}
```

- Success response:

```json
{
  "success": true,
  "message": "Payment intent created successfully.",
  "data": {
    "clientSecret": "pi_xxx_secret_xxx",
    "paymentIntentId": "pi_xxx"
  },
  "errors": []
}
```

### 2) Stripe Webhook

- Method: `POST`
- Route: `/api/payments/webhook`
- Auth: No bearer token (validated by Stripe signature)
- Header required: `Stripe-Signature`

Handled events:

- `payment_intent.succeeded` -> updates order `PaymentStatus` to `Paid`
- `payment_intent.payment_failed` -> updates order `PaymentStatus` to `Failed`

Order resolution from webhook is done via Stripe payment intent metadata key: `orderId`.

## Configuration

Set these values in:

- `appsettings.json` for shared/default placeholders
- `appsettings.Development.json` for local development overrides
- or `dotnet user-secrets` for local secrets storage (recommended)

```json
"Stripe": {
  "SecretKey": "",
  "PublishableKey": "",
  "WebhookSecret": "",
  "DefaultCurrency": "EGP"
}
```

### Required values

- `SecretKey`: Stripe secret key (`sk_test_...`)
- `WebhookSecret`: Stripe webhook signing secret (`whsec_...`)

`PublishableKey` is included for completeness and frontend usage.

Security note:

- Do not commit real `sk_test_...` or `whsec_...` values.

## Local Testing Notes

Example Stripe CLI forwarding command:

```bash
stripe listen --forward-to http://localhost:5068/api/payments/webhook --events payment_intent.succeeded,payment_intent.payment_failed
```

Use the produced `whsec_...` secret in `Stripe:WebhookSecret`.

Optional user-secrets commands:

```bash
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..." --project "E-Commerce Backend/E-Commerce.csproj"
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..." --project "E-Commerce Backend/E-Commerce.csproj"
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..." --project "E-Commerce Backend/E-Commerce.csproj"
dotnet user-secrets set "Stripe:DefaultCurrency" "EGP" --project "E-Commerce Backend/E-Commerce.csproj"
```

## Authorization Behavior

`POST /api/payments/initiate` allows:

- Admin users for any order
- Non-admin users only for their own orders

Additional guards:

- Order must exist
- Order `PaymentMethod` must be `Stripe`
- Order must not be already paid
