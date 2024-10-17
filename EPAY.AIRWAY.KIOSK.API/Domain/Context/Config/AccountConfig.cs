using EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            v => v.MySerialize(),
            v => v.MyDeserialize<AdditionData>());

        entity.Property(x => x.SystemRoles).HasConversion(
            v => v.MySerialize(),
            v => v.MyDeserialize<List<string>>());

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
                HasOtp = false,
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
                HasOtp = true,
                SystemRoles = [MyPolicy.Device, MyPolicy.Viewer],
                AdditionData = new()
                {
                    Themes = new()
                    {
                        new()
                        {
                            Type = MyEnum.PlatformType.Kiosk,
                            CustomKey = "kiosk-setting",
                            Name = "kiosk-setting",
                            Description = "kiosk-setting",
                            BgColor = "#0c3a98",
                            TextColor = "#000000",
                            PaymentMethods = new()
                            {
                                new()
                                {
                                    Type = MyEnum.PaymentType.Pos,
                                    Name = "Thanh toán qua thiết bị POS",
                                    Description = "Quẹt thẻ ngân hàng bằng máy POS để thanh toán",
                                    Icon = "/resources/payment-icon/1-pos.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.Qr,
                                    Name = "Thanh toán qua QRcode",
                                    Description = "Quét QRcode bằng ứng dụng ngân hàng hoặc ví điện tử",
                                    Icon = "/resources/payment-icon/1-qr.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.PayLater,
                                    Name = "Thanh toán trả sau",
                                    Description = "Lấy phiếu đặt chỗ và thực hiện thanh toán sau",
                                    Icon = "/resources/payment-icon/1-paylater.png"
                                }
                            }
                        },
                        new()
                        {
                            Type = MyEnum.PlatformType.EpayWallet,
                            CustomKey = "epay-wallet-setting",
                            Name = "epay-wallet-setting",
                            Description = "epay-wallet-setting",
                            BgColor = "#0c3a98",
                            TextColor = "#000000",
                            PaymentMethods = new()
                            {
                                new()
                                {
                                    Type = MyEnum.PaymentType.Qr,
                                    Name = "Thanh toán qua QRcode",
                                    Description = "Quét QRcode bằng ứng dụng ngân hàng hoặc ví điện tử",
                                    Icon = "/resources/payment-icon/1-qr.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.PayLater,
                                    Name = "Thanh toán trả sau",
                                    Description = "Lấy phiếu đặt chỗ và thực hiện thanh toán sau",
                                    Icon = "/resources/payment-icon/1-paylater.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.LocalCard,
                                    Name = "Thẻ tín dụng và ghi nợ nội địa",
                                    Description = "Chấp nhận thẻ NAPAS",
                                    Icon = "/resources/payment-icon/1-local-card.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.GlobalCard,
                                    Name = "Thẻ tín dụng và ghi nợ quốc tế",
                                    Description = "Chấp nhận thẻ VISA/MASTERCARD",
                                    Icon = "/resources/payment-icon/1-global-card.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.EpayWallet,
                                    Name = "Ví điện tử EPAY",
                                    Description = "Sử dụng ví điện tử EPAY để thanh toán",
                                    Icon = "/resources/payment-icon/1-epay-wallet.png"
                                }
                            }
                        },
                        new()
                        {
                            Type = MyEnum.PlatformType.Vneid,
                            CustomKey = "vneid-setting",
                            Name = "vneid-setting",
                            Description = "vneid-setting",
                            BgColor = "#0c3a98",
                            TextColor = "#000000",
                            PaymentMethods = new()
                            {
                                new()
                                {
                                    Type = MyEnum.PaymentType.Qr,
                                    Name = "Thanh toán qua QRcode",
                                    Description = "Quét QRcode bằng ứng dụng ngân hàng hoặc ví điện tử",
                                    Icon = "/resources/payment-icon/1-qr.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.PayLater,
                                    Name = "Thanh toán trả sau",
                                    Description = "Lấy phiếu đặt chỗ và thực hiện thanh toán sau ",
                                    Icon = "/resources/payment-icon/1-paylater.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.LocalCard,
                                    Name = "Thẻ tín dụng và ghi nợ nội địa",
                                    Description = "Chấp nhận thẻ NAPAS",
                                    Icon = "/resources/payment-icon/1-local-card.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.GlobalCard,
                                    Name = "Thẻ tín dụng và ghi nợ quốc tế",
                                    Description = "Chấp nhận thẻ VISA/MASTERCARD",
                                    Icon = "/resources/payment-icon/1-global-card.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.EpayWallet,
                                    Name = "Ví điện tử EPAY",
                                    Description = "Sử dụng ví điện tử EPAY để thanh toán",
                                    Icon = "/resources/payment-icon/1-epay-wallet.png"
                                }
                            }
                        },
                        new()
                        {
                            Type = MyEnum.PlatformType.IACV,
                            CustomKey = "iacv-setting",
                            Name = "iacv-setting",
                            Description = "iacv-setting",
                            BgColor = "#0c3a98",
                            TextColor = "#000000",
                            PaymentMethods = new()
                            {
                                new()
                                {
                                    Type = MyEnum.PaymentType.Qr,
                                    Name = "Thanh toán qua QRcode",
                                    Description = "Quét QRcode bằng ứng dụng ngân hàng hoặc ví điện tử",
                                    Icon = "/resources/payment-icon/1-qr.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.PayLater,
                                    Name = "Thanh toán trả sau",
                                    Description = "Lấy phiếu đặt chỗ và thực hiện thanh toán sau",
                                    Icon = "/resources/payment-icon/1-paylater.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.LocalCard,
                                    Name = "Thẻ tín dụng và ghi nợ nội địa",
                                    Description = "Chấp nhận thẻ NAPAS",
                                    Icon = "/resources/payment-icon/1-local-card.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.GlobalCard,
                                    Name = "Thẻ tín dụng và ghi nợ quốc tế",
                                    Description = "Chấp nhận thẻ VISA/MASTERCARD",
                                    Icon = "/resources/payment-icon/1-global-card.png"
                                },
                                new()
                                {
                                    Type = MyEnum.PaymentType.EpayWallet,
                                    Name = "Ví điện tử EPAY",
                                    Description = "Sử dụng ví điện tử EPAY để thanh toán",
                                    Icon = "/resources/payment-icon/1-epay-wallet.png"
                                }
                            }
                        },
                    }
                },
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
                HasOtp = false,
                AdditionData = new(),
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Active = true
            },
        ]);
    }
}