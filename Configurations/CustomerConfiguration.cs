using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerFluent.Models;

namespace CustomerFluent.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("tblCustomer");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Full name of the customer");

            builder.Property(e => e.Age)
                .IsRequired()
                .HasColumnType("tinyint")
                .HasComment("Age in years (must be between 16-120)");

            builder.Property(e => e.Height)
                .IsRequired()
                .HasColumnType("decimal(5,2)")
                .HasComment("Height in centimeters");

            builder.Property(e => e.Weight)
                .IsRequired()
                .HasColumnType("decimal(5,2)")
                .HasComment("Weight in kilograms");

            builder.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(25)
                .HasComment("Phone number in Egyptian format");

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Record creation timestamp");

            builder.Property(e => e.ModifiedDate)
                .IsRequired()
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Last modification timestamp");

            builder.HasIndex(e => e.Name)
                .HasDatabaseName("IX_tblCustomer_Name");

            builder.HasIndex(e => e.PhoneNumber)
                .IsUnique()
                .HasDatabaseName("IX_tblCustomer_PhoneNumber");

            builder.HasIndex(e => e.Age)
                .HasDatabaseName("IX_tblCustomer_Age");

            builder.ToTable(t => t.HasCheckConstraint("CK_tblCustomer_Age", 
                "[Age] BETWEEN 16 AND 120")); 

            builder.ToTable(t => t.HasCheckConstraint("CK_tblCustomer_Height", 
                "[Height] BETWEEN 100.0 AND 250.0")); 

            builder.ToTable(t => t.HasCheckConstraint("CK_tblCustomer_Weight", 
                "[Weight] BETWEEN 20.0 AND 500.0")); 

            builder.ToTable(t => t.HasCheckConstraint("CK_tblCustomer_Name", 
                "LEN(LTRIM(RTRIM([Name]))) >= 2 AND [Name] NOT LIKE '%[0-9]%'")); 

            // RELAXED PHONE CONSTRAINT - More flexible
            builder.ToTable(t => t.HasCheckConstraint("CK_tblCustomer_Phone",
                "LEN([PhoneNumber]) >= 10 AND LEN([PhoneNumber]) <= 15"));

            builder.ToTable(t => t.HasCheckConstraint("CK_tblCustomer_BMI_Logic", 
                "([Weight] / POWER([Height]/100.0, 2)) BETWEEN 10.0 AND 60.0")); 

            builder.ToTable(t => t.HasTrigger("TR_tblCustomer_UpdateModifiedDate"));
        }
    }
}