namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Log
/// </summary>
public sealed class LogConfig : IEntityTypeConfiguration<Models.Log>
{
    public void Configure(EntityTypeBuilder<Models.Log> entity)
    {
        entity.ToTable("tbl_log");
        entity.Property(x => x.RequestDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.ResponseDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.Property(x => x.RequestHeaders).HasConversion(
            v => v.MySerialize(),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, RelateText.GetMySerializeConfig()));

        entity.Property(x => x.RequestQueries).HasConversion(
            v => v.MySerialize(),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, RelateText.GetMySerializeConfig()));

        entity.Property(x => x.ResponseHeaders).HasConversion(
            v => v.MySerialize(),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, RelateText.GetMySerializeConfig()));

        entity.Property(x => x.LogType).HasConversion<string>();

        entity.HasKey(x => x.Id);
        entity.HasIndex(x => new { x.RequestDatetimeUtc, x.TraceId, x.RequestPath, x.HasException, x.LogType });
    }
}