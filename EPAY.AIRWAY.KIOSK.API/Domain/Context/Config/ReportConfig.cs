using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Report
/// </summary>
public sealed class ReportConfig : IEntityTypeConfiguration<Model.ReportSection.Report>
{
    public void Configure(EntityTypeBuilder<Model.ReportSection.Report> entity)
    {
        entity.ToTable("tbl_report");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
        
        entity.Property(x => x.OtherInfo).HasConversion(
            v => v.MySerialize(),
            v => v.MyDeserialize<Model.ReportSection.ToJson.OtherInfo>());
        
        // Indexing
        entity.HasIndex(x => new { x.OrderCode, x.Active });
        entity.HasIndex(x => new { x.CreatedDatetimeUtc, x.Active });
    }
}