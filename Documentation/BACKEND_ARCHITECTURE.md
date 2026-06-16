# ASP.NET Core 3.1 Backend - Architecture & Setup Guide

## Project Architecture

### Clean Architecture Layers

```
BusTicketManagement.API/
├── BusTicketManagement.Domain/              # Layer 1: Core Business Logic
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Bus.cs
│   │   ├── Route.cs
│   │   ├── Schedule.cs
│   │   ├── Booking.cs
│   │   ├── Payment.cs
│   │   └── Seat.cs
│   └── Enums/
│       ├── BusType.cs
│       ├── BookingStatus.cs
│       ├── PaymentStatus.cs
│       └── SeatStatus.cs
│
├── BusTicketManagement.Application/         # Layer 2: Business Services
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   ├── IAuthService.cs
│   │   ├── IBookingService.cs
│   │   ├── IBusService.cs
│   │   └── ...
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── Bus/
│   │   ├── Booking/
│   │   └── ...
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── BookingService.cs
│   │   ├── BusService.cs
│   │   └── ...
│   ├── Validators/
│   │   ├── RegisterDtoValidator.cs
│   │   ├── CreateBusDtoValidator.cs
│   │   └── ...
│   └── Mapping/
│       └── MappingProfile.cs
│
├── BusTicketManagement.Infrastructure/      # Layer 3: Data Access
│   ├── Data/
│   │   ├── DbConnectionFactory.cs
│   │   └── AppDbContext.cs (for reference)
│   ├── Repositories/
│   │   ├── BaseRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── BusRepository.cs
│   │   ├── BookingRepository.cs
│   │   └── ...
│   ├── UnitOfWork/
│   │   └── UnitOfWork.cs
│   └── External/
│       └── PaymentGateway.cs
│
├── BusTicketManagement.API/                 # Layer 4: REST API
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── BusesController.cs
│   │   ├── BookingsController.cs
│   │   ├── AdminController.cs
│   │   └── ...
│   ├── Middleware/
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── RequestLoggingMiddleware.cs
│   │   └── JwtMiddleware.cs
│   ├── Filters/
│   │   ├── AuthorizeFilter.cs
│   │   └── ValidationFilter.cs
│   ├── Helpers/
│   │   ├── JwtTokenGenerator.cs
│   │   ├── EncryptionHelper.cs
│   │   └── ResponseFormatter.cs
│   ├── appsettings.json
│   ├── Program.cs
│   └── Startup.cs
│
└── BusTicketManagement.Tests/               # Layer 5: Unit Tests
    ├── ServiceTests/
    ├── RepositoryTests/
    └── ControllerTests/
```

## Layer Responsibilities

### Domain Layer
- **Purpose**: Pure business entities and value objects
- **No Dependencies**: No references to infrastructure or application
- **Contains**: Entity models with minimal logic
- **Example**:
  ```csharp
  public class Bus
  {
      public int BusId { get; set; }
      public string BusNo { get; set; }
      public BusType Type { get; set; }
      public int Capacity { get; set; }
  }
  ```

### Application Layer
- **Purpose**: Business logic and use cases
- **Dependencies**: Domain, external interfaces
- **Contains**: Services, DTOs, Validators, Mappers
- **No Database Access**: Uses interfaces (Dependency Injection)
- **Example**:
  ```csharp
  public class BusService : IBusService
  {
      private readonly IUnitOfWork _unitOfWork;
      private readonly IMapper _mapper;
      
      public async Task<BusDto> CreateBusAsync(CreateBusDto dto)
      {
          // Business logic here
      }
  }
  ```

### Infrastructure Layer
- **Purpose**: Data access and external service integration
- **Dependencies**: Application interfaces
- **Contains**: Repositories, ADO.NET code, UnitOfWork
- **Database Access**: Direct SQL queries via ADO.NET
- **Example**:
  ```csharp
  public class BusRepository : BaseRepository<Bus>
  {
      public async Task<Bus> GetByIdAsync(int id)
      {
          using (SqlConnection conn = _connectionFactory.GetConnection())
          {
              using (SqlCommand cmd = new SqlCommand("SELECT * FROM Buses WHERE BusId = @Id", conn))
              {
                  cmd.Parameters.AddWithValue("@Id", id);
                  // Execute query
              }
          }
      }
  }
  ```

### API Layer
- **Purpose**: HTTP endpoints and request/response handling
- **Dependencies**: Application services
- **Contains**: Controllers, Middleware, Filters
- **No Business Logic**: Delegates to services
- **Example**:
  ```csharp
  [ApiController]
  [Route("api/[controller]")]
  public class BusesController : ControllerBase
  {
      private readonly IBusService _busService;
      
      [HttpPost]
      [Authorize(Roles = "Admin")]
      public async Task<IActionResult> CreateBus(CreateBusDto dto)
      {
          var result = await _busService.CreateBusAsync(dto);
          return Ok(result);
      }
  }
  ```

## Technology Stack

| Layer | Technology | Version |
|-------|-----------|----------|
| **Framework** | ASP.NET Core | 3.1 |
| **Database** | SQL Server | 2019+ |
| **Data Access** | ADO.NET | .NET Native |
| **Authentication** | JWT | Custom Implementation |
| **Logging** | Serilog | Latest |
| **Validation** | FluentValidation | Latest |
| **Mapping** | AutoMapper | Latest |
| **API Documentation** | Swagger/Swashbuckle | Latest |
| **Testing** | xUnit + Moq | Latest |

## Authentication & Authorization

### JWT Token Flow

```
1. User Login (POST /api/auth/login)
   ↓
2. Validate credentials against AspNetUsers
   ↓
3. Get user roles (sp_GetUserRoles)
   ↓
4. Get user permissions (sp_GetUserPermissions)
   ↓
5. Generate JWT token with claims
   ├── userId
   ├── email
   ├── roles ["Admin", "Staff"]
   └── permissions ["CREATE_BUS", "VIEW_REPORTS"]
   ↓
6. Return token to client
   ↓
7. Client includes token in Authorization header
   ↓
8. Server validates token signature
   ↓
9. Extract claims and authorize request
```

### JWT Token Structure

```json
{
  "sub": "user-id-123",
  "email": "user@example.com",
  "roles": ["Admin", "Staff"],
  "permissions": ["CREATE_BUS", "UPDATE_BUS"],
  "exp": 1705330800,
  "iat": 1705244400
}
```

### Authorization Decorators

```csharp
// Role-based
[Authorize(Roles = "Admin")]

// Permission-based
[Authorize(Policy = "CREATE_BUS_PERMISSION")]

// Multiple requirements
[Authorize(Roles = "Admin,Staff")]

// No authentication required
[AllowAnonymous]
```

## ADO.NET Implementation

### DbConnectionFactory

```csharp
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }
    
    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
```

### Base Repository Pattern

```csharp
public abstract class BaseRepository<T> where T : class
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected readonly string _tableName;
    
    protected BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public virtual async Task<T> GetByIdAsync(int id)
    {
        using (SqlConnection conn = _connectionFactory.GetConnection())
        {
            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {_tableName} WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapFromReader(reader);
                    }
                }
            }
        }
        return null;
    }
    
    protected abstract T MapFromReader(SqlDataReader reader);
}
```

### Unit of Work Pattern

```csharp
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnectionFactory _connectionFactory;
    
    public IUserRepository Users { get; }
    public IBusRepository Buses { get; }
    public IBookingRepository Bookings { get; }
    public IPaymentRepository Payments { get; }
    
    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        Users = new UserRepository(_connectionFactory);
        Buses = new BusRepository(_connectionFactory);
        Bookings = new BookingRepository(_connectionFactory);
        Payments = new PaymentRepository(_connectionFactory);
    }
    
    public async Task<bool> SaveChangesAsync()
    {
        // ADO.NET doesn't require SaveChanges
        // Changes are immediate via ExecuteNonQuery()
        return await Task.FromResult(true);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
```

## Standard Response Model

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int? StatusCode { get; set; }
}

// Usage
return Ok(new ApiResponse<BusDto>
{
    Success = true,
    Message = "Bus created successfully",
    Data = busDto,
    StatusCode = 200
});
```

## Global Exception Handling

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "An error occurred",
            Errors = new List<string> { exception.Message }
        };
        
        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return context.Response.WriteAsJsonAsync(response);
    }
}
```

## Dependency Injection Setup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Database
    services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    
    // Repositories
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IBusRepository, BusRepository>();
    services.AddScoped<IBookingRepository, BookingRepository>();
    
    // Services
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IBusService, BusService>();
    services.AddScoped<IBookingService, BookingService>();
    
    // AutoMapper
    services.AddAutoMapper(typeof(MappingProfile));
    
    // FluentValidation
    services.AddFluentValidation(config => 
        config.RegisterValidatorsFromAssemblyContaining<RegisterDtoValidator>());
    
    // JWT
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => 
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    
    // Swagger
    services.AddSwaggerGen(c => 
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bus Ticket API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
    });
}
```

## Error Handling

### Validation Errors

```csharp
// FluentValidation exception handler
[ApiExceptionFilter]
public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            var response = new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationException.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList()
            };
            
            context.Result = new BadRequestObjectResult(response);
        }
    }
}
```

## Logging Configuration

```csharp
// Serilog setup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Usage in services
private readonly ILogger<BusService> _logger;

public async Task<BusDto> CreateBusAsync(CreateBusDto dto)
{
    _logger.LogInformation("Creating bus with number {BusNo}", dto.BusNo);
    // ...
    _logger.LogError(ex, "Error creating bus");
}
```

## Development Setup

### Prerequisites
- .NET Core SDK 3.1 or later
- SQL Server 2019 or later
- Visual Studio 2019+ or VS Code

### Installation

```bash
# Clone repository
git clone https://github.com/sadiqjadoon/bus-ticket-management-system.git

# Navigate to backend
cd src/BusTicketManagement.API

# Restore dependencies
dotnet restore

# Update connection string in appsettings.json
# Run database migrations
dotnet ef database update

# Run application
dotnet run

# Access Swagger
# https://localhost:5001/swagger
```

### Configuration

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BusTicketDB;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "ExpirationMinutes": 60
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

## API Endpoints Overview

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/refresh` - Refresh JWT token

### Buses
- `GET /api/buses` - List all buses
- `POST /api/buses` - Create bus (Admin only)
- `PUT /api/buses/{id}` - Update bus (Admin only)
- `DELETE /api/buses/{id}` - Delete bus (Admin only)

### Bookings
- `POST /api/bookings` - Create booking (Customer)
- `GET /api/bookings/user/{userId}` - Get user bookings
- `POST /api/bookings/{id}/cancel` - Cancel booking

### Admin
- `GET /api/admin/users` - List users (Admin)
- `POST /api/admin/assign-role` - Assign role (Admin)
- `POST /api/admin/assign-permission` - Assign permission (Admin)

---

**Version**: 1.0  
**Framework**: ASP.NET Core 3.1  
**Last Updated**: 2024
