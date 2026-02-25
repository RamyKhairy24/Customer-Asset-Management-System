namespace CustomerFluent.Models
{
    public class PaymentStatus
    {
        public int Id { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public int CustomerId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Customer Customer { get; set; } = null!;
    }
}