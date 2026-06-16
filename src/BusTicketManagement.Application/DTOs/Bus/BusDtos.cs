namespace BusTicketManagement.Application.DTOs.Bus
{
    public class CreateBusDto
    {
        public string BusNo { get; set; }
        public string BusType { get; set; } // AC, NonAC
        public int Capacity { get; set; }
        public string RegistrationNo { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public int? YearOfManufacture { get; set; }
        public string Owner { get; set; }
    }

    public class UpdateBusDto
    {
        public int BusId { get; set; }
        public string BusNo { get; set; }
        public string BusType { get; set; }
        public int Capacity { get; set; }
        public string RegistrationNo { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public int? YearOfManufacture { get; set; }
        public string Owner { get; set; }
        public bool IsActive { get; set; }
    }

    public class BusDto
    {
        public int BusId { get; set; }
        public string BusNo { get; set; }
        public string BusType { get; set; }
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
