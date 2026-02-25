namespace CustomerFluent.Models
{
    public class House
    {
        public int Id { get; set; }
        public int HouseNumber { get; set; }
        public int LocationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Location Location { get; set; } = null!;
    }
}