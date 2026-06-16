namespace BusTicketManagement.Domain.Entities
{
    public class Bus
    {
        public int BusId { get; set; }
        public string BusNo { get; set; }
        public string BusType { get; set; } // AC, NonAC
        public int Capacity { get; set; }
        public string RegistrationNo { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public int? YearOfManufacture { get; set; }
        public string Owner { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
