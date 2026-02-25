using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerFluent.Models;

namespace CustomerFluent.Configurations
{
    public class PaymentStatusConfiguration : IEntityTypeConfiguration<PaymentStatus>
    {
        public void Configure(EntityTypeBuilder<PaymentStatus> builder)
        {
            builder.ToTable("tblPaymentStatus");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.AmountPaid)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasComment("Amount already paid by customer");

            builder.Property(e => e.AmountRemaining)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasComment("Remaining amount to be paid");

            builder.Property(e => e.CustomerId)
                .IsRequired();

            builder.Property(e => e.PaymentDate)
                .IsRequired()
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Date of payment transaction");

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
                .WithMany(c => c.PaymentStatuses)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblPaymentStatus_Customer");

            // Indexes for financial queries
            builder.HasIndex(e => e.CustomerId)
                .HasDatabaseName("IX_tblPaymentStatus_CustomerId");

            builder.HasIndex(e => e.PaymentDate)
                .HasDatabaseName("IX_tblPaymentStatus_PaymentDate");

            builder.HasIndex(e => new { e.CustomerId, e.PaymentDate })
                .HasDatabaseName("IX_tblPaymentStatus_Customer_Date");

            builder.HasIndex(e => e.AmountRemaining)
                .HasDatabaseName("IX_tblPaymentStatus_AmountRemaining");

            // Enhanced financial logic constraints - FIXED
            builder.ToTable(t => t.HasCheckConstraint("CK_tblPaymentStatus_AmountPaid", 
                "[AmountPaid] >= 1.00")); // Use consistent minimum payment amount

            builder.ToTable(t => t.HasCheckConstraint("CK_tblPaymentStatus_AmountRemaining", 
                "[AmountRemaining] >= 0"));

            builder.ToTable(t => t.HasCheckConstraint("CK_tblPaymentStatus_TotalAmount", 
                "[AmountPaid] + [AmountRemaining] >= 1.00")); // Minimum total transaction

            builder.ToTable(t => t.HasCheckConstraint("CK_tblPaymentStatus_MaxAmount",
                "[AmountPaid] <= 99999999.99 AND [AmountRemaining] <= 99999999.99"));

            // Payment date should not be in the future
            builder.ToTable(t => t.HasCheckConstraint("CK_tblPaymentStatus_PaymentDate",
                "[PaymentDate] <= GETDATE()"));

            // Remove conflicting constraint - REMOVED CK_tblPaymentStatus_MinimumPayment

            builder.ToTable(t => t.HasTrigger("TR_tblPaymentStatus_UpdateModifiedDate"));
        }
    }
}