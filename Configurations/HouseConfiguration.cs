using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerFluent.Models;

namespace CustomerFluent.Configurations
{
    public class HouseConfiguration : IEntityTypeConfiguration<House>
    {
        public void Configure(EntityTypeBuilder<House> builder)
        {
            builder.ToTable("tblHouse");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.HouseNumber)
                .IsRequired()
                .HasComment("House number on the street");

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
                .WithOne(l => l.House)
                .HasForeignKey<House>(e => e.LocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblHouse_Location");

            // Indexes
            builder.HasIndex(e => e.LocationId)
                .IsUnique()
                .HasDatabaseName("IX_tblHouse_LocationId");

            builder.HasIndex(e => e.HouseNumber)
                .HasDatabaseName("IX_tblHouse_HouseNumber");

            // Enhanced logical constraints
            builder.ToTable(t => t.HasCheckConstraint("CK_tblHouse_HouseNumber",
                "[HouseNumber] BETWEEN 1 AND 9999")); // More realistic range

            builder.ToTable(t => t.HasTrigger("TR_tblHouse_UpdateModifiedDate"));
        }
    }
}