using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;


/// <summary>
/// Chức năng: cấu hình schema cho bảng FlightData
/// </summary>
public sealed class FlightDataConfig : IEntityTypeConfiguration<Model.FlightData>
{
    public void Configure(EntityTypeBuilder<Model.FlightData> entity)
    {
        entity.ToTable("tbl_flightdata");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
        
        // Indexing
        entity.HasIndex(x => new { x.StartPoint, x.Active });
    }
}