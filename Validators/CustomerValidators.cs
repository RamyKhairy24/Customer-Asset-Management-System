using FluentValidation;
using CustomerFluent.DTOs;

namespace CustomerFluent.Validators
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 200).WithMessage("Name must be between 2 and 200 characters")
                .Matches(@"^[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF\s\u0020]+$|^[a-zA-Z\s]+$")
                .WithMessage("Name can only contain Arabic or English letters and spaces")
                .Must(NotContainNumbers).WithMessage("Name cannot contain numbers");

            RuleFor(x => x.Age)
                .InclusiveBetween((byte)16, (byte)120)
                .WithMessage("Age must be between 16 and 120 years");

            RuleFor(x => x.Height)
                .InclusiveBetween(100.0m, 250.0m)
                .WithMessage("Height must be between 100.0 and 250.0 cm");

            RuleFor(x => x.Weight)
                .InclusiveBetween(20.0m, 500.0m)
                .WithMessage("Weight must be between 20.0 and 500.0 kg");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^(\+20[0-9]{8,9}|01[0-9]{9}|0[2-9][0-9]{8})$")
                .WithMessage("Phone number must be in valid Egyptian format");

            // BMI validation
            RuleFor(x => x)
                .Must(HaveValidBMI)
                .WithMessage("BMI must be between 10.0 and 60.0")
                .WithName("BMI");

            // Location validation
            RuleFor(x => x.Location)
                .SetValidator(new CreateLocationValidator()!)
                .When(x => x.Location != null);
        }

        private static bool NotContainNumbers(string name)
        {
            return !name.Any(char.IsDigit);
        }

        private static bool HaveValidBMI(CreateCustomerDto customer)
        {
            if (customer.Height <= 0) return false;

            var heightInMeters = customer.Height / 100.0m;
            var bmi = customer.Weight / (heightInMeters * heightInMeters);

            return bmi >= 10.0m && bmi <= 60.0m;
        }
    }

    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 200).WithMessage("Name must be between 2 and 200 characters")
                .Matches(@"^[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF\s\u0020]+$|^[a-zA-Z\s]+$")
                .WithMessage("Name can only contain Arabic or English letters and spaces")
                .Must(NotContainNumbers).WithMessage("Name cannot contain numbers");

            RuleFor(x => x.Age)
                .InclusiveBetween((byte)16, (byte)120)
                .WithMessage("Age must be between 16 and 120 years");

            RuleFor(x => x.Height)
                .InclusiveBetween(100.0m, 250.0m)
                .WithMessage("Height must be between 100.0 and 250.0 cm");

            RuleFor(x => x.Weight)
                .InclusiveBetween(20.0m, 500.0m)
                .WithMessage("Weight must be between 20.0 and 500.0 kg");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^(\+20[0-9]{8,9}|01[0-9]{9}|0[2-9][0-9]{8})$")
                .WithMessage("Phone number must be in valid Egyptian format");

            RuleFor(x => x)
                .Must(HaveValidBMI)
                .WithMessage("BMI must be between 10.0 and 60.0")
                .WithName("BMI");
        }

        private static bool NotContainNumbers(string name)
        {
            return !name.Any(char.IsDigit);
        }

        private static bool HaveValidBMI(UpdateCustomerDto customer)
        {
            if (customer.Height <= 0) return false;
            
            var heightInMeters = customer.Height / 100.0m;
            var bmi = customer.Weight / (heightInMeters * heightInMeters);
            
            return bmi >= 10.0m && bmi <= 60.0m;
        }
    }

    public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
    {
        public CreateLocationValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .Length(2, 50).WithMessage("Country must be between 2 and 50 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .Length(2, 100).WithMessage("City must be between 2 and 100 characters")
                .Matches(@"^[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF\s\u0020]+$|^[a-zA-Z\s]+$")
                .WithMessage("City can only contain Arabic or English letters and spaces");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required")
                .Length(5, 300).WithMessage("Street must be between 5 and 300 characters")
                .Must(ContainValidStreetContent)
                .WithMessage("Street must contain valid street indicators (شارع, ش., Street, St., Avenue, Road, طريق) or be at least 10 characters long");

            RuleFor(x => x.House)
                .NotNull().WithMessage("House information is required for house addresses")
                .SetValidator(new CreateHouseValidator()!)
                .When(x => x.IsHouse);

            RuleFor(x => x.Building)
                .NotNull().WithMessage("Building information is required for apartment addresses")
                .SetValidator(new CreateBuildingValidator()!)
                .When(x => !x.IsHouse);

            RuleFor(x => x.House)
                .Null().WithMessage("House information should not be provided for apartment addresses")
                .When(x => !x.IsHouse);

            RuleFor(x => x.Building)
                .Null().WithMessage("Building information should not be provided for house addresses")
                .When(x => x.IsHouse);
        }

        private static bool ContainValidStreetContent(string street)
        {
            var streetIndicators = new[] { "شارع", "ش.", "Street", "St.", "Avenue", "Road", "طريق" };
            return streetIndicators.Any(indicator => street.Contains(indicator, StringComparison.OrdinalIgnoreCase)) 
                   || street.Length >= 10;
        }
    }

    public class CreateHouseValidator : AbstractValidator<CreateHouseDto>
    {
        public CreateHouseValidator()
        {
            RuleFor(x => x.HouseNumber)
                .InclusiveBetween(1, 9999)
                .WithMessage("House number must be between 1 and 9999");
        }
    }

    public class CreateBuildingValidator : AbstractValidator<CreateBuildingDto>
    {
        public CreateBuildingValidator()
        {
            RuleFor(x => x.BuildingNumber)
                .InclusiveBetween(1, 9999)
                .WithMessage("Building number must be between 1 and 9999");

            RuleFor(x => x.Floor)
                .InclusiveBetween((byte)0, (byte)50)
                .WithMessage("Floor must be between 0 (Ground) and 50");

            RuleFor(x => x.ApartmentNumber)
                .InclusiveBetween(1, 999)
                .WithMessage("Apartment number must be between 1 and 999");

            RuleFor(x => x)
                .Must(HaveValidFloorApartmentRelationship)
                .WithMessage("Ground floor (0) can have apartments 1-20, upper floors can have apartments 1-10")
                .WithName("FloorApartmentRelationship");
        }

        private static bool HaveValidFloorApartmentRelationship(CreateBuildingDto building)
        {
            return building.Floor == 0 
                ? building.ApartmentNumber >= 1 && building.ApartmentNumber <= 20
                : building.ApartmentNumber >= 1 && building.ApartmentNumber <= 10;
        }
    }
}