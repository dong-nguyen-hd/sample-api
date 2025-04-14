using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Bill
/// </summary>
public sealed class BillConfig : IEntityTypeConfiguration<Model.Bill>
{
    public void Configure(EntityTypeBuilder<Model.Bill> entity)
    {
        entity.ToTable("tbl_bill");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
        
        entity.HasIndex(x => new { x.AbTripOrderId, x.ExpiredDatetimeUtc, x.Active, });
    }
}