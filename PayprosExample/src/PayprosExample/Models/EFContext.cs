using Microsoft.Data.Entity;

namespace PayprosExample.Models
{
    public partial class EFContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region [finance]
            modelBuilder.Entity<PayPros>(entity =>
            {
                entity.Property(e => e.PaymentAccount).HasColumnType("varchar");
                entity.Property(e => e.PaymentAmount).HasColumnType("decimal");

                entity.Property(e => e.RequestingApplication)
                    .HasColumnType("varchar")
                    .HasDefaultValue("Legacy");
                entity.Property(e => e.RequestingOrigin).HasColumnType("varchar");

                entity.Property(e => e.NameOnCard).HasColumnType("varchar");
                entity.Property(e => e.CCExpireMonth).HasColumnType("varchar");
                entity.Property(e => e.CCExpireYear).HasColumnType("varchar");
                entity.Property(e => e.CCLast4).HasColumnType("varchar");
                entity.Property(e => e.CCV).HasColumnType("varchar");

                entity.Ignore(e => e.CreditCardNumber);

                entity.Property(e => e.responseText).HasColumnType("varchar");
                entity.Property(e => e.retryRecommended).HasDefaultValue(false);
            });
            #endregion
        }
        
        #region [finance]
        public virtual DbSet<PayPros> PayPros { get; set; }
        #endregion
        
    }
}
