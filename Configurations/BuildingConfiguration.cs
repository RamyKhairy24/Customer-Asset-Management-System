using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerFluent.Models;

namespace CustomerFluent.Configurations
{
    public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public void Configure(EntityTypeBuilder<Building> builder)
        {
            builder.ToTable("tblBuilding");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.BuildingNumber)
                .IsRequired()
                .HasComment("Building number on the street");

            builder.Property(e => e.Floor)
                .IsRequired()
                .HasColumnType("tinyint")
                .HasComment("Floor number (0=Ground, 1-200=Upper floors)");

            builder.Property(e => e.ApartmentNumber)
                .IsRequired()
                .HasComment("Apartment number on the floor");

            builder.Property(e => e.LocationId)
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
            builder.HasOne(e => e.Location)
                .WithOne(l => l.Building)
                .HasForeignKey<Building>(e => e.LocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblBuilding_Location");

            // Indexes
            builder.HasIndex(e => e.LocationId)
                .IsUnique()
                .HasDatabaseName("IX_tblBuilding_LocationId");

            builder.HasIndex(e => new { e.BuildingNumber, e.Floor, e.ApartmentNumber })
                .HasDatabaseName("IX_tblBuilding_Complete_Address");

            // Enhanced logical constraints
            builder.ToTable(t => t.HasCheckConstraint("CK_tblBuilding_BuildingNumber", 
                "[BuildingNumber] BETWEEN 1 AND 9999")); // Realistic building numbers

            builder.ToTable(t => t.HasCheckConstraint("CK_tblBuilding_Floor", 
                "[Floor] BETWEEN 0 AND 50")); // More realistic floor range (0=Ground)

            builder.ToTable(t => t.HasCheckConstraint("CK_tblBuilding_ApartmentNumber", 
                "[ApartmentNumber] BETWEEN 1 AND 999")); // Realistic apartment numbers

            // Logical relationship: Ground floor can have apartments 1-20, upper floors 1-10
            builder.ToTable(t => t.HasCheckConstraint("CK_tblBuilding_Floor_Apartment_Logic",
                "([Floor] = 0 AND [ApartmentNumber] BETWEEN 1 AND 20) OR " +
                "([Floor] > 0 AND [ApartmentNumber] BETWEEN 1 AND 10)"));

            builder.ToTable(t => t.HasTrigger("TR_tblBuilding_UpdateModifiedDate"));
        }
    }
}