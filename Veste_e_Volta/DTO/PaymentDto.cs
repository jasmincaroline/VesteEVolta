public class CreatePaymentDto
{
    public string PaymentMethod { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = "pending";
}
