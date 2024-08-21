using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng AdditionalService
/// </summary>
public sealed class AdditionalServiceConfig : IEntityTypeConfiguration<Model.AdditionalService>
{
    public void Configure(EntityTypeBuilder<Model.AdditionalService> entity)
    {
        entity.ToTable("tbl_additional_service");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
    }
}