namespace BusTicketManagement.Domain.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public string UserId { get; set; }
        public int ScheduleId { get; set; }
        public int SeatId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public string PassengerPhone { get; set; }
        public string BookingStatus { get; set; } // Pending, Confirmed, Cancelled
        public DateTime BookingDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
