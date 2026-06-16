namespace BusTicketManagement.Domain.Entities
{
    public class Route
    {
        public int RouteId { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal Distance { get; set; }
        public int Duration { get; set; } // in minutes
        public decimal BaseFare { get; set; }
        public string RouteCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
