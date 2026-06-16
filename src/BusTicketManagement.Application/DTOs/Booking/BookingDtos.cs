namespace BusTicketManagement.Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        public int ScheduleId { get; set; }
        public int SeatId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public string PassengerPhone { get; set; }
    }

    public class BookingDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public int ScheduleId { get; set; }
        public int SeatId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerEmail { get; set; }
        public string PassengerPhone { get; set; }
        public string BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public ScheduleDto Schedule { get; set; }
        public PaymentDto Payment { get; set; }
    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class CancelBookingDto
    {
        public int BookingId { get; set; }
        public string CancellationReason { get; set; }
    }
}
