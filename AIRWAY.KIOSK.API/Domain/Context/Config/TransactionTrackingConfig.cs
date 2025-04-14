using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng TransactionTracking
/// </summary>
public sealed class TransactionTrackingConfig : IEntityTypeConfiguration<Model.TransactionTracking>
{
    public void Configure(EntityTypeBuilder<Model.TransactionTracking> entity)
    {
        entity.ToTable("tbl_transaction_tracking");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
    }
}