using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CustomerFluent.Configurations;
using CustomerFluent.Models; // Correct namespace
using CustomerFluent.Models.Identity;

namespace CustomerFluent.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<House> Houses { get; set; } = null!;
        public DbSet<Building> Buildings { get; set; } = null!;
        public DbSet<PaymentStatus> PaymentStatuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // IMPORTANT: Call base first for Identity

            // Set default schema first
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new HouseConfiguration());
            modelBuilder.ApplyConfiguration(new BuildingConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentStatusConfiguration());

            // Configure Identity tables to use consistent naming
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.ToTable("Users", "dbo"); // Explicit schema
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.ToTable("Roles", "dbo"); // Explicit schema
            });

            // Configure other Identity tables with explicit schema
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()
                .ToTable("AspNetUserRoles", "dbo");

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>()
                .ToTable("AspNetUserClaims", "dbo");

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                .ToTable("AspNetUserLogins", "dbo");

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>()
                .ToTable("AspNetRoleClaims", "dbo");

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
                .ToTable("AspNetUserTokens", "dbo");

            SeedData(modelBuilder);
            SeedIdentityData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "أحمد محمد علي",
                    Age = 28,
                    Height = 175.50m,
                    Weight = 75.20m,
                    PhoneNumber = "+201234567890",
                    CreatedDate = new DateTime(2024, 1, 15, 10, 30, 0),
                    ModifiedDate = new DateTime(2024, 1, 15, 10, 30, 0)
                },
                new Customer
                {
                    Id = 2,
                    Name = "فاطمة حسن محمود",
                    Age = 25,
                    Height = 162.00m,
                    Weight = 58.50m,
                    PhoneNumber = "01098765432",
                    CreatedDate = new DateTime(2024, 2, 10, 14, 20, 0),
                    ModifiedDate = new DateTime(2024, 2, 10, 14, 20, 0)
                },
                new Customer
                {
                    Id = 3,
                    Name = "محمد أحمد صالح",
                    Age = 35,
                    Height = 180.25m,
                    Weight = 82.75m,
                    PhoneNumber = "+201555123456",
                    CreatedDate = new DateTime(2024, 3, 5, 9, 15, 0),
                    ModifiedDate = new DateTime(2024, 3, 5, 9, 15, 0)
                },
                new Customer
                {
                    Id = 4,
                    Name = "مريم عبد الله",
                    Age = 22,
                    Height = 165.75m,
                    Weight = 55.30m,
                    PhoneNumber = "01012345678", // Fixed to match migration
                    CreatedDate = new DateTime(2024, 3, 20, 16, 45, 0),
                    ModifiedDate = new DateTime(2024, 3, 20, 16, 45, 0)
                },
                new Customer
                {
                    Id = 5,
                    Name = "خالد إبراهيم",
                    Age = 42,
                    Height = 178.00m,
                    Weight = 88.00m,
                    PhoneNumber = "+201777888999",
                    CreatedDate = new DateTime(2024, 4, 12, 11, 30, 0),
                    ModifiedDate = new DateTime(2024, 4, 12, 11, 30, 0)
                }
            );

            modelBuilder.Entity<Location>().HasData(
                new Location
                {
                    Id = 1,
                    Country = "مصر",
                    City = "القاهرة",
                    Street = "شارع النصر، مدينة نصر",
                    IsHouse = true,
                    CustomerId = 1,
                    CreatedDate = new DateTime(2024, 1, 15, 10, 35, 0),
                    ModifiedDate = new DateTime(2024, 1, 15, 10, 35, 0)
                },
                new Location
                {
                    Id = 2,
                    Country = "مصر",
                    City = "الإسكندرية",
                    Street = "شارع الحرية، محطة الرمل",
                    IsHouse = false,
                    CustomerId = 2,
                    CreatedDate = new DateTime(2024, 2, 10, 14, 25, 0),
                    ModifiedDate = new DateTime(2024, 2, 10, 14, 25, 0)
                },
                new Location
                {
                    Id = 3,
                    Country = "مصر",
                    City = "الجيزة",
                    Street = "شارع الهرم، الطالبية",
                    IsHouse = false,
                    CustomerId = 3,
                    CreatedDate = new DateTime(2024, 3, 5, 9, 20, 0),
                    ModifiedDate = new DateTime(2024, 3, 5, 9, 20, 0)
                },
                new Location
                {
                    Id = 4,
                    Country = "مصر",
                    City = "القاهرة",
                    Street = "شارع التحرير، وسط البلد",
                    IsHouse = true,
                    CustomerId = 4,
                    CreatedDate = new DateTime(2024, 3, 20, 16, 50, 0),
                    ModifiedDate = new DateTime(2024, 3, 20, 16, 50, 0)
                },
                new Location
                {
                    Id = 5,
                    Country = "مصر",
                    City = "المنصورة",
                    Street = "شارع الجمهورية، وسط المدينة",
                    IsHouse = false,
                    CustomerId = 5,
                    CreatedDate = new DateTime(2024, 4, 12, 11, 35, 0),
                    ModifiedDate = new DateTime(2024, 4, 12, 11, 35, 0)
                }
            );

            modelBuilder.Entity<House>().HasData(
                new House
                {
                    Id = 1,
                    HouseNumber = 25,
                    LocationId = 1,
                    CreatedDate = new DateTime(2024, 1, 15, 10, 40, 0),
                    ModifiedDate = new DateTime(2024, 1, 15, 10, 40, 0)
                },
                new House
                {
                    Id = 2,
                    HouseNumber = 142,
                    LocationId = 4,
                    CreatedDate = new DateTime(2024, 3, 20, 16, 55, 0),
                    ModifiedDate = new DateTime(2024, 3, 20, 16, 55, 0)
                }
            );

            modelBuilder.Entity<Building>().HasData(
                new Building
                {
                    Id = 1,
                    BuildingNumber = 15,
                    Floor = 3,
                    ApartmentNumber = 8,
                    LocationId = 2,
                    CreatedDate = new DateTime(2024, 2, 10, 14, 30, 0),
                    ModifiedDate = new DateTime(2024, 2, 10, 14, 30, 0)
                },
                new Building
                {
                    Id = 2,
                    BuildingNumber = 88,
                    Floor = 7,
                    ApartmentNumber = 5,
                    LocationId = 3,
                    CreatedDate = new DateTime(2024, 3, 5, 9, 25, 0),
                    ModifiedDate = new DateTime(2024, 3, 5, 9, 25, 0)
                },
                new Building
                {
                    Id = 3,
                    BuildingNumber = 33,
                    Floor = 2,
                    ApartmentNumber = 4,
                    LocationId = 5,
                    CreatedDate = new DateTime(2024, 4, 12, 11, 40, 0),
                    ModifiedDate = new DateTime(2024, 4, 12, 11, 40, 0)
                }
            );

            modelBuilder.Entity<PaymentStatus>().HasData(
                new PaymentStatus
                {
                    Id = 1,
                    AmountPaid = 5000.00m,
                    AmountRemaining = 15000.00m,
                    CustomerId = 1,
                    PaymentDate = new DateTime(2024, 1, 20, 14, 30, 0),
                    CreatedDate = new DateTime(2024, 1, 20, 14, 30, 0),
                    ModifiedDate = new DateTime(2024, 1, 20, 14, 30, 0)
                },
                new PaymentStatus
                {
                    Id = 2,
                    AmountPaid = 3000.00m,
                    AmountRemaining = 12000.00m,
                    CustomerId = 1,
                    PaymentDate = new DateTime(2024, 2, 15, 10, 15, 0),
                    CreatedDate = new DateTime(2024, 2, 15, 10, 15, 0),
                    ModifiedDate = new DateTime(2024, 2, 15, 10, 15, 0)
                },
                new PaymentStatus
                {
                    Id = 3,
                    AmountPaid = 8500.50m,
                    AmountRemaining = 6500.50m,
                    CustomerId = 2,
                    PaymentDate = new DateTime(2024, 2, 25, 16, 20, 0),
                    CreatedDate = new DateTime(2024, 2, 25, 16, 20, 0),
                    ModifiedDate = new DateTime(2024, 2, 25, 16, 20, 0)
                },
                new PaymentStatus
                {
                    Id = 4,
                    AmountPaid = 12000.75m,
                    AmountRemaining = 8000.25m,
                    CustomerId = 3,
                    PaymentDate = new DateTime(2024, 3, 10, 11, 45, 0),
                    CreatedDate = new DateTime(2024, 3, 10, 11, 45, 0),
                    ModifiedDate = new DateTime(2024, 3, 10, 11, 45, 0)
                },
                new PaymentStatus
                {
                    Id = 5,
                    AmountPaid = 2500.00m,
                    AmountRemaining = 7500.00m,
                    CustomerId = 4,
                    PaymentDate = new DateTime(2024, 3, 25, 13, 30, 0),
                    CreatedDate = new DateTime(2024, 3, 25, 13, 30, 0),
                    ModifiedDate = new DateTime(2024, 3, 25, 13, 30, 0)
                },
                new PaymentStatus
                {
                    Id = 6,
                    AmountPaid = 18000.00m,
                    AmountRemaining = 0.00m,
                    CustomerId = 5,
                    PaymentDate = new DateTime(2024, 4, 15, 15, 45, 0),
                    CreatedDate = new DateTime(2024, 4, 15, 15, 45, 0),
                    ModifiedDate = new DateTime(2024, 4, 15, 15, 45, 0)
                }
            );
        }

        private static void SeedIdentityData(ModelBuilder modelBuilder)
        {
            // Leave empty for now - roles and admin user are seeded in Program.cs
        }
    }
}