# Solid.Api Swagger and Endpoint Guide

## Current Verification

- `dotnet build Solid.Api/Solid.Api.csproj` passes with 0 errors.
- Swagger is available when the app runs in `Development`: `http://localhost:5209/swagger`.
- SignalR is mapped at `/hubs/notifications` and rejects anonymous clients with `401`.
- SignalR negotiate succeeds with a valid JWT bearer token.
- Database-backed endpoints currently return `500` locally because SQL Server cannot open database `Solid` for the current Windows user.
- Laravel Pint could not run because `vendor/` is not installed.

## Run From Swagger

1. Install .NET SDK 9.
2. Make sure SQL Server is running and has a database named `Solid`, or update `Solid.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Solid;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. Restore the local EF tool:

```powershell
dotnet tool restore
```

4. Create/update the SQL Server database using EF:

```powershell
dotnet tool run dotnet-ef database update --project Solid.Api/Solid.Api.csproj --startup-project Solid.Api/Solid.Api.csproj --context SolidDbContext
```

5. Run:

```powershell
dotnet run --project Solid.Api/Solid.Api.csproj
```

6. Open `http://localhost:5209/swagger`.
7. Use `POST /api/auth/register` or `POST /api/auth/login`.
8. Copy `body.token`.
9. In Swagger, click `Authorize` and enter:

```text
Bearer <token>
```

10. Use the protected endpoints.

## External Resources

- SQL Server database `Solid` is required for almost every endpoint.
- `Otp:TtlSeconds` controls how long an OTP stays valid; default is `300` seconds.
- Twilio SMS credentials are required for real OTP delivery: `Sms:Twilio:AccountSid`, `Sms:Twilio:AuthToken`, and `Sms:Twilio:From`.
- `Jitsi:ServerUrl` is required for real session joining/start links.
- Payment gateways are not fully wired in `Solid.Api`; the old Laravel webhook route exists in Laravel but not in the .NET API.
- FCM is temporarily disabled in `Modules/Notifications/Services/PushNotificationService.php`.
- Firebase credentials are not required while FCM is disabled.
- SignalR runs in-process; no external SignalR service is required right now.

## SignalR Notifications

- Hub URL: `http://localhost:5209/hubs/notifications`
- Auth: `Bearer <JWT token>`
- On connect, the server adds the connection to group `user:{userId}`.
- Test method: call hub method `Ping`; the client receives event `pong` with `{ "at": "..." }`.
- Production notification broadcasting still needs server-side calls through `IHubContext<NotificationsHub>` when notifications are created.

## Differences From Old Laravel API

- The old Laravel API used Sanctum tokens. `Solid.Api` now returns JWT tokens so Swagger authorization works with the configured JWT middleware.
- Missing in `Solid.Api`: `POST /api/payments/webhook/{gateway}`.
- Old Laravel notifications route used `Route::apiResource`, but only `index` is implemented in the controller. `Solid.Api` exposes only `GET /api/v1/notifications`.
- `Solid.Api` payment initiation creates a manual pending payment and returns an empty `payment_url`; old Laravel delegates to configured gateways.
- `Solid.Api` session join/start returns a random placeholder `jitsi_jwt`; old Laravel has a Jitsi token service.

## Common Response Envelope

All current endpoints return the same envelope:

```json
{
  "custom_code": 2000,
  "status": true,
  "message": "Data retrieved successfully.",
  "body": {},
  "info": "from response action"
}
```

Error example:

```json
{
  "custom_code": 2000,
  "status": false,
  "message": "Not found.",
  "body": {},
  "info": "from response action"
}
```

## Public Endpoints

### POST `/api/auth/register`

Request:

```json
{
  "display_name": "Ahmed Hassan",
  "mobile_number": "+201001234567",
  "password": "SecurePass123!",
  "preferred_language": "ar",
  "addiction_duration_id": 1,
  "education_level_id": 2,
  "had_prior_treatment": false,
  "substance_ids": [1, 2],
  "treatment_type_ids": [3],
  "addiction_reason": "Stress and peer pressure",
  "days_clean": 15
}
```

Success body:

```json
{
  "user": {
    "id": 1,
    "display_name": "Ahmed Hassan",
    "mobile_number": "+201001234567",
    "role": "addict",
    "preferred_language": "ar",
    "is_active": false
  },
  "token": "<jwt>",
  "token_type": "Bearer"
}
```

### POST `/api/auth/verify`

Headers:

```text
Authorization: Bearer <register_token>
```

Request:

```json
{
  "otp": "<sms_otp>"
}
```

Success body:

```json
{}
```

### POST `/api/auth/login`

Request:

```json
{
  "mobile_number": "+201001234567",
  "password": "SecurePass123!",
  "device_id": "device-001"
}
```

Success body:

```json
{
  "token": "<jwt>",
  "token_type": "Bearer",
  "user": {
    "id": 1,
    "display_name": "Ahmed Hassan",
    "role": "addict"
  }
}
```

### POST `/api/auth/forgot-password`

Request:

```json
{
  "mobile_number": "+201001234567"
}
```

Success body:

```json
{
  "token": "<password_reset_token>"
}
```

### POST `/api/auth/verify-forgot-otp`

Request:

```json
{
  "token": "<password_reset_token>",
  "otp": "<sms_otp>"
}
```

Success body:

```json
{
  "reset_token": "<reset_token>"
}
```

### POST `/api/auth/reset-password`

Request:

```json
{
  "reset_token": "<reset_token>",
  "password": "NewSecurePass123!"
}
```

Success body:

```json
{}
```

### GET `/api/privacy-policy`

Request: no body.

Success body:

```json
{
  "privacy_policy": "<html-or-text>"
}
```

### GET `/api/terms-and-conditions`

Request: no body.

Success body:

```json
{
  "terms_and_conditions": "<html-or-text>"
}
```

### GET `/api/lookups/substances`

Headers:

```text
Accept-Language: ar
```

Request: no body.

Success body:

```json
{
  "categories": [
    {
      "id": 1,
      "label": "Category",
      "substances": [
        {
          "id": 1,
          "label": "Substance"
        }
      ]
    }
  ]
}
```

### GET `/api/lookups/{type}`

Example: `/api/lookups/education_levels`

Request: no body.

Success body:

```json
{
  "values": [
    {
      "id": 1,
      "value_key": "high_school",
      "label": "High school"
    }
  ]
}
```

## Protected Auth Endpoints

All endpoints in this section require:

```text
Authorization: Bearer <jwt>
```

### POST `/api/auth/logout`

Request: no body.

Success body:

```json
{}
```

### GET `/api/auth/me`

Request: no body.

Success body:

```json
{
  "user": {
    "id": 1,
    "display_name": "Ahmed Hassan",
    "email": null,
    "mobile_number": "+201001234567",
    "role": "addict",
    "payment_methods": []
  }
}
```

### DELETE `/api/auth/account`

Request: no body.

Success body:

```json
{}
```

## Users

### GET `/api/user`

Request: no body.

Success body:

```json
{
  "id": 1,
  "display_name": "Ahmed Hassan",
  "mobile_number": "+201001234567",
  "role": "addict"
}
```

### GET `/api/users/{userId}`

Request: no body.

Success body:

```json
{
  "user": {
    "id": 1,
    "display_name": "Ahmed Hassan",
    "role": "addict"
  }
}
```

### PUT `/api/profile`

Request:

```json
{
  "display_name": "Ahmed Updated",
  "email": "ahmed@example.com",
  "mobile_number": "+201001234567",
  "bio": "Short bio",
  "avatar_url": "https://example.com/avatar.png"
}
```

Success body:

```json
{
  "user": {
    "id": 1,
    "display_name": "Ahmed Updated",
    "email": "ahmed@example.com"
  }
}
```

### GET `/api/instructors`

Request: no body.

Success body:

```json
{
  "instructors": [
    {
      "id": 2,
      "display_name": "Dr. Mostafa",
      "bio": "Instructor bio",
      "avatar_url": null
    }
  ]
}
```

### GET `/api/instructors/{userId}`

Request: no body.

Success body:

```json
{
  "instructor": {
    "id": 2,
    "display_name": "Dr. Mostafa"
  }
}
```

## Groups

### GET `/api/groups`

Request: no body.

Success body:

```json
{
  "groups": [
    {
      "id": 1,
      "group_type": "mixed",
      "status": "forming",
      "name_ar": "مجموعة جديدة",
      "name_en": "New Group"
    }
  ]
}
```

### GET `/api/groups/my`

Request: no body.

Success body:

```json
{
  "group": {
    "id": 1,
    "status": "forming"
  }
}
```

### POST `/api/groups/subscribe`

Request: no body.

Success body:

```json
{
  "group": {
    "id": 1,
    "status": "forming"
  }
}
```

## Sessions

### GET `/api/sessions`

Request: no body.

Success body:

```json
{
  "sessions": [
    {
      "id": 1,
      "group_id": 1,
      "instructor_id": 2,
      "session_number": 1,
      "title": "Session 1",
      "status": "scheduled",
      "is_locked": true
    }
  ]
}
```

### GET `/api/sessions/me`

Request: no body.

Success body:

```json
{
  "sessions": [
    {
      "id": 1,
      "status": "scheduled"
    }
  ]
}
```

### GET `/api/sessions/{sessionId}`

Request: no body.

Success body:

```json
{
  "session": {
    "id": 1,
    "jitsi_room_name": "solid-room-1",
    "duration_minutes": 45
  }
}
```

### POST `/api/sessions/{sessionId}/join`

Request: no body.

Success body:

```json
{
  "jitsi_jwt": "<placeholder-token>",
  "jitsi_room_name": "solid-room-1",
  "jitsi_server_url": "https://meet.example.com",
  "session_duration_minutes": 45
}
```

### POST `/api/sessions/{sessionId}/leave`

Request: no body.

Success body:

```json
{}
```

### POST `/api/sessions/{sessionId}/start`

Request: no body.

Success body:

```json
{
  "session": {
    "id": 1,
    "status": "live"
  },
  "jitsi_jwt": "<placeholder-token>",
  "jitsi_room_name": "solid-room-1",
  "jitsi_server_url": "https://meet.example.com",
  "session_duration_minutes": 45
}
```

### POST `/api/sessions/{sessionId}/end`

Request: no body.

Success body:

```json
{}
```

### POST `/api/sessions/{sessionId}/feedback`

Request:

```json
{
  "rating": 5,
  "comment": "Great session"
}
```

Success body:

```json
{
  "attendance": {
    "session_id": 1,
    "user_id": 1,
    "rating": 5,
    "comment": "Great session"
  }
}
```

## Payments

### POST `/api/payments/initiate/{sessionId}`

Request: no body.

Success body:

```json
{
  "payment": {
    "id": 1,
    "user_id": 1,
    "session_id": 1,
    "amount": 1200,
    "currency": "EGP",
    "status": "pending",
    "gateway": "manual"
  },
  "payment_url": ""
}
```

### GET `/api/payments/history`

Request: no body.

Success body:

```json
[
  {
    "id": 1,
    "amount": 1200,
    "currency": "EGP",
    "status": "pending"
  }
]
```

### POST `/api/payment-methods`

Request:

```json
{
  "card_holder": "Ahmed Hassan",
  "card_number": "4111111111111111",
  "expiry": "12/30",
  "is_default": true,
  "gateway_token": "tok_123"
}
```

Success body:

```json
{
  "payment_method": {
    "id": 1,
    "card_holder": "Ahmed Hassan",
    "card_number": "4111111111111111",
    "expiry": "12/30",
    "is_default": true
  }
}
```

### GET `/api/payment-methods`

Request: no body.

Success body:

```json
{
  "payment_methods": [
    {
      "id": 1,
      "card_holder": "Ahmed Hassan",
      "card_number": "4111111111111111",
      "expiry": "12/30",
      "is_default": true
    }
  ]
}
```

## Recommendations

### GET `/api/recommendations`

Request: no body.

Success body:

```json
{
  "recommendations": [
    {
      "id": 1,
      "substance_category_id": 1,
      "type": "clinic",
      "name_ar": "اسم التوصية",
      "name_en": "Recommendation name",
      "contact_info": "01000000000",
      "latitude": 30.0444,
      "longitude": 31.2357,
      "is_active": true
    }
  ]
}
```

## Settings and Notifications

### GET `/api/v1/settings`

Request: no body.

Success body:

```json
{
  "settings": {
    "session_price": {
      "value": "1200",
      "type": "int"
    },
    "group_min_members": {
      "value": "7",
      "type": "int"
    }
  }
}
```

### GET `/api/v1/settings/{key}`

Request: no body.

Success body:

```json
{
  "setting": {
    "key": "session_price",
    "value": "1200",
    "type": "string"
  }
}
```

### PUT `/api/v1/settings/{key}`

Request:

```json
{
  "value": 1500
}
```

Success body:

```json
{}
```

### GET `/api/v1/notifications`

Request: no body.

Success body:

```json
{
  "notifications": [
    {
      "id": "e7b5bd3c-0000-4000-8000-000000000000",
      "title": "Notification",
      "body": "Notification body",
      "type": "info",
      "icon": "bell",
      "read_at": null,
      "created_at": "2026-06-03T12:00:00"
    }
  ],
  "pagination": {
    "total": 1,
    "count": 1,
    "per_page": 20,
    "current_page": 1,
    "total_pages": 1
  }
}
```
