# Database Schema Documentation

## Overview
This document provides comprehensive documentation for the Bus Ticket Management System database schema. The database is designed using SQL Server with ADO.NET implementation (No Entity Framework).

## Database: BusTicketDB

### Table Structure

#### 1. Identity & Authentication Tables

##### AspNetUsers
- **Purpose**: Stores user account information
- **Primary Key**: Id (NVARCHAR(450))
- **Key Columns**:
  - `Id`: Unique identifier (GUID format)
  - `UserName`: Unique username for login
  - `Email`: User email address
  - `PasswordHash`: Hashed password (ASP.NET Core Identity)
  - `FirstName`, `LastName`: User personal information
  - `IsActive`: Flag to soft-delete users
  - `LastLoginAt`: Track user activity
  - `CreatedAt`, `UpdatedAt`: Audit timestamps

**Indexes**:
- `IX_AspNetUsers_Email`: For email-based lookups
- `IX_AspNetUsers_IsActive`: For active user queries

##### AspNetRoles
- **Purpose**: Stores role definitions
- **Primary Key**: Id (NVARCHAR(450))
- **Key Columns**:
  - `Name`: Role name (Admin, Staff, Customer)
  - `Description`: Role description

##### AspNetUserRoles
- **Purpose**: Junction table for user-role relationships
- **Primary Key**: (UserId, RoleId) - Composite
- **Foreign Keys**:
  - `UserId` → AspNetUsers(Id)
  - `RoleId` → AspNetRoles(Id)
- **Cascade Delete**: Yes (both)

#### 2. Permission Tables

##### Permissions
- **Purpose**: Stores permission definitions
- **Primary Key**: PermissionId (INT IDENTITY)
- **Key Columns**:
  - `PermissionCode`: Unique permission identifier (e.g., 'CREATE_BUS')
  - `PermissionName`: Human-readable name
  - `Description`: Permission details

**Sample Permissions**:
- CREATE_BUS, UPDATE_BUS, DELETE_BUS, VIEW_BUS
- BOOK_TICKET, CANCEL_TICKET
- VIEW_REPORTS
- MANAGE_USERS, ASSIGN_ROLES
- MANAGE_ROUTES, MANAGE_SCHEDULES
- VIEW_PAYMENTS

##### RolePermissions
- **Purpose**: Junction table for role-permission mappings
- **Primary Key**: RolePermissionId (INT IDENTITY)
- **Unique Constraint**: (RoleId, PermissionId)
- **Foreign Keys**:
  - `RoleId` → AspNetRoles(Id)
  - `PermissionId` → Permissions(PermissionId)

#### 3. Business Domain Tables

##### Buses
- **Purpose**: Stores bus inventory
- **Primary Key**: BusId (INT IDENTITY)
- **Key Columns**:
  - `BusNo`: Unique bus number (operational identifier)
  - `BusType`: 'AC' or 'NonAC'
  - `Capacity`: Number of seats (CHECK > 0)
  - `RegistrationNo`: Government registration
  - `ManufacturerName`, `ModelName`: Vehicle details
  - `YearOfManufacture`: Production year
  - `Owner`: Bus owner name
  - `IsActive`: Soft delete flag

**Indexes**:
- `IX_Buses_IsActive`: For active bus queries

##### Routes
- **Purpose**: Stores route definitions
- **Primary Key**: RouteId (INT IDENTITY)
- **Key Columns**:
  - `SourceCity`: Departure city
  - `DestinationCity`: Arrival city
  - `Distance`: Distance in km (DECIMAL)
  - `Duration`: Journey duration in minutes (INT)
  - `BaseFare`: Base fare amount
  - `RouteCode`: Unique route code
  - `IsActive`: Soft delete flag

**Business Rules**:
- Distance > 0
- Duration > 0
- BaseFare > 0

**Indexes**:
- `IX_Routes_IsActive`: For active route queries

##### Schedules
- **Purpose**: Stores bus-route assignments with timings
- **Primary Key**: ScheduleId (INT IDENTITY)
- **Key Columns**:
  - `BusId`: FK to Buses(BusId)
  - `RouteId`: FK to Routes(RouteId)
  - `DepartureTime`: Start time (DATETIME2)
  - `ArrivalTime`: End time (DATETIME2)
  - `AvailableSeats`: Current available seat count
  - `TotalSeats`: Total capacity
  - `Fare`: Ticket price
  - `ScheduleStatus`: 'Scheduled', 'InProgress', 'Completed', 'Cancelled'

**Business Rules**:
- ArrivalTime > DepartureTime
- Fare > 0

**Indexes**:
- `IX_Schedules_BusId`: For bus queries
- `IX_Schedules_RouteId`: For route queries
- `IX_Schedules_DepartureTime`: For time-based searches

##### Seats
- **Purpose**: Stores individual seat information per schedule
- **Primary Key**: SeatId (INT IDENTITY)
- **Key Columns**:
  - `ScheduleId`: FK to Schedules(ScheduleId)
  - `SeatNumber`: Seat position (1 to Capacity)
  - `SeatStatus`: 'Available', 'Booked', 'Blocked'

**Unique Constraint**: (ScheduleId, SeatNumber)

**Cascade Delete**: Yes (when schedule deleted)

**Indexes**:
- `IX_Seats_ScheduleId_Status`: For availability queries

##### Bookings
- **Purpose**: Stores ticket booking records
- **Primary Key**: BookingId (INT IDENTITY)
- **Key Columns**:
  - `BookingCode`: Unique booking reference (e.g., 'BK20240115001')
  - `UserId`: FK to AspNetUsers(Id)
  - `ScheduleId`: FK to Schedules(ScheduleId)
  - `SeatId`: FK to Seats(SeatId)
  - `PassengerName`: Name of ticket holder
  - `PassengerEmail`: Contact email
  - `PassengerPhone`: Contact phone
  - `BookingStatus`: 'Pending', 'Confirmed', 'Cancelled'
  - `BookingDate`: When booking was made
  - `CancellationDate`: When booking was cancelled
  - `CancellationReason`: Reason for cancellation

**Unique Constraint**: BookingCode

**Foreign Keys**:
- `UserId` → AspNetUsers(Id)
- `ScheduleId` → Schedules(ScheduleId)
- `SeatId` → Seats(SeatId)

**Indexes**:
- `IX_Bookings_UserId`: For user booking history
- `IX_Bookings_ScheduleId`: For schedule bookings
- `IX_Bookings_BookingStatus`: For status queries
- `IX_Bookings_BookingCode`: For quick lookup

##### Payments
- **Purpose**: Stores payment transaction records
- **Primary Key**: PaymentId (INT IDENTITY)
- **Key Columns**:
  - `BookingId`: FK to Bookings(BookingId)
  - `Amount`: Payment amount (DECIMAL 10,2)
  - `PaymentStatus`: 'Pending', 'Paid', 'Failed', 'Refunded'
  - `PaymentMethod`: Payment method (e.g., 'CreditCard', 'Debit')
  - `TransactionId`: External transaction reference
  - `PaymentDate`: When payment was made
  - `RefundDate`: When refund was processed
  - `RefundReason`: Reason for refund

**Business Rules**:
- Amount > 0

**Cascade Delete**: Yes (when booking deleted)

**Indexes**:
- `IX_Payments_BookingId`: For booking payments
- `IX_Payments_PaymentStatus`: For payment status queries

#### 4. Audit & Logging

##### AuditLog
- **Purpose**: Tracks all system changes for compliance
- **Primary Key**: AuditId (INT IDENTITY)
- **Key Columns**:
  - `UserId`: Who made the change
  - `Action`: Operation performed (CREATE, UPDATE, DELETE)
  - `EntityName`: Table affected
  - `EntityId`: Record ID
  - `OldValue`: Previous value (NVARCHAR(MAX))
  - `NewValue`: New value (NVARCHAR(MAX))
  - `Timestamp`: When change occurred
  - `IPAddress`: User's IP address

### Stored Procedures

#### Authentication Procedures

**sp_ValidateUserLogin**
```sql
EXEC sp_ValidateUserLogin 
    @UserName = 'john.doe',
    @Id = @userId OUTPUT,
    @Email = @email OUTPUT,
    @PasswordHash = @hash OUTPUT,
    @FirstName = @firstName OUTPUT,
    @LastName = @lastName OUTPUT,
    @IsActive = @isActive OUTPUT;
```
- Returns user credentials for login validation
- Automatically updates LastLoginAt timestamp

**sp_GetUserRoles**
- Returns all roles assigned to a user
- Used to populate JWT claims

**sp_GetUserPermissions**
- Returns all permissions for a user (via roles)
- Used for authorization checks

#### Booking Procedures

**sp_BookTicket**
```sql
EXEC sp_BookTicket
    @BookingCode = 'BK20240115001',
    @UserId = 'user123',
    @ScheduleId = 5,
    @SeatId = 12,
    @PassengerName = 'John Doe',
    @PassengerEmail = 'john@example.com',
    @PassengerPhone = '03001234567',
    @BookingId = @bookingId OUTPUT,
    @IsSuccess = @success OUTPUT;
```
- **Transaction-Safe**: Uses SQL transaction
- **Double-Booking Prevention**: Checks seat availability before booking
- **Atomic Operations**:
  1. Validates seat availability
  2. Updates seat status to 'Booked'
  3. Creates booking record
  4. Updates schedule available seats count
  5. Creates pending payment record
- **Rollback on Error**: Entire transaction rolls back if any step fails

**sp_CancelBooking**
- Cancels booking with transaction safety
- Releases seat back to available status
- Updates payment status to 'Refunded'
- Logs cancellation reason

#### Schedule Procedures

**sp_GetSchedulesForRoute**
```sql
EXEC sp_GetSchedulesForRoute
    @SourceCity = 'Karachi',
    @DestinationCity = 'Lahore',
    @TravelDate = '2024-01-15';
```
- Returns all available schedules for a route on a date
- Only returns scheduled (not cancelled) journeys
- Only returns journeys with available seats

**sp_GetBusStatistics**
- Returns statistics for a bus
- Shows total schedules, completed trips, and total bookings

#### User Procedures

**sp_GetAllUsers**
```sql
EXEC sp_GetAllUsers
    @PageNumber = 1,
    @PageSize = 10,
    @SearchTerm = 'john';
```
- Returns paginated list of users
- Supports search by username, email, or name
- Returns user roles as comma-separated string

**sp_GetUsersByRole**
- Returns users with a specific role
- Supports pagination

#### Reporting Procedures

**sp_GenerateBookingReport**
```sql
EXEC sp_GenerateBookingReport
    @StartDate = '2024-01-01',
    @EndDate = '2024-01-31';
```
- Daily booking statistics
- Shows confirmed and cancelled bookings
- Calculates revenue metrics

**sp_GetRevenueByRoute**
- Revenue analysis by route
- Shows total bookings and average fare
- Date range filtering

### Data Integrity Rules

1. **Referential Integrity**
   - All foreign keys have constraints
   - Cascade delete where appropriate
   - No orphaned records possible

2. **Business Rules**
   - Distances, durations, and fares must be positive
   - Arrival time must be after departure time
   - Seat numbers must be unique per schedule
   - Booking codes must be unique

3. **Audit Trail**
   - All changes logged to AuditLog
   - CreatedAt and UpdatedAt on all business entities
   - User tracking via UserId and IPAddress

### Performance Optimization

1. **Indexes Created**:
   - Email lookups on users
   - Status-based queries
   - Date range queries on schedules
   - Composite indexes for complex searches

2. **Connection Pooling**
   - Configured in ADO.NET connection string
   - Default pool size: 100 connections

3. **Parameterized Queries**
   - All ADO.NET queries use parameters
   - Prevention of SQL injection
   - Query plan caching

### Backup & Recovery

- Full backups: Daily at 2 AM UTC
- Transaction log backups: Every 15 minutes
- Retention: 30 days for full backups
- Testing: Weekly restore tests

### Security

- **Authentication**: Integrated Windows Authentication for server
- **Application Login**: Restricted permissions (no schema modification)
- **Encryption**: TDE (Transparent Data Encryption) enabled
- **Audit**: All changes logged with user and IP address
- **Data Masking**: PII fields masked in audit logs

---

**Version**: 1.0  
**Last Updated**: 2024  
**Maintainer**: Database Administration Team
