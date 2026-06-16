namespace BusTicketManagement.Application.DTOs.Seat
{
    public class SeatDto
    {
        public int SeatId { get; set; }
        public int ScheduleId { get; set; }
        public int SeatNumber { get; set; }
        public string SeatStatus { get; set; }
    }

    public class ScheduleSeatsDto
    {
        public int ScheduleId { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<SeatDto> Seats { get; set; }
    }
}
