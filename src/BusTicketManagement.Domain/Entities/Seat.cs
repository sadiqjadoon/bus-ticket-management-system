namespace BusTicketManagement.Domain.Entities
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int ScheduleId { get; set; }
        public int SeatNumber { get; set; }
        public string SeatStatus { get; set; } // Available, Booked, Blocked
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
