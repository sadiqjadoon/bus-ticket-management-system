-- =====================================================
-- BUS TICKET MANAGEMENT SYSTEM - SQL SERVER SCHEMA
-- =====================================================
-- Production-grade database with ADO.NET integration
-- No Entity Framework - Raw SQL implementation
-- =====================================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BusTicketDB')
CREATE DATABASE BusTicketDB;
GO

USE BusTicketDB;
GO

-- =====================================================
-- 1. IDENTITY TABLES
-- =====================================================

-- AspNetUsers Table (Extended Identity)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256) NOT NULL UNIQUE,
    NormalizedUserName NVARCHAR(256) UNIQUE,
    Email NVARCHAR(256) UNIQUE,
    NormalizedEmail NVARCHAR(256) UNIQUE,
    EmailConfirmed BIT DEFAULT 0,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    ConcurrencyStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(20),
    PhoneNumberConfirmed BIT DEFAULT 0,
    TwoFactorEnabled BIT DEFAULT 0,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT DEFAULT 1,
    AccessFailedCount INT DEFAULT 0,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    IsActive BIT DEFAULT 1,
    LastLoginAt DATETIME2 NULL
);
GO

-- AspNetRoles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles')
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL UNIQUE,
    NormalizedName NVARCHAR(256) UNIQUE,
    ConcurrencyStamp NVARCHAR(MAX),
    Description NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

-- AspNetUserRoles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserRoles')
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    AssignedAt DATETIME2 DEFAULT GETUTCDATE(),
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
GO

-- =====================================================
-- 2. PERMISSION TABLES
-- =====================================================

-- Permissions Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
CREATE TABLE Permissions (
    PermissionId INT PRIMARY KEY IDENTITY(1,1),
    PermissionCode NVARCHAR(100) NOT NULL UNIQUE,
    PermissionName NVARCHAR(256) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

-- RolePermissions Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePermissions')
CREATE TABLE RolePermissions (
    RolePermissionId INT PRIMARY KEY IDENTITY(1,1),
    RoleId NVARCHAR(450) NOT NULL,
    PermissionId INT NOT NULL,
    AssignedAt DATETIME2 DEFAULT GETUTCDATE(),
    UNIQUE (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId) ON DELETE CASCADE
);
GO

-- =====================================================
-- 3. BUSINESS TABLES
-- =====================================================

-- Buses Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Buses')
CREATE TABLE Buses (
    BusId INT PRIMARY KEY IDENTITY(1,1),
    BusNo NVARCHAR(50) NOT NULL UNIQUE,
    BusType NVARCHAR(20) NOT NULL CHECK (BusType IN ('AC', 'NonAC')),
    Capacity INT NOT NULL CHECK (Capacity > 0),
    RegistrationNo NVARCHAR(50) NOT NULL UNIQUE,
    ManufacturerName NVARCHAR(100),
    ModelName NVARCHAR(100),
    YearOfManufacture INT,
    Owner NVARCHAR(256),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

-- Routes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Routes')
CREATE TABLE Routes (
    RouteId INT PRIMARY KEY IDENTITY(1,1),
    SourceCity NVARCHAR(100) NOT NULL,
    DestinationCity NVARCHAR(100) NOT NULL,
    Distance DECIMAL(10, 2) NOT NULL CHECK (Distance > 0),
    Duration INT NOT NULL CHECK (Duration > 0),
    BaseFare DECIMAL(10, 2) NOT NULL CHECK (BaseFare > 0),
    RouteCode NVARCHAR(50) NOT NULL UNIQUE,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

-- Schedules Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Schedules')
CREATE TABLE Schedules (
    ScheduleId INT PRIMARY KEY IDENTITY(1,1),
    BusId INT NOT NULL,
    RouteId INT NOT NULL,
    DepartureTime DATETIME2 NOT NULL,
    ArrivalTime DATETIME2 NOT NULL,
    AvailableSeats INT NOT NULL,
    TotalSeats INT NOT NULL,
    Fare DECIMAL(10, 2) NOT NULL CHECK (Fare > 0),
    ScheduleStatus NVARCHAR(20) DEFAULT 'Scheduled' CHECK (ScheduleStatus IN ('Scheduled', 'InProgress', 'Completed', 'Cancelled')),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (BusId) REFERENCES Buses(BusId),
    FOREIGN KEY (RouteId) REFERENCES Routes(RouteId),
    CHECK (ArrivalTime > DepartureTime)
);
GO

-- Seats Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Seats')
CREATE TABLE Seats (
    SeatId INT PRIMARY KEY IDENTITY(1,1),
    ScheduleId INT NOT NULL,
    SeatNumber INT NOT NULL,
    SeatStatus NVARCHAR(20) DEFAULT 'Available' CHECK (SeatStatus IN ('Available', 'Booked', 'Blocked')),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId) ON DELETE CASCADE,
    UNIQUE (ScheduleId, SeatNumber)
);
GO

-- Bookings Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookings')
CREATE TABLE Bookings (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    BookingCode NVARCHAR(50) NOT NULL UNIQUE,
    UserId NVARCHAR(450) NOT NULL,
    ScheduleId INT NOT NULL,
    SeatId INT NOT NULL,
    PassengerName NVARCHAR(256) NOT NULL,
    PassengerEmail NVARCHAR(256),
    PassengerPhone NVARCHAR(20),
    BookingStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (BookingStatus IN ('Pending', 'Confirmed', 'Cancelled')),
    BookingDate DATETIME2 DEFAULT GETUTCDATE(),
    CancellationDate DATETIME2 NULL,
    CancellationReason NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId),
    FOREIGN KEY (SeatId) REFERENCES Seats(SeatId)
);
GO

-- Payments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments')
CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    BookingId INT NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL CHECK (Amount > 0),
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Paid', 'Failed', 'Refunded')),
    PaymentMethod NVARCHAR(50),
    TransactionId NVARCHAR(100),
    PaymentDate DATETIME2 NULL,
    RefundDate DATETIME2 NULL,
    RefundReason NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId) ON DELETE CASCADE
);
GO

-- =====================================================
-- 4. AUDIT & LOGGING TABLES
-- =====================================================

-- AuditLog Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLog')
CREATE TABLE AuditLog (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450),
    Action NVARCHAR(256),
    EntityName NVARCHAR(100),
    EntityId NVARCHAR(100),
    OldValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    Timestamp DATETIME2 DEFAULT GETUTCDATE(),
    IPAddress NVARCHAR(45)
);
GO

-- =====================================================
-- 5. INDEXES FOR PERFORMANCE
-- =====================================================

CREATE INDEX IX_AspNetUsers_Email ON AspNetUsers(Email);
CREATE INDEX IX_AspNetUsers_IsActive ON AspNetUsers(IsActive);
CREATE INDEX IX_Buses_IsActive ON Buses(IsActive);
CREATE INDEX IX_Routes_IsActive ON Routes(IsActive);
CREATE INDEX IX_Schedules_BusId ON Schedules(BusId);
CREATE INDEX IX_Schedules_RouteId ON Schedules(RouteId);
CREATE INDEX IX_Schedules_DepartureTime ON Schedules(DepartureTime);
CREATE INDEX IX_Seats_ScheduleId_Status ON Seats(ScheduleId, SeatStatus);
CREATE INDEX IX_Bookings_UserId ON Bookings(UserId);
CREATE INDEX IX_Bookings_ScheduleId ON Bookings(ScheduleId);
CREATE INDEX IX_Bookings_BookingStatus ON Bookings(BookingStatus);
CREATE INDEX IX_Bookings_BookingCode ON Bookings(BookingCode);
CREATE INDEX IX_Payments_BookingId ON Payments(BookingId);
CREATE INDEX IX_Payments_PaymentStatus ON Payments(PaymentStatus);
GO

-- =====================================================
-- 6. SEED INITIAL DATA
-- =====================================================

-- Insert Default Roles
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
INSERT INTO AspNetRoles (Id, Name, NormalizedName, Description) 
VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Admin', 'ADMIN', 'Administrator role with full system access');

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Staff')
INSERT INTO AspNetRoles (Id, Name, NormalizedName, Description) 
VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Staff', 'STAFF', 'Staff role with operational access');

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Customer')
INSERT INTO AspNetRoles (Id, Name, NormalizedName, Description) 
VALUES (CAST(NEWID() AS NVARCHAR(450)), 'Customer', 'CUSTOMER', 'Customer role for ticket booking');
GO

-- Insert Default Permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'CREATE_BUS')
INSERT INTO Permissions (PermissionCode, PermissionName, Description)
VALUES 
    ('CREATE_BUS', 'Create Bus', 'Permission to create new buses'),
    ('UPDATE_BUS', 'Update Bus', 'Permission to update bus information'),
    ('DELETE_BUS', 'Delete Bus', 'Permission to delete buses'),
    ('VIEW_BUS', 'View Bus', 'Permission to view bus information'),
    ('BOOK_TICKET', 'Book Ticket', 'Permission to book tickets'),
    ('CANCEL_TICKET', 'Cancel Ticket', 'Permission to cancel bookings'),
    ('VIEW_REPORTS', 'View Reports', 'Permission to view system reports'),
    ('MANAGE_USERS', 'Manage Users', 'Permission to manage user accounts'),
    ('ASSIGN_ROLES', 'Assign Roles', 'Permission to assign roles to users'),
    ('MANAGE_ROUTES', 'Manage Routes', 'Permission to manage routes'),
    ('MANAGE_SCHEDULES', 'Manage Schedules', 'Permission to manage schedules'),
    ('VIEW_PAYMENTS', 'View Payments', 'Permission to view payment information');
GO

PRINT 'Database schema created successfully!';
GO
