using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng PaymentTransaction
/// </summary>
public sealed class PaymentTransactionConfig : IEntityTypeConfiguration<Models.PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<Models.PaymentTransaction> entity)
    {
        entity.ToTable("tbl_payment_transaction");
        entity.Property(x => x.ExpiredDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Version).IsRowVersion();
        entity.HasQueryFilter(x => x.Active);

        // Indexing
        entity.HasIndex(x => new { x.OrderCode, BillId = x.BillId, x.Active })
            .IncludeProperties(x =>
                new
                {
                    x.Id,
                    x.ServiceProviderStatus,
                    x.PaymentProviderStatus,
                });
    }
}