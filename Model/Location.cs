namespace CustomerFluent.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public bool IsHouse { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Customer Customer { get; set; } = null!;
        public House? House { get; set; }
        public Building? Building { get; set; }
    }
}