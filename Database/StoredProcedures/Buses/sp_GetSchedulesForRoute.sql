-- =====================================================
-- STORED PROCEDURE: Get Schedules for Route
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetSchedulesForRoute')
DROP PROCEDURE sp_GetSchedulesForRoute;
GO

CREATE PROCEDURE sp_GetSchedulesForRoute
    @SourceCity NVARCHAR(100),
    @DestinationCity NVARCHAR(100),
    @TravelDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.ScheduleId,
        s.BusId,
        b.BusNo,
        b.BusType,
        b.Capacity,
        s.DepartureTime,
        s.ArrivalTime,
        s.AvailableSeats,
        s.Fare,
        r.RouteId,
        r.SourceCity,
        r.DestinationCity,
        r.Distance,
        r.Duration
    FROM Schedules s
    INNER JOIN Buses b ON s.BusId = b.BusId
    INNER JOIN Routes r ON s.RouteId = r.RouteId
    WHERE r.SourceCity = @SourceCity 
        AND r.DestinationCity = @DestinationCity
        AND CAST(s.DepartureTime AS DATE) = @TravelDate
        AND s.ScheduleStatus = 'Scheduled'
        AND s.AvailableSeats > 0
    ORDER BY s.DepartureTime;
END;
GO

-- =====================================================
-- STORED PROCEDURE: Get Bus Statistics
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetBusStatistics')
DROP PROCEDURE sp_GetBusStatistics;
GO

CREATE PROCEDURE sp_GetBusStatistics
    @BusId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        b.BusId,
        b.BusNo,
        b.BusType,
        b.Capacity,
        COALESCE(COUNT(s.ScheduleId), 0) AS TotalSchedules,
        COALESCE(SUM(CASE WHEN s.ScheduleStatus = 'Completed' THEN 1 ELSE 0 END), 0) AS CompletedSchedules,
        COALESCE(SUM(b.Capacity - s.AvailableSeats), 0) AS TotalBookings
    FROM Buses b
    LEFT JOIN Schedules s ON b.BusId = s.BusId
    WHERE b.BusId = @BusId
    GROUP BY b.BusId, b.BusNo, b.BusType, b.Capacity;
END;
GO

PRINT 'Bus stored procedures created successfully!';
GO
