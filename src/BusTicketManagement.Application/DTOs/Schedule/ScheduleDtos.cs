namespace BusTicketManagement.Application.DTOs.Schedule
{
    public class CreateScheduleDto
    {
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
    }

    public class UpdateScheduleDto
    {
        public int ScheduleId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public string ScheduleStatus { get; set; }
    }

    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public decimal Fare { get; set; }
        public string ScheduleStatus { get; set; }
        public BusInfoDto Bus { get; set; }
        public RouteInfoDto Route { get; set; }
    }

    public class BusInfoDto
    {
        public int BusId { get; set; }
        public string BusNo { get; set; }
        public string BusType { get; set; }
        public int Capacity { get; set; }
    }

    public class RouteInfoDto
    {
        public int RouteId { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal Distance { get; set; }
        public int Duration { get; set; }
    }

    public class SearchScheduleDto
    {
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public DateTime TravelDate { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
