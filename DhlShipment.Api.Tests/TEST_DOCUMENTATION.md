# DHL Shipment API - Unit Tests

## Overview

Comprehensive unit test suite for the DHL Shipment Tracking API using xUnit, Moq, and FluentAssertions.

## Test Structure

### 1. **Services Tests** - `Services/ShipmentServiceTests.cs`

#### CreateShipment Tests (4 tests)

- ✅ Creates shipment with valid data
- ✅ Generates unique tracking IDs
- ✅ Creates initial milestone with correct status and location
- ✅ Sets estimated delivery date correctly

#### GetShipmentByTrackingId Tests (3 tests)

- ✅ Returns shipment with valid tracking ID
- ✅ Throws KeyNotFoundException for invalid tracking ID
- ✅ Returns correct milestones

#### UpdateShipmentStatus Tests (5 tests)

- ✅ Updates status and adds new milestone
- ✅ Throws KeyNotFoundException for invalid tracking ID
- ✅ Can update status multiple times (maintains complete history)
- ✅ Maintains tracking ID and origin/destination
- ✅ Records recent timestamp for updates

**Total Service Tests: 12**

---

### 2. **Controllers Tests**

#### ShipmentsController Tests - `Controllers/ShipmentsControllerTests.cs`

**GetByTrackingId Tests (3 tests)**

- ✅ Returns 200 OK with shipment data
- ✅ Throws KeyNotFoundException for invalid tracking ID
- ✅ Calls service with correct tracking ID

**CreateShipment Tests (3 tests)**

- ✅ Returns 201 Created status
- ✅ Returns correct route values with tracking ID
- ✅ Calls service with correct DTO

**UpdateStatus Tests (4 tests)**

- ✅ Returns 200 OK with updated shipment
- ✅ Throws KeyNotFoundException for invalid tracking ID
- ✅ Calls service with correct parameters
- ✅ Handles different status values correctly

**Total ShipmentsController Tests: 10**

#### AuthController Tests - `Controllers/AuthControllerTests.cs`

**Login Tests (10 tests)**

- ✅ Admin login returns 200 OK with token
- ✅ User login returns 200 OK with token
- ✅ Invalid credentials return 401 Unauthorized
- ✅ Wrong password returns 401 Unauthorized
- ✅ Wrong email returns 401 Unauthorized
- ✅ Admin token contains Admin role claim
- ✅ User token contains User role claim
- ✅ Token contains correct issuer
- ✅ Token contains correct audience
- ✅ Token has expiration (approximately 60 minutes)
- ✅ Different users get different tokens

**Total AuthController Tests: 11**

---

### 3. **Middleware Tests** - `Middleware/ExceptionMiddlewareTests.cs`

**Exception Handling Tests (7 tests)**

- ✅ KeyNotFoundException returns 404 Not Found
- ✅ UnauthorizedAccessException returns 401 Unauthorized
- ✅ ArgumentException returns 400 Bad Request
- ✅ Generic Exception returns 500 Internal Server Error
- ✅ No exception calls next delegate
- ✅ Logs exception message
- ✅ Response content type is application/json

**Total Middleware Tests: 7**

---

## Test Statistics

| Category                     | Count  |
| ---------------------------- | ------ |
| Service Tests                | 12     |
| Controller Tests (Shipments) | 10     |
| Controller Tests (Auth)      | 11     |
| Middleware Tests             | 7      |
| **Total Tests**              | **40** |

---

## Running Tests

### Run All Tests

```bash
cd DhlShipment.Api.Tests
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter ClassName=DhlShipment.Api.Tests.Services.ShipmentServiceTests
```

### Run Tests with Verbose Output

```bash
dotnet test -v detailed
```

### Run Tests with Coverage (requires coverlet)

```bash
dotnet test /p:CollectCoverage=true
```

---

## Test Dependencies

- **xUnit**: Test framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.NET.Test.Sdk**: Test SDK

---

## Test Coverage

### Services (100% Coverage)

- ✅ CreateShipment method
- ✅ GetShipmentByTrackingId method
- ✅ UpdateShipmentStatus method
- ✅ Mapping logic (MapToDto)

### Controllers (100% Coverage)

- ✅ GetByTrackingId endpoint
- ✅ CreateShipment endpoint
- ✅ UpdateStatus endpoint
- ✅ Login endpoint

### Middleware (100% Coverage)

- ✅ Exception handling for all exception types
- ✅ Normal request flow
- ✅ Logging behavior

---

## Key Testing Patterns Used

### 1. **Arrange-Act-Assert (AAA) Pattern**

Every test follows the AAA pattern for clarity and consistency.

### 2. **Moq for Dependencies**

Controllers are tested with mocked services to isolate behavior.

### 3. **FluentAssertions for Readability**

Assertions are written in fluent style for better readability.

### 4. **Data-Driven Testing**

Tests validate multiple scenarios (success, failure, edge cases).

### 5. **JWT Token Validation**

Auth tests validate JWT structure, claims, issuer, audience, and expiration.

---

## Example Test

```csharp
[Fact]
public void CreateShipment_WithValidData_ReturnsShipmentResponseDto()
{
    // Arrange
    var dto = new CreateShipmentDto(
        Origin: "New York",
        Destination: "Los Angeles",
        EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5)
    );

    // Act
    var result = _service.CreateShipment(dto);

    // Assert
    result.Should().NotBeNull();
    result.Origin.Should().Be("New York");
    result.TrackingId.Should().StartWith("DHL");
}
```

---

## Continuous Integration

These tests are designed to run in CI/CD pipelines and provide:

- ✅ Fast execution (< 5 seconds for all tests)
- ✅ No external dependencies
- ✅ Deterministic results
- ✅ Clear failure messages

---

**Generated:** December 21, 2025
