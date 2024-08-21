using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Contact
/// </summary>
public sealed class ContactConfig : IEntityTypeConfiguration<Model.Contact>
{
    public void Configure(EntityTypeBuilder<Model.Contact> entity)
    {
        entity.ToTable("tbl_contact");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
    }
}