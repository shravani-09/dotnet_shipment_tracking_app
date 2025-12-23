# DHL Shipment Tracking API

ASP.NET Core REST API for managing and tracking DHL shipments. Provides endpoints for creating shipments, tracking by ID, and updating status throughout the shipment lifecycle.

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10.0 with C# 13
- **Database**: Entity Framework Core (In-Memory)
- **Authentication**: JWT-based with role-based authorization
- **API Docs**: Swagger/OpenAPI

## ✨ Features

- **Shipment Tracking**: Public endpoint to track shipments by tracking ID
- **Shipment Creation**: Admin-only endpoint to create new shipments
- **Status Updates**: Admin-only endpoint to update shipment status and milestones
- **Authentication**: JWT-based login with Admin/User roles
- **Validation**: Server-side validation for all inputs
- **Error Handling**: Centralized exception handling with consistent responses

## 📁 Project Structure

```
DhlShipment.Api/
├── Controllers/           # API endpoints
├── Services/              # Business logic
├── Data/                  # Database context
├── Models/                # Domain entities
├── DTOs/                  # Data Transfer Objects
├── Middleware/            # Exception handling
├── Exceptions/            # Custom exceptions
├── Validation/            # Custom validators
├── Program.cs             # Startup configuration
├── appsettings.json       # Configuration
└── DhlShipment.Api.http   # HTTP test requests
```

## 🚀 Getting Started

### Prerequisites

- **.NET SDK 10.0** or later

### Setup

```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

**Access the API:**

- API: `https://localhost:5001/api`
- Swagger UI: `https://localhost:5001/swagger/index.html`

## 📚 API Endpoints

### Authentication

**Login** - `POST /auth/login`

- Request: `{ "email": "admin@dhl.com", "password": "admin123" }`
- Response: `{ "token": "...", "role": "Admin" }`
- **Test Credentials**:
  - Admin: `admin@dhl.com` / `admin123`
  - User: `user@dhl.com` / `user123`

### Shipment Operations

**Get Shipment by Tracking ID** - `GET /shipment/{trackingId}`

- Authorization: Not required
- Response: Shipment details with milestones

**Get All Shipments** - `GET /shipment/view`

- Authorization: Required (Admin only)
- Response: Array of all shipments

**Create Shipment** - `POST /shipment/create`

- Authorization: Required (Admin only)
- Request: `{ "origin": "...", "destination": "...", "estimatedDeliveryDate": "..." }`
- Response: Created shipment with tracking ID

**Update Shipment Status** - `PUT /shipment/{trackingId}/status`

- Authorization: Required (Admin only)
- Request: `{ "status": "InTransit", "location": "Chicago" }`
- Response: Updated shipment with new milestone

### Status Values

```
Created = 0
PickedUp = 1
InTransit = 2
ArrivedAtFacility = 3
OutForDelivery = 4
Delivered = 5
Delayed = 6
Exception = 7
```

### Validation Rules

- **Origin/Destination**: 2-100 characters, letters/spaces/hyphens only
- **EstimatedDeliveryDate**: Must be a future date
- **Location**: 2-100 characters, letters/spaces/hyphens only
- **Tracking ID**: Format `DHL` + 6 digits (auto-generated)

### Error Responses

```json
{
  "statusCode": 400,
  "message": "Error description"
}
```

Status codes: `400` (validation), `404` (not found), `401` (unauthorized), `500` (server error)

## 🔐 Authentication & Authorization

- JWT tokens expire in 60 minutes
- Include token in Authorization header: `Bearer <token>`
- **Admin**: Full access (create, update shipments)
- **User**: Read-only access (track shipments)

## 🧪 Testing

Use the included `DhlShipment.Api.http` file with REST Client extension in VS Code, or use Swagger UI at `https://localhost:5001/swagger/index.html`

Run unit tests:

```bash
dotnet test
```
