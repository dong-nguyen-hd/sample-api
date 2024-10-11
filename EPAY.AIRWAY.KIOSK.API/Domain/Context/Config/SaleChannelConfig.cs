using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng SaleChannel
/// </summary>
public sealed class SaleChannelConfig : IEntityTypeConfiguration<Model.ReportSection.SaleChannel>
{
    public static List<Guid> SaleChannelIds = [];

    public void Configure(EntityTypeBuilder<Model.ReportSection.SaleChannel> entity)
    {
        entity.ToTable("tbl_sale_channel");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);

        // Seeding data
        for (int i = 0 ; i < 4 ; i++) SaleChannelIds.Add(Guid.NewGuid());
        entity.HasData([
            new
            {
                Id = SaleChannelIds[0],
                Code = "01",
                Name = "VNEID",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
            new Model.ReportSection.SaleChannel()
            {
                Id = SaleChannelIds[1],
                Code = "02",
                Name = "EPAY eWallet",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
            new Model.ReportSection.SaleChannel()
            {
                Id = SaleChannelIds[2],
                Code = "03",
                Name = "Kiosk",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
            new Model.ReportSection.SaleChannel()
            {
                Id = SaleChannelIds[3],
                Code = "04",
                Name = "ServiceProvider (các kênh khác của đơn vị cung cấp dịch vụ, vd: web)",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            }
        ]);
    }
}