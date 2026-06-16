-- =====================================================
-- STORED PROCEDURE: Generate Booking Report
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GenerateBookingReport')
DROP PROCEDURE sp_GenerateBookingReport;
GO

CREATE PROCEDURE sp_GenerateBookingReport
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CAST(b.BookingDate AS DATE) AS BookingDate,
        COUNT(*) AS TotalBookings,
        SUM(CASE WHEN b.BookingStatus = 'Confirmed' THEN 1 ELSE 0 END) AS ConfirmedBookings,
        SUM(CASE WHEN b.BookingStatus = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledBookings,
        SUM(p.Amount) AS TotalRevenue,
        AVG(p.Amount) AS AverageRevenue
    FROM Bookings b
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    WHERE CAST(b.BookingDate AS DATE) BETWEEN @StartDate AND @EndDate
    GROUP BY CAST(b.BookingDate AS DATE)
    ORDER BY BookingDate DESC;
END;
GO

-- =====================================================
-- STORED PROCEDURE: Get Revenue by Route
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetRevenueByRoute')
DROP PROCEDURE sp_GetRevenueByRoute;
GO

CREATE PROCEDURE sp_GetRevenueByRoute
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.RouteId,
        r.SourceCity,
        r.DestinationCity,
        COUNT(b.BookingId) AS TotalBookings,
        SUM(p.Amount) AS TotalRevenue,
        AVG(p.Amount) AS AverageFare
    FROM Routes r
    LEFT JOIN Schedules s ON r.RouteId = s.RouteId
    LEFT JOIN Bookings b ON s.ScheduleId = b.ScheduleId
    LEFT JOIN Payments p ON b.BookingId = p.BookingId
    WHERE CAST(b.BookingDate AS DATE) BETWEEN @StartDate AND @EndDate
        OR (b.BookingId IS NULL AND CAST(s.DepartureTime AS DATE) BETWEEN @StartDate AND @EndDate)
    GROUP BY r.RouteId, r.SourceCity, r.DestinationCity
    ORDER BY TotalRevenue DESC;
END;
GO

PRINT 'Report stored procedures created successfully!';
GO
