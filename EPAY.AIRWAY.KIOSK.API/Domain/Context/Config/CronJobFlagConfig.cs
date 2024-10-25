using EPAY.AIRWAY.KIOSK.API.Services.CronJob;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng CronJobFlag
/// </summary>
public sealed class CronJobFlagConfig : IEntityTypeConfiguration<Model.CronJobFlag>
{
    public void Configure(EntityTypeBuilder<Model.CronJobFlag> entity)
    {
        entity.ToTable("tbl_cron_job_flag");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
        entity.Property(x => x.Version).IsRowVersion();
        
        // Indexing
        entity.HasIndex(x => new { x.Name, x.Active });

        entity.HasData(new[]
        {
            new Models.CronJobFlag
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Name = $"{nameof(DeleteExpiredLogJob)}"
            },
            new Models.CronJobFlag
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Name = $"{nameof(DeleteExpiredTokenJob)}"
            },
            new Models.CronJobFlag
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Name = $"{nameof(PaymentReportJob)}/Daily"
            },
            new Models.CronJobFlag
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Name = $"{nameof(PaymentReportJob)}/Monthly"
            }
        });
    }
}