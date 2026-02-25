namespace CustomerFluent.DTOs
{
    public class CreateCustomerDto
    {
        public string Name { get; set; } = null!;
        public byte Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public CreateLocationDto? Location { get; set; }
    }

    public class UpdateCustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public byte Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string PhoneNumber { get; set; } = null!;
    }

    public class CreateLocationDto
    {
        public string Country { get; set; } = "???";
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public bool IsHouse { get; set; }
        public CreateHouseDto? House { get; set; }
        public CreateBuildingDto? Building { get; set; }
    }

    public class CreateHouseDto
    {
        public int HouseNumber { get; set; }
    }

    public class CreateBuildingDto
    {
        public int BuildingNumber { get; set; }
        public byte Floor { get; set; }
        public int ApartmentNumber { get; set; }
    }

    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public byte Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public LocationResponseDto? Location { get; set; }
        public ICollection<PaymentStatusResponseDto> PaymentStatuses { get; set; } = new List<PaymentStatusResponseDto>();
    }

    public class LocationResponseDto
    {
        public int Id { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public bool IsHouse { get; set; }
        public HouseResponseDto? House { get; set; }
        public BuildingResponseDto? Building { get; set; }
    }

    public class HouseResponseDto
    {
        public int Id { get; set; }
        public int HouseNumber { get; set; }
    }

    public class BuildingResponseDto
    {
        public int Id { get; set; }
        public int BuildingNumber { get; set; }
        public byte Floor { get; set; }
        public int ApartmentNumber { get; set; }
    }

    public class PaymentStatusResponseDto
    {
        public int Id { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}