namespace BusTicketManagement.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // Pending, Paid, Failed, Refunded
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? RefundDate { get; set; }
        public string RefundReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
