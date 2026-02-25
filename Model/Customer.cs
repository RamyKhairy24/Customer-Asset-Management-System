namespace CustomerFluent.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public byte Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Location? Location { get; set; }
        public ICollection<PaymentStatus> PaymentStatuses { get; set; } = new List<PaymentStatus>();
    }
}