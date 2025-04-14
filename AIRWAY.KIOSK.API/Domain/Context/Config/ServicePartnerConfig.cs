using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng ServicePartner
/// </summary>
public sealed class ServicePartnerConfig : IEntityTypeConfiguration<Model.ReportSection.ServicePartner>
{
    public void Configure(EntityTypeBuilder<Model.ReportSection.ServicePartner> entity)
    {
        entity.ToTable("tbl_service_partner");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);

        // Seeding data
        entity.HasData([
            new()
            {
                Id = Guid.NewGuid(),
                SaleChannelId = SaleChannelConfig.SaleChannelIds[2],
                Code = "031",
                Name = "Kiosk BV Saint Paul",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true,
            },
            new()
            {
                Id = Guid.NewGuid(),
                SaleChannelId = SaleChannelConfig.SaleChannelIds[2],
                Code = "032",
                Name = "Kiosk BV Đống Đa",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true,
            },
            new()
            {
                Id = Guid.NewGuid(),
                SaleChannelId = SaleChannelConfig.SaleChannelIds[2],
                Code = "033",
                Name = "Kiosk Dịch vụ công",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true,
            },
            new()
            {
                Id = Guid.NewGuid(),
                SaleChannelId = SaleChannelConfig.SaleChannelIds[2],
                Code = "034",
                Name = "Kiosk ĐSVN",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true,
            },
            new()
            {
                Id = Guid.NewGuid(),
                SaleChannelId = SaleChannelConfig.SaleChannelIds[2],
                Code = "035",
                Name = "Kiosk ABTrip",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true,
            },
            new()
            {
                Id = Guid.NewGuid(),
                SaleChannelId = SaleChannelConfig.SaleChannelIds[2],
                Code = "036",
                Name = "Kiosk ACV",
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true,
            }
        ]);
    }
}