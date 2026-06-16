namespace BusTicketManagement.Domain.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public decimal Fare { get; set; }
        public string ScheduleStatus { get; set; } // Scheduled, InProgress, Completed, Cancelled
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
