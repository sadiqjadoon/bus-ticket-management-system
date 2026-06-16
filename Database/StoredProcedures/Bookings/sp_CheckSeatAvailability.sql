-- =====================================================
-- STORED PROCEDURE: Check Seat Availability
-- =====================================================
-- Purpose: Check if a seat is available for booking
-- Returns: 1 if available, 0 if not available

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CheckSeatAvailability')
DROP PROCEDURE sp_CheckSeatAvailability;
GO

CREATE PROCEDURE sp_CheckSeatAvailability
    @SeatId INT,
    @IsAvailable BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @IsAvailable = 0;
    
    IF EXISTS (SELECT 1 FROM Seats WHERE SeatId = @SeatId AND SeatStatus = 'Available')
    BEGIN
        SET @IsAvailable = 1;
    END
END;
GO

-- =====================================================
-- STORED PROCEDURE: Get Available Seats for Schedule
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAvailableSeats')
DROP PROCEDURE sp_GetAvailableSeats;
GO

CREATE PROCEDURE sp_GetAvailableSeats
    @ScheduleId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT SeatId, SeatNumber, SeatStatus
    FROM Seats
    WHERE ScheduleId = @ScheduleId
    ORDER BY SeatNumber;
END;
GO

-- =====================================================
-- STORED PROCEDURE: Book Ticket (Transaction-Safe)
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_BookTicket')
DROP PROCEDURE sp_BookTicket;
GO

CREATE PROCEDURE sp_BookTicket
    @BookingCode NVARCHAR(50),
    @UserId NVARCHAR(450),
    @ScheduleId INT,
    @SeatId INT,
    @PassengerName NVARCHAR(256),
    @PassengerEmail NVARCHAR(256),
    @PassengerPhone NVARCHAR(20),
    @BookingId INT OUTPUT,
    @IsSuccess BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @IsSuccess = 0;
    SET @BookingId = 0;
    
    BEGIN TRANSACTION
    BEGIN TRY
        -- Check if seat is available
        IF NOT EXISTS (SELECT 1 FROM Seats WHERE SeatId = @SeatId AND SeatStatus = 'Available')
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Lock seat and update status
        UPDATE Seats
        SET SeatStatus = 'Booked', UpdatedAt = GETUTCDATE()
        WHERE SeatId = @SeatId;
        
        -- Create booking
        INSERT INTO Bookings (BookingCode, UserId, ScheduleId, SeatId, PassengerName, PassengerEmail, PassengerPhone, BookingStatus)
        VALUES (@BookingCode, @UserId, @ScheduleId, @SeatId, @PassengerName, @PassengerEmail, @PassengerPhone, 'Pending');
        
        SET @BookingId = SCOPE_IDENTITY();
        
        -- Update available seats count
        UPDATE Schedules
        SET AvailableSeats = AvailableSeats - 1, UpdatedAt = GETUTCDATE()
        WHERE ScheduleId = @ScheduleId;
        
        -- Create pending payment
        DECLARE @Fare DECIMAL(10, 2);
        SELECT @Fare = Fare FROM Schedules WHERE ScheduleId = @ScheduleId;
        
        INSERT INTO Payments (BookingId, Amount, PaymentStatus)
        VALUES (@BookingId, @Fare, 'Pending');
        
        COMMIT TRANSACTION;
        SET @IsSuccess = 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @IsSuccess = 0;
    END CATCH
END;
GO

-- =====================================================
-- STORED PROCEDURE: Cancel Booking
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CancelBooking')
DROP PROCEDURE sp_CancelBooking;
GO

CREATE PROCEDURE sp_CancelBooking
    @BookingId INT,
    @CancellationReason NVARCHAR(500),
    @IsSuccess BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @IsSuccess = 0;
    
    BEGIN TRANSACTION
    BEGIN TRY
        -- Check if booking exists and is not already cancelled
        IF NOT EXISTS (SELECT 1 FROM Bookings WHERE BookingId = @BookingId AND BookingStatus != 'Cancelled')
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Get seat and schedule info
        DECLARE @SeatId INT, @ScheduleId INT;
        SELECT @SeatId = SeatId, @ScheduleId = ScheduleId FROM Bookings WHERE BookingId = @BookingId;
        
        -- Update booking status
        UPDATE Bookings
        SET BookingStatus = 'Cancelled', CancellationDate = GETUTCDATE(), CancellationReason = @CancellationReason, UpdatedAt = GETUTCDATE()
        WHERE BookingId = @BookingId;
        
        -- Release seat
        UPDATE Seats
        SET SeatStatus = 'Available', UpdatedAt = GETUTCDATE()
        WHERE SeatId = @SeatId;
        
        -- Update available seats count
        UPDATE Schedules
        SET AvailableSeats = AvailableSeats + 1, UpdatedAt = GETUTCDATE()
        WHERE ScheduleId = @ScheduleId;
        
        -- Update payment status to refunded
        UPDATE Payments
        SET PaymentStatus = 'Refunded', RefundDate = GETUTCDATE(), RefundReason = @CancellationReason, UpdatedAt = GETUTCDATE()
        WHERE BookingId = @BookingId AND PaymentStatus != 'Failed';
        
        COMMIT TRANSACTION;
        SET @IsSuccess = 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @IsSuccess = 0;
    END CATCH
END;
GO

PRINT 'Booking stored procedures created successfully!';
GO
