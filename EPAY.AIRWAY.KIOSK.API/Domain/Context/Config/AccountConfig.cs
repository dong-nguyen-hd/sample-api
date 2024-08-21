using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.ToJson;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Account
/// </summary>
public sealed class AccountConfig : IEntityTypeConfiguration<Model.Account>
{
    public const string AdminId = "-1";
    public const string DeviceId = "-2";
    public const string EpayDeviceId = "-3";
    
    public void Configure(EntityTypeBuilder<Model.Account> entity)
    {
        entity.ToTable("tbl_account");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.Property(x => x.AdditionData).HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<AdditionData>(v, (JsonSerializerOptions)null));

        entity.Property(x => x.SystemRoles).HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);

        // Indexing
        entity.HasIndex(x => x.UserName).IsUnique();
        entity.HasIndex(x => new { x.UserName, x.Active })
            .IncludeProperties(x =>
                new
                {
                    x.Id,
                    x.Password,
                });

        // Seeding data
        entity.HasData(
        [
            new Model.Account
            {
                Id = AdminId,
                UserName = "admin",
                Password = "10000./CP+/UCm70eq07vqVhMohg==.vDhe4BzzcSf7RbwTMdycBnFmLN7Lc2SRFjcUFMAWp1U=", // Password: admin@epay
                Name = "ADMIN - EPAY",
                SystemRoles = [MyPolicy.Administrator],
                AdditionData = new(),
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
            new Model.Account
            {
                Id = DeviceId,
                UserName = "vungocanh",
                Password = "10000.Y7SOy33CScWSulwViJVXMQ==.E06b2v6lbfklDbvPqOsOKcCpZFqN/InKCsUs9kVXIH4=", // Password: 1
                Name = "DEVICE - EPAY",
                SystemRoles = [MyPolicy.Device, MyPolicy.Viewer],
                AdditionData = new(),
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
            new Model.Account
            {
                Id = EpayDeviceId,
                UserName = "epaydevice",
                Password = "10000.pYbpSlKnPx2/rWWxRxV+ig==.vET7tWqa5a+hdkNMIgnQsTf4/8g4uJsMTinr1XXt7uo=", // Password: mqEDrpdTIk8N
                Name = "DEVICE - EPAY",
                SystemRoles = [MyPolicy.Device, MyPolicy.Viewer],
                AdditionData = new(),
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
        ]);
    }
}