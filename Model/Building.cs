namespace CustomerFluent.Models
{
    public class Building
    {
        public int Id { get; set; }
        public int BuildingNumber { get; set; }
        public byte Floor { get; set; }
        public int ApartmentNumber { get; set; }
        public int LocationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Location Location { get; set; } = null!;
    }
}