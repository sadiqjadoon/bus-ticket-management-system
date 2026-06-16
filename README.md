# Bus Ticket Management System

## Production-Grade ASP.NET Core 8 Web API

### Technology Stack
- **Framework**: ASP.NET Core 8 Web API
- **Database**: SQL Server with ADO.NET (No Entity Framework)
- **Authentication**: ASP.NET Core Identity + JWT
- **Authorization**: Role-based + Permission-based
- **Architecture**: Clean Architecture (4 layers)
- **Data Access**: Repository Pattern + Unit of Work
- **Logging**: Serilog
- **Validation**: FluentValidation
- **API Documentation**: Swagger/OpenAPI

### Project Structure

```
BusTicketManagement/
├── src/
│   ├── Domain/                          # Core domain entities
│   ├── Application/                     # Business logic, services, DTOs
│   ├── Infrastructure/                  # Data access, repositories, external services
│   └── API/                             # ASP.NET Core Web API
├── Database/
│   ├── Schema/
│   │   └── InitialSchema.sql           # Database creation script
│   └── StoredProcedures/
│       ├── Auth/
│       ├── Users/
│       ├── Buses/
│       ├── Bookings/
│       └── Common/
└── Documentation/
    ├── API.postman_collection.json
    └── Architecture.md
```

### Core Features

#### 1. Authentication & Authorization
- User registration and login
- JWT token generation with refresh tokens
- Role-based access control (Admin, Staff, Customer)
- Permission-based authorization

#### 2. Bus Management
- CRUD operations for buses
- Bus types (AC, Non-AC)
- Capacity management

#### 3. Route Management
- Source and destination management
- Distance and duration tracking

#### 4. Schedule Management
- Bus-Route assignments
- Departure and arrival times
- Real-time seat availability

#### 5. Booking System
- Ticket booking with seat selection
- Transaction-safe double-booking prevention
- Booking cancellation

#### 6. Payment Processing (Mock)
- Payment status tracking
- Payment history

### API Endpoints

#### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token

#### Admin
- `GET /api/admin/users` - List users (paginated)
- `POST /api/admin/users` - Create user
- `PUT /api/admin/users/{id}` - Update user
- `DELETE /api/admin/users/{id}` - Delete user
- `POST /api/admin/assign-role` - Assign role to user
- `POST /api/admin/assign-permission` - Assign permission to role

#### Buses
- `GET /api/buses` - List buses (paginated)
- `POST /api/buses` - Create bus
- `PUT /api/buses/{id}` - Update bus
- `DELETE /api/buses/{id}` - Delete bus

#### Routes
- `GET /api/routes` - List routes
- `POST /api/routes` - Create route
- `PUT /api/routes/{id}` - Update route
- `DELETE /api/routes/{id}` - Delete route

#### Schedules
- `GET /api/schedules` - List schedules
- `POST /api/schedules` - Create schedule
- `GET /api/schedules/{id}/availability` - Check seat availability

#### Bookings
- `POST /api/bookings` - Create booking
- `GET /api/bookings/user/{userId}` - Get user bookings
- `POST /api/bookings/{id}/cancel` - Cancel booking
- `GET /api/bookings/{id}` - Get booking details

### Getting Started

#### Prerequisites
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code
- Postman (for API testing)

#### Setup Instructions

1. **Create Database**
   ```sql
   -- Execute InitialSchema.sql on SQL Server
   ```

2. **Configure Connection String**
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=BusTicketDB;User Id=sa;Password=YOUR_PASSWORD;"
     }
   }
   ```

3. **Run Application**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

4. **Access Swagger**
   - Navigate to `https://localhost:5001/swagger`

### Database Schema

Key tables:
- `AspNetUsers` - User accounts
- `AspNetRoles` - Roles (Admin, Staff, Customer)
- `RolePermissions` - Role-permission mappings
- `Buses` - Bus inventory
- `Routes` - Route information
- `Schedules` - Bus schedule assignments
- `Seats` - Seat inventory per schedule
- `Bookings` - Ticket bookings
- `Payments` - Payment records

### Response Model

All API responses follow this standard format:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": {},
  "errors": []
}
```

### Logging

Logging is configured with Serilog:
- Console output
- File output (`logs/log-.txt`)
- Structured logging for all requests
- Error tracking with full context

### Security Features

- JWT token-based authentication
- Role and permission-based authorization
- SQL injection prevention via parameterized queries
- Transaction management for data consistency
- Secure password hashing (Identity)
- CORS configuration

### Performance Considerations

- Parameterized SQL queries
- Proper indexing on frequently queried columns
- Connection pooling via ADO.NET
- Pagination for list endpoints
- Caching strategies (where applicable)

### Error Handling

- Global exception handling middleware
- Consistent error response format
- Detailed logging of exceptions
- User-friendly error messages

---

**Author**: Senior .NET Solution Architect  
**Version**: 1.0.0  
**Last Updated**: 2024
