# 🚗 Car Auction Management API

A Car auction management system, built with **.NET 10**, **Entity Framework Core**, **JWT Authentication**, and **xUnit**.

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Running](#-running)
- [API Endpoints](#-api-endpoints)
- [Authentication](#-authentication)
- [Usage Examples](#-usage-examples)
- [Testing](#-testing)
- [Logging](#-logging)
- [Project Structure](#-project-structure)

---

## ✨ Features

✅ **Vehicle Management**

- Supports 4 types: Sedan, Hatchback, SUV, Truck
- Automatic data validation with FluentValidation
- Advanced search with multiple filters (type, manufacturer, model, year)
- Result pagination

✅ **Auction System**

- Create, manage, and close auctions
- One active auction per vehicle
- Bid history

✅ **Bidding with Validations**

- Mandatory minimum increment (€100)
- Validation that bid is higher than the previous one
- Identification of the highest bidder

✅ **Authentication & Security**

- JWT (JSON Web Tokens)
- Endpoint authorization
- Flexible token configuration

✅ **Complete Logging**

- Serilog with console and file output
- Daily logs in `logs/`
- Full traceability

✅ **Database**

- Entity Framework Core with InMemory
- Easy migration to SQL Server, PostgreSQL, etc.
- Configured relationships and constraints

✅ **Unit Tests**

- 38 tests with xUnit
- FluentAssertions for readable assertions

---

## 🏗️ Architecture

**Clean Architecture** with clear separation of responsibilities:

```
CarAuctionManagementAPI/
├── CarAuctionManagement.Domain/          # Entities, Exceptions, Interfaces
│   ├── Entities/                         # Vehicle, Auction, Sedan, SUV, etc.
│   ├── Exceptions/                       # DomainException, InvalidBidException, etc.
│   └── Ports/                            # IVehicleRepository, IAuctionRepository
│
├── CarAuctionManagement.Application/     # Business Logic
│   ├── Services/                         # AuctionService, JwtTokenService
│   └── DTOs/                             # Data Transfer Objects
│
├── CarAuctionManagement.Infrastructure/  # Persistence
│   ├── Context/                          # AuctionDbContext (EF Core)
│   └── Repositories/                     # EfVehicleRepository, EfAuctionRepository
│
├── CarAuctionManagementAPI/              # Presentation (Controllers)
│   ├── Controllers/                      # AuctionsController, AuthController
│   ├── Requests/                         # AddVehicleRequest, PlaceBidRequest, etc.
│   ├── Responses/                        # VehicleResponse, AuctionResponse
│   └── Validators/                       # AddVehicleRequestValidator, etc.
│
└── CarAuctionManagement.Tests/           # Unit Tests
    └── AuctionServiceTests.cs            # 38 tests with xUnit + FluentAssertions
```

### Implemented Patterns

- **Domain-Driven Design (DDD)**: Domain entities with business logic
- **Repository Pattern**: Decoupling of persistence
- **Dependency Injection**: Configured via `Program.cs`
- **DTO Pattern**: Separation between input/output models
- **Validation Pattern**: FluentValidation for declarative validations
- **JWT Authentication**: Secure and stateless tokens

---

## 📦 Prerequisites

- **.NET 10 SDK** or higher ([Download](https://dotnet.microsoft.com/download))
- **Visual Studio 2026** (Enterprise/Professional/Community) or **VS Code**
- **Git** to clone the repository

### Verify Installation

```bash
dotnet --version
```

---

## 🚀 Installation

### 1. Restore Dependencies

```bash
dotnet restore
```

### 2. Verify Build

```bash
dotnet build
```

---

## ⚙️ Configuration

### appsettings.json

The `appsettings.json` file contains the application settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "SecretKey": "your-super-secret-key",
    "Issuer": "CarAuctionManagement",
    "Audience": "CarAuctionClients",
    "ExpirationMinutes": 60
  }
}
```

### Demo Users

For testing, use the following users:

| Username | Password    | Role   |
| -------- | ----------- | ------ |
| admin    | admin123    | Admin  |
| bidder1  | password123 | Bidder |
| bidder2  | password123 | Bidder |

---

## ▶️ Running

### Development Mode

```bash
dotnet run --project CarAuctionManagementAPI/CarAuctionManagementAPI.csproj
```

The API will be available at:

- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `https://localhost:5001/swagger`

### Release Mode

```bash
dotnet run --project CarAuctionManagementAPI/CarAuctionManagementAPI.csproj --configuration Release
```

---

## 🔌 API Endpoints

### Authentication

#### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "bidder1",
  "password": "password123"
}
```

**Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login successful."
}
```

### Vehicles

#### Add Vehicle

```http
POST /api/auctions/vehicles
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": "sedan-001",
  "type": "sedan",
  "manufacturer": "Toyota",
  "model": "Camry",
  "year": 2023,
  "startingBid": 15000,
  "numberOfDoors": 4
}
```

#### Search Vehicles

```http
GET /api/auctions/vehicles/search?type=sedan&manufacturer=toyota&year=2023
Authorization: Bearer {token}
```

#### Search Vehicles with Pagination

```http
GET /api/auctions/vehicles/search-paged?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

**Response:**

```json
{
  "data": [
    {
      "id": "sedan-001",
      "type": "Sedan",
      "manufacturer": "Toyota",
      "model": "Camry",
      "year": 2023,
      "startingBid": 15000
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

### Auctions

#### Start Auction

```http
POST /api/auctions/start?vehicleId=sedan-001
Authorization: Bearer {token}
```

#### Place Bid

```http
POST /api/auctions/bid
Authorization: Bearer {token}
Content-Type: application/json

{
  "vehicleId": "sedan-001",
  "amount": 16000
}
```

**Validations:**

- Bid must be ≥ Previous Bid + €100
- Auction must be active

#### Close Auction

```http
POST /api/auctions/close?vehicleId=sedan-001
Authorization: Bearer {token}
```

---

## 🔐 Authentication

### How It Works

1. **Login** via `/api/auth/login`
2. **Receive JWT token** valid for 60 minutes
3. **Include token** in the `Authorization: Bearer {token}` header on all protected requests

### JWT Token Structure

```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "bidder1",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "bidder1",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "bidder",
  "iat": 1700000000,
  "exp": 1700003600
}

Signature: HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secretKey)
```

---

## 📚 Usage Examples

### Complete Example with cURL

```bash
# 1. Login
TOKEN=$(curl -s -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"bidder1","password":"password123"}' \
  | jq -r '.token')

echo "Token: $TOKEN"

# 2. Add Vehicle
curl -X POST "https://localhost:5001/api/auctions/vehicles" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "sedan-001",
    "type": "sedan",
    "manufacturer": "Toyota",
    "model": "Camry",
    "year": 2023,
    "startingBid": 15000,
    "numberOfDoors": 4
  }'

# 3. Start Auction
curl -X POST "https://localhost:5001/api/auctions/start?vehicleId=sedan-001" \
  -H "Authorization: Bearer $TOKEN"

# 4. Place Bid
curl -X POST "https://localhost:5001/api/auctions/bid" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": "sedan-001",
    "amount": 16000
  }'

# 5. Close Auction
curl -X POST "https://localhost:5001/api/auctions/close?vehicleId=sedan-001" \
  -H "Authorization: Bearer $TOKEN"
```

### Example with PowerShell

```powershell
# 1. Login
$response = Invoke-RestMethod -Uri "https://localhost:5001/api/auth/login" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"username":"bidder1","password":"password123"}'

$token = $response.token

# 2. Add Vehicle
$headers = @{ "Authorization" = "Bearer $token" }
Invoke-RestMethod -Uri "https://localhost:5001/api/auctions/vehicles" `
  -Method POST `
  -Headers $headers `
  -ContentType "application/json" `
  -Body @{
    id = "sedan-001"
    type = "sedan"
    manufacturer = "Toyota"
    model = "Camry"
    year = 2023
    startingBid = 15000
    numberOfDoors = 4
  } | ConvertTo-Json
```

---

## 🧪 Testing

### Run All Tests

```bash
dotnet test
```

### Run Tests with Verbosity

```bash
dotnet test --verbosity=detailed
```

### Run Specific Test

```bash
dotnet test --filter "PlaceBid_WithValidBid_ShouldSucceed"
```

### Test Coverage

- ✅ **36 functionality tests**
- ✅ **2 integration tests**
- ✅ Success cases for each operation
- ✅ All error scenarios
- ✅ Edge cases (extreme values, invalid inputs, etc.)

#### Implemented Tests

**AddVehicle (9 tests)**

- ✅ Add valid Sedan/Hatchback/SUV/Truck
- ✅ Duplicate ID
- ✅ Null/empty ID
- ✅ Invalid year (≤ 1885)
- ✅ Negative/zero starting bid

**SearchVehicles (10 tests)**

- ✅ Search without filters
- ✅ Filter by type (case-insensitive)
- ✅ Filter by manufacturer (case-insensitive)
- ✅ Filter by model (case-insensitive)
- ✅ Filter by year
- ✅ Combined multiple filters
- ✅ No results found

**StartAuction (3 tests)**

- ✅ Start auction with valid vehicle
- ✅ Vehicle does not exist
- ✅ Auction already active

**PlaceBid (7 tests)**

- ✅ Valid bid
- ✅ Vehicle does not exist
- ✅ No active auction
- ✅ Bid below minimum
- ✅ Null/empty bidder
- ✅ Multiple successive bids
- ✅ Bid with exact minimum increment

**CloseAuction (5 tests)**

- ✅ Close active auction
- ✅ Vehicle does not exist
- ✅ No active auction
- ✅ Do not allow bid after closing
- ✅ Allow restarting auction after closing

---

## 📝 Logging

Logs are saved in `logs/` with daily rotation:

```
logs/
├── carauction-20231201.txt
├── carauction-20231202.txt
└── carauction-20231203.txt
```

### Log Example

```
2023-12-03 14:35:12.123 [INF] Starting Car Auction Management API...
2023-12-03 14:35:12.456 [INF] Car Auction Management API started successfully.
2023-12-03 14:35:45.789 [INF] User 'bidder1' logged in successfully.
2023-12-03 14:35:50.012 [INF] Vehicle 'sedan-001' added to inventory.
```

### Log Levels

- `DEBUG`: Detailed information for diagnostics
- `INFO`: General operation information
- `WARN`: Warnings for unusual situations
- `ERR`: Errors affecting functionality
- `FATAL`: Critical errors that may bring down the application

---

## 📂 Project Structure

### Domain Layer

```csharp
// Entities
public abstract class Vehicle { }
public class Sedan : Vehicle { }
public class Auction { }

// Exceptions
public abstract class DomainException : Exception { }
public class InvalidBidException : DomainException { }
public class DuplicateVehicleException : DomainException { }

// Interfaces (Ports)
public interface IVehicleRepository { }
public interface IAuctionRepository { }
```

### Application Layer

```csharp
// Services
public class AuctionService { }
public class JwtTokenService { }

// DTOs
public class PagedResult<T> { }
```

### Infrastructure Layer

```csharp
// DbContext
public class AuctionDbContext : DbContext { }

// Repositories
public class EfVehicleRepository : IVehicleRepository { }
public class EfAuctionRepository : IAuctionRepository { }
```

### API Layer

```csharp
// Controllers
public class AuctionsController : ControllerBase { }
public class AuthController : ControllerBase { }

// Validators
public class AddVehicleRequestValidator : AbstractValidator<AddVehicleRequest> { }
public class PlaceBidRequestValidator : AbstractValidator<PlaceBidRequest> { }

// Requests/Responses
public class AddVehicleRequest { }
public class VehicleResponse { }
```
