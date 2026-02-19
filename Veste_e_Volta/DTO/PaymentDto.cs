   public class CreatePaymentDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "pending";
    }

    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid RentalId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "pending";
        public DateTime CreatedAt { get; set; }
    }
