namespace BusTicketManagement.Application.DTOs.Route
{
    public class CreateRouteDto
    {
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal Distance { get; set; }
        public int Duration { get; set; }
        public decimal BaseFare { get; set; }
    }

    public class UpdateRouteDto
    {
        public int RouteId { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal Distance { get; set; }
        public int Duration { get; set; }
        public decimal BaseFare { get; set; }
        public bool IsActive { get; set; }
    }

    public class RouteDto
    {
        public int RouteId { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal Distance { get; set; }
        public int Duration { get; set; }
        public decimal BaseFare { get; set; }
        public string RouteCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
