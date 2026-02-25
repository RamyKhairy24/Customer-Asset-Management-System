using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerFluent.Models;

namespace CustomerFluent.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("tblLocation");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("مصر")
                .HasComment("Country name (default: Egypt)");

            builder.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("City or governorate name");

            builder.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(300)
                .HasComment("Street address details");

            builder.Property(e => e.IsHouse)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("True for house, False for apartment building");

            builder.Property(e => e.CustomerId)
                .IsRequired();

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.ModifiedDate)
                .IsRequired()
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(e => e.Customer)
                .WithOne(c => c.Location)
                .HasForeignKey<Location>(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblLocation_Customer");

            // Indexes
            builder.HasIndex(e => e.CustomerId)
                .IsUnique()
                .HasDatabaseName("IX_tblLocation_CustomerId");

            builder.HasIndex(e => new { e.Country, e.City })
                .HasDatabaseName("IX_tblLocation_Country_City");

            builder.HasIndex(e => e.IsHouse)
                .HasDatabaseName("IX_tblLocation_IsHouse");

            // Enhanced logical constraints
            builder.ToTable(t => t.HasCheckConstraint("CK_tblLocation_Country", 
                "LEN(LTRIM(RTRIM([Country]))) >= 2")); // Minimum country name length

            builder.ToTable(t => t.HasCheckConstraint("CK_tblLocation_City", 
                "LEN(LTRIM(RTRIM([City]))) >= 2 AND [City] NOT LIKE '%[0-9]%'")); // No numbers in city names

            builder.ToTable(t => t.HasCheckConstraint("CK_tblLocation_Street", 
                "LEN(LTRIM(RTRIM([Street]))) >= 5")); // Meaningful street address

            // Logical constraint: Street should contain meaningful content
            builder.ToTable(t => t.HasCheckConstraint("CK_tblLocation_Street_Content",
                "[Street] LIKE '%شارع%' OR [Street] LIKE '%ش.%' OR " +
                "[Street] LIKE '%Street%' OR [Street] LIKE '%St.%' OR " +
                "[Street] LIKE '%Avenue%' OR [Street] LIKE '%Road%' OR " +
                "[Street] LIKE '%طريق%' OR LEN([Street]) >= 10"));

            builder.ToTable(t => t.HasTrigger("TR_tblLocation_UpdateModifiedDate"));
        }
    }
}